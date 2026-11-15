using PressInfo = TapHoldDetector.PressInfo;

public class HoldStartPoint : JumpPoint
{
    public override void OnActivate() => detector.HoldStarted += OnHoldStarted;
    public override void OnDeactivate() => detector.HoldStarted -= OnHoldStarted;

    private void OnHoldStarted(PressInfo info)
    {
        if (info.IsChord) return;
        tracker.ReportRelease();
        Judge(info.Time);
    }
}