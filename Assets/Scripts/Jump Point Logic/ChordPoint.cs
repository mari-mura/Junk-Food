using UnityEngine;
using PressInfo = TapHoldDetector.PressInfo;

public class ChordPoint : JumpPoint
{
    public override void OnActivate() => detector.TapStarted += OnTapStarted;
    public override void OnDeactivate() => detector.TapStarted -= OnTapStarted;

    protected override string DefaultPrompt => "TAP BOTH D & E!";
    private void OnTapStarted(PressInfo info)
    {
        if (!info.IsChord)
        {
            return;
        }
        Judge(info.PressTime);
    }

}
