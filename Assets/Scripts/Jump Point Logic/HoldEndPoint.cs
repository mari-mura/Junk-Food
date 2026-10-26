using PressInfo = TapHoldDetector.PressInfo;

public class HoldEndPoint : JumpPoint
{
    public override void OnActivate() => detector.HoldEnded += OnHoldEnded;
    public override void OnDeactivate() => detector.HoldEnded -= OnHoldEnded;
    
    protected override string DefaultPrompt => "RELEASE!";

    private void OnHoldEnded(PressInfo info)
    {
        if (info.IsChord) return;
        tracker.ReportRelease();
        Judge(info.Time);
    }
}