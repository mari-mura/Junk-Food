using UnityEngine;

public class WorldPoint : JumpPoint
{
    protected override string DefaultPrompt => "";

    public override void OnActivate()
    {
        // Nothing
    }

    public override void OnDeactivate()
    {
        // Nothing
    }

    public override void Update()
    {
        Succeed();
    }
}