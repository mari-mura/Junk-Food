using UnityEngine;

public enum HitGrade
{
    Perfect,
    Great,
    Good,
    Miss
}
public class HitJudge
{
    public float PerfectDistance;
    public float GoodDistance;

    public HitJudge(float perfectDistanceInput, float goodDistanceInput)
    {
        PerfectDistance = perfectDistanceInput;
        GoodDistance = goodDistanceInput;
    }

    public HitGrade Judge(Vector2 playerPosition, PathPoint targetPoint)
    {
        float distance = Vector2.Distance(playerPosition, targetPoint.Position);

        if (distance <= PerfectDistance)
        {
            return HitGrade.Perfect;
        }

        if (distance <= GoodDistance)
        {
            return HitGrade.Good;
        }

        return HitGrade.Miss;
    }
    
    public float GetSignedOffset(Vector2 playerPosition, PathPoint targetPoint, Vector2 travelDirection)
    {
        Vector2 offset = playerPosition - targetPoint.Position;
        return Vector2.Dot(offset, travelDirection);
    }

    public bool HasPassedWindow(Vector2 playerPosition, PathPoint targetPoint, Vector2 travelDirection)
    {
        float signedOffset = GetSignedOffset(playerPosition, targetPoint, travelDirection);
        return signedOffset > GoodDistance;
    }
}