using PressInfo = TapHoldDetector.PressInfo;

public class ChordHoldEndPoint : JumpPoint
{
    public override void OnActivate() => detector.HoldEnded += OnChordHoldEnded;
    public override void OnDeactivate() => detector.HoldEnded -= OnChordHoldEnded;

    private void OnChordHoldEnded(PressInfo info)
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