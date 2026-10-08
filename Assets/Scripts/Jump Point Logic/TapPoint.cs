using PressInfo = TapHoldDetector.PressInfo;

public class TapPoint : JumpPoint
{
    public override void OnActivate() => detector.TapStarted += OnTapStarted;
    public override void OnDeactivate() => detector.TapStarted -= OnTapStarted;

    private void OnTapStarted(PressInfo info)
    {
        if (info.IsChord) return;
        Judge(info.PressTime);
    }
}