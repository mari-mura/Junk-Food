using PressInfo = TapHoldDetector.PressInfo;

public class ChordHoldStartPoint : JumpPoint
{
    public override void OnActivate() => detector.HoldStarted += OnChordHoldStarted;
    public override void OnDeactivate() => detector.HoldStarted -= OnChordHoldStarted;

    private void OnChordHoldStarted(PressInfo info)
    {
        if (info.IsChord)
        {
            tracker.ReportRelease();
            Judge(info.Time);
        }
        else
        {
            return;
        }
    }
}