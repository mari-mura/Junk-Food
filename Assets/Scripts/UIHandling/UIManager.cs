/*using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ButtonHold _buttonHold;
    [SerializeField] private TapHoldReceiver _receiver;

    private void Start()
    {
        ListenForHoldAction();
    }

    private void ListenForHoldAction()
    {
       //_buttonHold.ConnectToAction(_receiver.HoldAction);
       _receiver.HoldAction.performed += OnHoldPerformed;
    }

    private void StopHoldAction()
    {
        //_buttonHold.Disconnect();
        _receiver.HoldAction.performed -= OnHoldPerformed;
    }

    private void OnHoldPerformed(InputAction.CallbackContext obj)
    {
        Debug.Log("Hold Perfmored, Do Something");
        
        _buttonHold.OnHold();
    }
    
    
}
*/
