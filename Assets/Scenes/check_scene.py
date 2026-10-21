#!/usr/bin/env python3
"""
check_scene.py - sanity-check a Unity YAML scene or prefab after a git merge.

Run it from the Unity project root (the folder that contains Assets/):

  python3 check_scene.py Assets/Scenes/TB_Level1.unity
  python3 check_scene.py Assets/Scenes/TB_Level1.unity --ours /tmp/ours.unity --theirs /tmp/theirs.unity

It reports:
  1. Leftover git conflict markers
  2. Duplicate object IDs
  3. Dangling references (a {fileID: N} that points at an object not in the file)
  4. Parent/child mismatches in the Transform hierarchy
  5. Asset GUIDs (prefabs, scripts) with no matching .meta file
  6. With --ours/--theirs: objects present in either side but missing from the merge
"""
import argparse
import os
import re
import sys
from collections import defaultdict

HEADER = re.compile(r'^--- !u!(\d+) &(-?\d+)( stripped)?\s*$')
REF = re.compile(r'\{fileID:\s*(-?\d+)(?:,\s*guid:\s*([0-9a-fA-F]{32}))?(?:,\s*type:\s*\d+)?\s*\}')
MARKER = re.compile(r'^(<<<<<<< |=======\s*$|>>>>>>> )')

CLASS_NAMES = {
    1: "GameObject", 4: "Transform", 224: "RectTransform", 114: "MonoBehaviour",
    1001: "PrefabInstance", 212: "SpriteRenderer", 222: "CanvasRenderer",
    223: "Canvas", 61: "BoxCollider2D", 50: "Rigidbody2D", 20: "Camera",
}


class Doc:
    def __init__(self, cls, id_, stripped, start):
        self.cls = cls
        self.id = id_
        self.stripped = stripped
        self.start = start
        self.lines = []  # (line number, text)


class Scene:
    def __init__(self, path):
        self.path = path
        self.docs = {}       # id -> first Doc with that id
        self.all_docs = []   # every Doc in file order
        self.markers = []    # (line number, text)
        cur = None
        with open(path, encoding="utf-8", errors="replace") as f:
            for n, raw in enumerate(f, 1):
                line = raw.rstrip("\n")
                if MARKER.match(line):
                    self.markers.append((n, line.strip()))
                    continue
                m = HEADER.match(line)
                if m:
                    cur = Doc(int(m.group(1)), m.group(2), bool(m.group(3)), n)
                    self.all_docs.append(cur)
                    self.docs.setdefault(cur.id, cur)
                    continue
                if cur is not None:
                    cur.lines.append((n, line))

    def go_name(self, doc):
        for _, t in doc.lines:
            m = re.match(r'\s*m_Name:\s*(.*)$', t)
            if m:
                return m.group(1).strip()
        return ""

    def label(self, doc):
        kind = CLASS_NAMES.get(doc.cls, f"class {doc.cls}")
        name = ""
        if doc.stripped:
            name = "stripped prefab object"
        elif doc.cls == 1:
            name = self.go_name(doc)
        else:
            for _, t in doc.lines:
                m = re.match(r'\s*m_GameObject:\s*\{fileID:\s*(-?\d+)\}', t)
                if m:
                    go = self.docs.get(m.group(1))
                    if go is not None and go.cls == 1:
                        name = self.go_name(go)
                    break
        extra = f' "{name}"' if name else ""
        return f"{kind} &{doc.id}{extra}"

    def local_refs(self, doc):
        for n, t in doc.lines:
            for m in REF.finditer(t):
                fid, guid = m.group(1), m.group(2)
                if guid is None and fid != "0":
                    yield n, fid, t.strip()

    def external_guids(self, doc):
        for n, t in doc.lines:
            for m in REF.finditer(t):
                guid = m.group(2)
                if guid and not guid.startswith("0" * 16):
                    yield n, guid.lower(), t.strip()

    def transform_info(self, doc):
        father, children, in_children = None, [], False
        for _, t in doc.lines:
            s = t.strip()
            m = re.match(r'm_Father:\s*\{fileID:\s*(-?\d+)\}', s)
            if m:
                father = m.group(1)
                in_children = False
                continue
            if s.startswith("m_Children:"):
                in_children = not s.endswith("[]")
                continue
            if in_children:
                m = re.match(r'-\s*\{fileID:\s*(-?\d+)\}', s)
                if m:
                    children.append(m.group(1))
                    continue
                in_children = False
        return father, children


def find_root(scene_path, hint):
    candidates = [hint, os.getcwd()]
    p = os.path.abspath(os.path.dirname(scene_path))
    while True:
        candidates.append(p)
        parent = os.path.dirname(p)
        if parent == p:
            break
        p = parent
    for c in candidates:
        if c and os.path.isdir(os.path.join(c, "Assets")):
            return c
    return None


def load_known_guids(root):
    known = set()
    for sub in ("Assets", "Packages", os.path.join("Library", "PackageCache")):
        base = os.path.join(root, sub)
        for dirpath, _, files in os.walk(base):
            for fn in files:
                if not fn.endswith(".meta"):
                    continue
                try:
                    with open(os.path.join(dirpath, fn), encoding="utf-8", errors="ignore") as f:
                        for _ in range(6):
                            line = f.readline()
                            if line.startswith("guid:"):
                                known.add(line.split(":", 1)[1].strip().lower())
                                break
                except OSError:
                    pass
    return known


