using System;
using System.Collections;
using UnityEngine;
using UnityEngine. InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ButtonHold : MonoBehaviour
{
    [SerializeField] private Image _fillImage; 
    [SerializeField] private float drainTime = 0.5f;
    // ^ Need to find out how long the hold is supposed to be
    [SerializeField] private Color fillColor = Color.yellow;
    [SerializeField] private Color failColor = Color.red;
    [SerializeField] private Color successColor = Color.lightGreen;
    
    private bool isHolding = false;
    private bool hasTriggered = false;
    
    public bool isEmpty => _fillImage.fillAmount == 0f;
    
    private void Start()
    {
        ResetFill();
    }

    private void Update()
    {
        if (!isHolding || hasTriggered)
            return;
        
        _fillImage.fillAmount = Mathf.Clamp01(_fillImage.fillAmount - Time.deltaTime / drainTime);
        UpdateFillColor();

        if (_fillImage.fillAmount <= 0f)
        {
            hasTriggered = true;
            isHolding = false;
        }
    }
    
    private void ResetFill()
    {
        isHolding = false;
        hasTriggered = false;
        _fillImage.fillAmount = 1f;
        _fillImage.color = fillColor;
    }
    
    public void StartHold()
    {
        isHolding = true;
    }

    public void StopHold()
    {
        isHolding = false;
        
        if (_fillImage.fillAmount <= 0f)
            SuccessfulHold();
        else
        {
            FailedHold();
        }
    }

    // Blends the fill color based on how full the circle is (neat!)
    public void UpdateFillColor()
    {
        _fillImage.color = Color.Lerp(fillColor, successColor, Time.deltaTime * drainTime);
    }

    public void SuccessfulHold()
    {
        _fillImage.color = successColor;
        
        StartCoroutine(DelayedReset());
    }

    public void FailedHold()
    {
        _fillImage.color = failColor;

        StartCoroutine(DelayedReset());
    }

    private IEnumerator DelayedReset()
    {
        yield return new WaitForSeconds(0.5f);
        ResetFill();
    }
}
