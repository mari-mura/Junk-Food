using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugConsole : MonoBehaviour
{
    public Key ToggleOverlay = Key.Backspace;
    public Key ReloadScene = Key.F5;
    
    public PathFollower Follower;
    
    private bool visible = false;
    private Rect windowRectangle = new Rect(20, 20, 330, 295);
    private string sceneToLoad = "";
    private string noiseText = "10";
    private string speedText = "300";

    private void Awake()
    {
        if (Debug.isDebugBuild == false)
        {
            enabled = false;
        }
    }

    private void Start()
    {
        if (Follower == null)
        {
            Follower = FindFirstObjectByType<PathFollower>();
        }
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard[ToggleOverlay].wasPressedThisFrame)
        {
            visible = !visible;
        }

        if (keyboard[ReloadScene].wasPressedThisFrame)
        {
            ReloadActiveScene();
        }
    }

    private void OnGUI()
    {
        if (visible == false)
        {
            return;
        }

        windowRectangle = GUI.Window(123456789, windowRectangle, DrawWindow, "Debug");
    }

    private void DrawWindow(int id)
    {
        DrawStatus();
        DrawNoiseControls();
        DrawMovementControls();
        DrawSceneControls();
        
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
    

    private void DrawStatus()
    {
        GUI.Label(new Rect(10, 25, 310, 20),
            "Noise: " + NoiseTracker.Noise.ToString("F1") + " / " + NoiseTracker.MaxNoise);

        PathSession session = GetSession();
        if (session == null)
        {
            GUI.Label(new Rect(10, 45, 310, 20), "Target: (no session found)");
            return;
        }

        string targetText = "none";
        if (session.CurrentTarget != null)
        {
            targetText = session.CurrentTarget.UniquePointId + " [" + session.CurrentTarget.Type + "]";
        }

        GUI.Label(new Rect(10, 45, 310, 20), "Target: " + targetText);
        GUI.Label(new Rect(10, 65, 310, 20),
            "Holding: " + session.IsHolding + "   Finished: " + session.IsFinished);
    }
    

    private void DrawNoiseControls()
    {
        GUI.Label(new Rect(10, 95, 100, 20), "Noise amount");
        noiseText = GUI.TextField(new Rect(115, 95, 50, 20), noiseText);
        float amount = ParseFloat(noiseText, 0f);

        if (GUI.Button(new Rect(175, 95, 60, 20), "+"))
        {
            NoiseTracker.Increase(amount);
        }

        if (GUI.Button(new Rect(240, 95, 60, 20), "-"))
        {
            NoiseTracker.Decrease(amount);
        }

        if (GUI.Button(new Rect(10, 125, 130, 22), "Reset noise"))
        {
            NoiseTracker.Reset();
        }

        if (GUI.Button(new Rect(150, 125, 150, 22), "Max noise (caught)"))
        {
            NoiseTracker.Increase(NoiseTracker.MaxNoise);
        }
    }
    

    private void DrawMovementControls()
    {
        if (Follower == null)
        {
            GUI.Label(new Rect(10, 160, 310, 20), "No PathFollower found in the scene.");
            return;
        }

        GUI.Label(new Rect(10, 160, 100, 20), "Player speed");
        speedText = GUI.TextField(new Rect(115, 160, 50, 20), speedText);

        if (GUI.Button(new Rect(175, 160, 60, 20), "Set"))
        {
            Follower.PlayerSpeed = ParseFloat(speedText, Follower.PlayerSpeed);
        }

        GUI.Label(new Rect(240, 160, 80, 20), "now " + Follower.PlayerSpeed.ToString("F0"));

        if (GUI.Button(new Rect(10, 190, 150, 22), "Teleport to target"))
        {
            TeleportToTarget();
        }
    }

    private void TeleportToTarget()
    {
        PathSession session = GetSession();
        if (session == null || session.CurrentTarget == null)
        {
            return;
        }
        
        Vector3 position = Follower.transform.position;
        position.x = session.CurrentTarget.Position.x;
        position.y = session.CurrentTarget.Position.y;
        Follower.transform.position = position;
    }
    
    private void DrawSceneControls()
    {
        if (GUI.Button(new Rect(10, 225, 150, 22), "Reload Scene (F5)"))
        {
            ReloadActiveScene();
        }

        GUI.Label(new Rect(10, 258, 80, 20), "Load scene");
        sceneToLoad = GUI.TextField(new Rect(90, 258, 120, 20), sceneToLoad);

        if (GUI.Button(new Rect(220, 258, 70, 20), "Load"))
        {
            LoadScene(sceneToLoad);
        }
    }

    private void ReloadActiveScene()
    {
        NoiseTracker.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName) == false)
        {
            NoiseTracker.Reset();
            SceneManager.LoadScene(sceneName);
        }
    }
    

    private PathSession GetSession()
    {
        if (Follower == null)
        {
            return null;
        }
        return Follower.Session;
    }

    private float ParseFloat(string input, float defaultValue)
    {
        float parsedValue;
        if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out parsedValue))
        {
            return parsedValue;
        }
        return defaultValue;
    }
}