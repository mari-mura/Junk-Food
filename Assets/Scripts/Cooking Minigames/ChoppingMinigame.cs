using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ChoppingMinigame : MonoBehaviour
{
    [SerializeField] private InputActionReference chop;
    [SerializeField] private Transform[] slices;

    [SerializeField] private int pressesPerCut = 2;
    [SerializeField] private float sliceGap = 0.25f;

    public UnityEvent onComplete;
    private int presses;
    private int cutsMade;

    void OnEnable()
    {
        chop.action.Enable();
        chop.action.performed += OnChop;
    }

    void OnDisable()
    {
        chop.action.performed -= OnChop;
    }

    void OnChop(InputAction.CallbackContext context)
    {
        int cutsNeeded = slices.Length - 1;
        if (cutsMade >= cutsNeeded)
        {
            return;
        }

        presses++;
        if (presses < pressesPerCut)
        {
            return;
        }

        presses = 0;
        cutsMade++;

        for (int i = cutsMade; i < slices.Length; i++)
            slices[i].position += Vector3.right * sliceGap;

        if (cutsMade >= cutsNeeded)
        {
            onComplete?.Invoke();
        }
    }
        
}