def section(title):
    print(f"\n=== {title} ===")


def main():
    ap = argparse.ArgumentParser(description="Check a Unity scene for merge damage.")
    ap.add_argument("scene")
    ap.add_argument("--ours", help="your branch's version of the scene")
    ap.add_argument("--theirs", help="main's version of the scene")
    ap.add_argument("--project", help="Unity project root (folder containing Assets)")
    args = ap.parse_args()

    scene = Scene(args.scene)
    problems = 0
    print(f"Checked {args.scene}: {len(scene.all_docs)} objects")

    # 1. Conflict markers
    section("1. Leftover conflict markers")
    if scene.markers:
        problems += len(scene.markers)
        for n, t in scene.markers:
            print(f"  line {n}: {t}")
    else:
        print("  none")

    # 2. Duplicate IDs
    section("2. Duplicate object IDs")
    by_id = defaultdict(list)
    for d in scene.all_docs:
        by_id[d.id].append(d)
    dups = {i: ds for i, ds in by_id.items() if len(ds) > 1}
    if dups:
        problems += len(dups)
        for i, ds in dups.items():
            where = ", ".join(f"line {d.start}" for d in ds)
            print(f"  &{i} defined {len(ds)} times ({where})")
    else:
        print("  none")

    # 3. Dangling references
    section("3. Dangling references (point at an object that isn't in the file)")
    dangling = defaultdict(list)
    for d in scene.all_docs:
        for n, fid, text in scene.local_refs(d):
            if fid not in scene.docs:
                dangling[fid].append((n, scene.label(d), text))
    if dangling:
        problems += len(dangling)
        for fid, refs in dangling.items():
            print(f"  &{fid} does not exist, referenced {len(refs)} time(s):")
            for n, owner, text in refs[:5]:
                print(f"      line {n} in {owner}: {text}")
            if len(refs) > 5:
                print(f"      ... and {len(refs) - 5} more")
    else:
        print("  none")

    # 4. Hierarchy consistency
    section("4. Parent/child mismatches")
    mismatches = []
    for d in scene.all_docs:
        if d.cls not in (4, 224) or d.stripped:
            continue
        father, children = scene.transform_info(d)
        for c in children:
            cd = scene.docs.get(c)
            if cd is None or cd.stripped or cd.cls not in (4, 224):
                continue
            cf, _ = scene.transform_info(cd)
            if cf != d.id:
                mismatches.append(f"{scene.label(d)} lists child &{c}, but that child's parent is &{cf}")
        if father and father != "0":
            pd = scene.docs.get(father)
            if pd is not None and not pd.stripped and pd.cls in (4, 224):
                _, pchildren = scene.transform_info(pd)
                if d.id not in pchildren:
                    mismatches.append(f"{scene.label(d)} says its parent is &{father}, but the parent doesn't list it")
    if mismatches:
        problems += len(mismatches)
        for m in mismatches:
            print("  " + m)
    else:
        print("  none")

    # 5. GUIDs
    section("5. Asset GUIDs with no .meta file (missing prefabs/scripts)")
    root = find_root(args.scene, args.project)
    if root is None:
        print("  skipped: run from the project root (the folder containing Assets/)")
    else:
        known = load_known_guids(root)
        missing = defaultdict(list)
        for d in scene.all_docs:
            for n, guid, text in scene.external_guids(d):
                if guid not in known:
                    missing[guid].append((n, scene.label(d)))
        if missing:
            problems += len(missing)
            for guid, uses in missing.items():
                print(f"  {guid} not found ({len(uses)} use(s)), first at line {uses[0][0]} in {uses[0][1]}")
            if not os.path.isdir(os.path.join(root, "Library", "PackageCache")):
                print("  note: Library/PackageCache not found, so Unity package GUIDs may show up as false alarms")
        else:
            print("  none")

    # 6. Compare against each side
    for tag, path in (("OURS (your branch)", args.ours), ("THEIRS (main)", args.theirs)):
        if not path:
            continue
        section(f"6. In {tag} but missing from the merged file")
        other = Scene(path)
        gone = [d for i, d in other.docs.items() if i not in scene.docs]
        referenced = [d for d in gone if d.id in dangling]
        others = [d for d in gone if d.id not in dangling]
        if not gone:
            print("  nothing missing")
            continue
        print("  Objects marked BROKEN are still referenced by the merged file.")
        print("  The rest are only a problem if you meant to keep them.")
        for d in referenced:
            print(f"  BROKEN  {other.label(d)} (line {d.start} in that file)")
        for d in others[:30]:
            print(f"          {other.label(d)} (line {d.start} in that file)")
        if len(others) > 30:
            print(f"          ... and {len(others) - 30} more")

    print()
    if problems:
        print(f"RESULT: {problems} problem(s) found. Fix these before opening the scene in Unity.")
        return 1
    print("RESULT: no structural problems found.")
    return 0


if __name__ == "__main__":
    sys.exit(main())