using UnityEngine;
using UnityEngine. InputSystem;
using UnityEngine.UI;

public class ButtonHold : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    
    private InputAction _currentAction;

    private void Update()
    {
        if (_currentAction != null)
        {
            _fillImage.fillAmount = _currentAction.GetTimeoutCompletionPercentage();
        }
    }

    public void ConnectToAction(InputAction holdAction)
    {
        _currentAction = holdAction;
        _currentAction.canceled += DoSomethingOnCancel;
    }

    private void DoSomethingOnCancel(InputAction.CallbackContext obj)
    {
        Debug.Log("Hold Canceled - this is called whether performed or not");
    }

    public void Disconnect()
    {
        _currentAction = null;
    }
}
