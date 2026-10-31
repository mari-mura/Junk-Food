using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerLauncher : MonoBehaviour
{

    [SerializeField] private LaunchBar launchBar;
    [SerializeField] private float minSpeed, maxSpeed;
    [SerializeField] private float secondsToFill;

    private Action<float> onLaunched;
    private bool waiting;
    private bool charging;
    private float heldTime;

    public void Launch(Action<float> speed)
    {
        onLaunched = speed;
        waiting = true;
        charging = false;
        launchBar.Hide();
    }

    private void Update()
    {
        if (waiting == false || Keyboard.current == null)
        {
            return;
        }

        KeyControl key = Keyboard.current.spaceKey;

        if (charging == false)
        {
            if (key.wasPressedThisFrame)
            {
                charging = true;
                heldTime = 0f;
                launchBar.Show();
            }

            return;
        }
        
        heldTime += Time.deltaTime;
        float percent = heldTime/secondsToFill;
        launchBar.SetFill(percent);

        if (key.wasReleasedThisFrame || percent >= 1f)
        {
            waiting = false;
            charging = false;
            launchBar.Hide();
            onLaunched(Mathf.Lerp(minSpeed, maxSpeed, percent));
        }
    }
}
