using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class MixingMinigame : MonoBehaviour
{
    [SerializeField] private InputActionReference mix;
    [SerializeField] private Transform bowl;

    [SerializeField] private int rotationsNeeded = 5;
    [SerializeField] private float deadzone = 0.5f;
    [SerializeField] private float spinSpeed = 360f;
    public UnityEvent onComplete;

    private float lastAngle;
    private bool hasLastAngle;
    private float totalDegrees;
    private float visualAngle;
    private int rotationsCompleted;
    private bool finished;

    void OnEnable()
    {
        mix.action.Enable();
    }

    void Update()
    {
        visualAngle = Mathf.MoveTowards(visualAngle, totalDegrees, spinSpeed * Time.deltaTime);
        bowl.rotation = Quaternion.Euler(0f, 0f, -visualAngle);

        if (finished)
        {
            return;
        }

        Vector2 input = mix.action.ReadValue<Vector2>();
        if (input.magnitude < deadzone)
        {
            return;
        }

        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

        if (hasLastAngle)
        {
            float delta = Mathf.DeltaAngle(lastAngle, angle);
            if (delta < 0)
            {
                totalDegrees += -delta;
            }
        }

        lastAngle = angle;
        hasLastAngle = true;

        int currentRotations = Mathf.FloorToInt(totalDegrees / 360f);
        if (currentRotations > rotationsCompleted)
        {
            rotationsCompleted = currentRotations;
            Debug.Log("One rotation complete");
        }

        if (totalDegrees >= rotationsNeeded * 360f)
        {
            finished = true;
            onComplete?.Invoke();
        }
    }

    public void Finish()
    {
        Debug.Log("Finished");
    }
    
}