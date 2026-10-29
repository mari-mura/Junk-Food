using UnityEngine;
using UnityEngine.InputSystem;
public class InventoryToggle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject inventoryPanel;

    [Header("Input")]
    // i bound the inventory action to the toggle action inventory and key i
    [SerializeField] private InputActionReference toggleAction;

    [Header("Options")]
    [SerializeField] private bool startOpen = false;
    [SerializeField] private bool pauseGameWhenOpen = true;
    [SerializeField] private bool unlockCursorWhenOpen = true;

    public bool IsOpen { get; private set; }

    /* player input was disabling the script earlier idk why the toggle did that so now we have this
     it actually solve 920 errors i made earlier sooooo*/
    private bool enabledByInventoryToggleScript;

    private void OnEnable()
    {
        if (toggleAction == null)
        {
            Debug.LogWarning("no Toggle", this);
            return;
        }
        
        InputAction action = toggleAction.action;
        if (!action.enabled)
        {
            action.Enable();
            enabledByInventoryToggleScript = true;
        }
    }

    private void OnDisable()
    {
        if (toggleAction != null && enabledByInventoryToggleScript)
        {
            toggleAction.action.Disable();
        }
        enabledByInventoryToggleScript = false;
    }

    /* im worried other scenes will be jank when a level loads from a pause or an error so im just
     forcing the panel, time, and cursor to start*/
    private void Start()
    {
        SetOpen(startOpen);
    }

    private void Update()
    {
        if (toggleAction != null && toggleAction.action.WasPressedThisFrame())
        {
            Toggle();
        }
    }

    // can add this to a button if we want
    public void Toggle()
    {
        SetOpen(!IsOpen);
    }

    public void SetOpen(bool open)
    {
        IsOpen = open;

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(open);
        }

        if (pauseGameWhenOpen)
        {
            Time.timeScale = open ? 0f : 1f;
        }

        if (unlockCursorWhenOpen)
        {
            Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = open;
        }
    }

    // makes sure when you leave the level time unfreezes still
    private void OnDestroy()
    {
        if (pauseGameWhenOpen)
        {
            Time.timeScale = 1f;
        }
    }
}