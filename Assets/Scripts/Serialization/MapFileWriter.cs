
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapFileWriter : MonoBehaviour
{
    [SerializeField] private List<string> levelSceneNames = new List<string>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        WriteMaps();
    }

    public void WriteMaps()
    {
        StartCoroutine(WriteMapsRoutine());
    }

    private IEnumerator WriteMapsRoutine()
    {
        List<LevelData> levelMaps = new List<LevelData>();

        foreach (string sceneName in levelSceneNames)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);

            levelMaps.Add(ReadLevelScene(sceneName));
        }

        SaveFile(levelMaps);
    }

    private LevelData ReadLevelScene(string sceneName)
    {
        LevelData map = new LevelData();
        map.MapId = sceneName;

        PathPoint[] points = FindObjectsByType<PathPoint>(FindObjectsSortMode.None);

        foreach (PathPoint point in points)
        {
            map.Points.Add(ReadPoint(point));
        }

        return map;
    }

    private PointData ReadPoint(PathPoint point)
    {
        PointData data = new PointData();
        data.UniquePointId = point.UniquePointId;
        data.Type = point.Type;
        data.IsStartPoint = point.IsStartPoint;
        data.Position = point.Position;
        data.Connections = new List<PathConnection>(point.Connections);

        return data;
    }

    private void SaveFile(List<LevelData> levelMaps)
    {
        MapFile file = new MapFile();
        file.LevelMaps = levelMaps;

        KeepExistingWorldMaps(file);

        Directory.CreateDirectory(Application.streamingAssetsPath);

        string path = MapFiles.GetPath();
        string json = JsonUtility.ToJson(file, true);

        File.WriteAllText(path, json);
    }

    private void KeepExistingWorldMaps(MapFile file)
    {
        if (File.Exists(MapFiles.GetPath()))
        {
            MapFile existing = MapFiles.ReadFile();
            file.WorldMaps = existing.WorldMaps;
        }
    }
}
