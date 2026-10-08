using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class TapHoldDetector : MonoBehaviour
{
    [Serializable]
    public class ButtonBinding
    {
        public string Name = "South";
        public InputActionReference ActionReference;
        public string BindingPath = "<Gamepad>/buttonSouth";
        public string KeyboardBindingPath = "<Keyboard>/s";
    }

    public struct PressInfo
    {
        public int Id;
        public double PressTime;
        public double Time;
        public List<string> Buttons;
        public double ChordSpread;

        public bool IsChord
        {
            get { return Buttons.Count > 1; }
        }

        public PressInfo(int id, double pressTime, double time, List<string> buttons, double chordSpread)
        {
            Id = id;
            PressTime = pressTime;
            Time = time;
            Buttons = buttons;
            ChordSpread = chordSpread;
        }
    }

    [SerializeField] private List<ButtonBinding> buttons = new List<ButtonBinding>
    {
        new ButtonBinding { Name = "South", BindingPath = "<Gamepad>/buttonSouth", KeyboardBindingPath = "<Keyboard>/s" },
        new ButtonBinding { Name = "East",  BindingPath = "<Gamepad>/buttonEast",  KeyboardBindingPath = "<Keyboard>/d" },
        new ButtonBinding { Name = "West",  BindingPath = "<Gamepad>/buttonWest",  KeyboardBindingPath = "<Keyboard>/a" },
        new ButtonBinding { Name = "North", BindingPath = "<Gamepad>/buttonNorth", KeyboardBindingPath = "<Keyboard>/w" },
    };

    [SerializeField, Min(0.01f)] private float holdThreshold = 0.25f;
    [SerializeField, Range(0f, 0.1f)] private float chordWindow = 0.04f;
    [SerializeField, Min(2)] private int maxChordSize = 2;

    public event Action<string, double> ButtonDown;
    public event Action<PressInfo> TapStarted;
    public event Action<PressInfo> TapReleased;
    public event Action<PressInfo> HoldStarted;
    public event Action<PressInfo> HoldEnded;

    public float HoldThreshold
    {
        get { return holdThreshold; }
    }

    public float ChordWindow
    {
        get { return chordWindow; }
    }

    private class Press
    {
        public int Id;
        public List<int> ButtonIndexes = new List<int>();
        public List<string> ButtonNames = new List<string>();
        public double FirstDownTime;
        public double LastDownTime;
        public bool HasStarted;
        public bool HasHeld;
    }

    private class Button
    {
        public ButtonBinding Binding;
        public InputAction Action;
        public bool OwnsAction;
        public bool IsDown;
        public Press CurrentPress;
        public System.Action<InputAction.CallbackContext> HandleDown;
        public System.Action<InputAction.CallbackContext> HandleUp;
    }

    private List<Button> allButtons = new List<Button>();
    private List<Press> openPresses = new List<Press>();
    private Press pressAcceptingChordMembers;
    private int nextPressId;

    private void OnValidate()
    {
        if (holdThreshold <= chordWindow)
        {
            holdThreshold = chordWindow + 0.01f;
        }
    }

    private void OnEnable()
    {
        foreach (ButtonBinding binding in buttons)
        {
            Button button = BuildButton(binding);
            StartListeningTo(button);
            allButtons.Add(button);
        }
    }

    private void OnDisable()
    {
        foreach (Button button in allButtons)
        {
            StopListeningTo(button);
            ReleaseOwnedAction(button);
        }

        allButtons.Clear();
        openPresses.Clear();
        pressAcceptingChordMembers = null;
    }

    private void Update()
    {
        double now = InputState.currentTime;
        CloseChordWindowIfExpired(now);
        StartHoldsThatCrossedThreshold(now);
    }

    private Button BuildButton(ButtonBinding binding)
    {
        Button button = new Button();
        button.Binding = binding;

        if (HasCustomAction(binding))
        {
            button.Action = binding.ActionReference.action;
            button.OwnsAction = false;
        }
        else
        {
            button.Action = new InputAction(binding.Name, InputActionType.Button);
            AddBindingIfSet(button.Action, binding.BindingPath);
            AddBindingIfSet(button.Action, binding.KeyboardBindingPath);
            button.OwnsAction = true;
        }

        return button;
    }

    private bool HasCustomAction(ButtonBinding binding)
    {
        return binding.ActionReference != null && binding.ActionReference.action != null;
    }

    private void AddBindingIfSet(InputAction action, string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            action.AddBinding(path);
        }
    }

    private void StartListeningTo(Button button)
    {
        button.HandleDown = context => OnButtonDown(button, context.time);
        button.HandleUp = context => OnButtonUp(button, context.time);

        button.Action.performed += button.HandleDown;
        button.Action.canceled += button.HandleUp;
        button.Action.Enable();
    }

    private void StopListeningTo(Button button)
    {
        button.Action.performed -= button.HandleDown;
        button.Action.canceled -= button.HandleUp;
        button.Action.Disable();
    }

    private void ReleaseOwnedAction(Button button)
    {
        if (button.OwnsAction)
        {
            button.Action.Dispose();
        }
    }

    private void OnButtonDown(Button button, double time)
    {
        if (button.IsDown)
        {
            return;
        }
        button.IsDown = true;

        RaiseButtonDown(button.Binding.Name, time);

        Press press = FindPressToJoin(time);
        if (press == null)
        {
            press = StartNewPress(time);
        }

        AddButtonToPress(press, button, time);

        if (PressShouldStartNow(press))
        {
            StartPress(press);
        }
    }

    private Press FindPressToJoin(double time)
    {
        if (pressAcceptingChordMembers == null)
        {
            return null;
        }

        bool stillWithinChordWindow = time - pressAcceptingChordMembers.FirstDownTime <= chordWindow;
        bool stillHasRoom = pressAcceptingChordMembers.ButtonIndexes.Count < maxChordSize;

        if (stillWithinChordWindow && stillHasRoom)
        {
            return pressAcceptingChordMembers;
        }

        return null;
    }

    private Press StartNewPress(double time)
    {
        nextPressId++;

        Press press = new Press();
        press.Id = nextPressId;
        press.FirstDownTime = time;

        pressAcceptingChordMembers = press;
        openPresses.Add(press);
        return press;
    }

    private void AddButtonToPress(Press press, Button button, double time)
    {
        press.ButtonIndexes.Add(allButtons.IndexOf(button));
        press.LastDownTime = time;
        button.CurrentPress = press;
    }

    private bool PressShouldStartNow(Press press)
    {
        bool chordsAreDisabled = chordWindow <= 0f;
        bool pressIsFull = press.ButtonIndexes.Count >= maxChordSize;
        return chordsAreDisabled || pressIsFull;
    }

    // ----- Button went up -----

    private void OnButtonUp(Button button, double time)
    {
        if (!button.IsDown)
        {
            return;
        }
        button.IsDown = false;

        Press press = button.CurrentPress;
        button.CurrentPress = null;

        if (press == null)
        {
            return;
        }

        EndPress(press, time);
    }

    private void EndPress(Press press, double releaseTime)
    {
        StartPress(press);
        openPresses.Remove(press);
        DetachAllButtonsFrom(press);

        if (WasQuickTap(press, releaseTime))
        {
            RaiseTapReleased(MakeInfo(press, releaseTime));
        }
        else
        {
            EndHold(press, releaseTime);
        }
    }

    private void DetachAllButtonsFrom(Press press)
    {
        foreach (int buttonIndex in press.ButtonIndexes)
        {
            allButtons[buttonIndex].CurrentPress = null;
        }
    }

    private bool WasQuickTap(Press press, double releaseTime)
    {
        return releaseTime - press.FirstDownTime < holdThreshold;
    }

    private void CloseChordWindowIfExpired(double now)
    {
        if (pressAcceptingChordMembers == null)
        {
            return;
        }

        bool chordWindowExpired = now - pressAcceptingChordMembers.FirstDownTime >= chordWindow;
        if (chordWindowExpired)
        {
            StartPress(pressAcceptingChordMembers);
        }
    }

    private void StartPress(Press press)
    {
        if (press.HasStarted)
        {
            return;
        }
        press.HasStarted = true;

        if (pressAcceptingChordMembers == press)
        {
            pressAcceptingChordMembers = null;
        }

        SortButtonsInInspectorOrder(press);
        RaiseTapStarted(MakeInfo(press, press.FirstDownTime));
    }

    private void SortButtonsInInspectorOrder(Press press)
    {
        press.ButtonIndexes.Sort();

        press.ButtonNames.Clear();
        foreach (int buttonIndex in press.ButtonIndexes)
        {
            press.ButtonNames.Add(allButtons[buttonIndex].Binding.Name);
        }
    }

    private void StartHoldsThatCrossedThreshold(double now)
    {
        foreach (Press press in openPresses)
        {
            if (HasCrossedHoldThreshold(press, now))
            {
                StartHold(press);
            }
        }
    }

    private bool HasCrossedHoldThreshold(Press press, double now)
    {
        return press.HasStarted && !press.HasHeld && now - press.FirstDownTime >= holdThreshold;
    }

    private void StartHold(Press press)
    {
        if (press.HasHeld)
        {
            return;
        }
        press.HasHeld = true;

        double holdStartTime = press.FirstDownTime + holdThreshold;
        RaiseHoldStarted(MakeInfo(press, holdStartTime));
    }

    private void EndHold(Press press, double releaseTime)
    {
        StartHold(press);
        RaiseHoldEnded(MakeInfo(press, releaseTime));
    }

    private static PressInfo MakeInfo(Press press, double time)
    {
        double chordSpread = press.LastDownTime - press.FirstDownTime;
        return new PressInfo(press.Id, press.FirstDownTime, time, press.ButtonNames, chordSpread);
    }

    private void RaiseButtonDown(string buttonName, double time)
    {
        if (ButtonDown != null)
        {
            ButtonDown.Invoke(buttonName, time);
        }
    }

    private void RaiseTapStarted(PressInfo pressInfo)
    {
        if (TapStarted != null)
        {
            TapStarted.Invoke(pressInfo);
        }
    }

    private void RaiseTapReleased(PressInfo pressInfo)
    {
        if (TapReleased != null)
        {
            TapReleased.Invoke(pressInfo);
        }
    }

    private void RaiseHoldStarted(PressInfo pressInfo)
    {
        if (HoldStarted != null)
        {
            HoldStarted.Invoke(pressInfo);
        }
    }

    private void RaiseHoldEnded(PressInfo pressInfo)
    {
        if (HoldEnded != null)
        {
            HoldEnded.Invoke(pressInfo);
        }
    }
}