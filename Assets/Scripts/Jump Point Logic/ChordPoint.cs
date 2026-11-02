using UnityEngine;
using PressInfo = TapHoldDetector.PressInfo;

public class ChordPoint : JumpPoint
{
    [SerializeField] private ApproachCircle approachCircle;

    public override void OnActivate()
    {
        detector.TapStarted += OnTapStarted;
        approachCircle.Begin(tracker);
    }


    public override void OnDeactivate()
    {
        detector.TapStarted -= OnTapStarted;
        approachCircle.Stop();
    }

    protected override string DefaultPrompt => "TAP D + E!";
    private void OnTapStarted(PressInfo info)
    {
        if (!info.IsChord)
        {
            return;
        }
        Judge(info.PressTime);
    }

}
