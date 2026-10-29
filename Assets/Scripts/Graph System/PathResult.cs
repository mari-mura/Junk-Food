public class PathResult
{
    public bool WasJudged;
    public HitGrade Grade;
    public PathPoint JudgedPoint;
    public PathEdge NextEdge;
    public PathPoint NextTarget;
    public float SignedOffset;
    public bool PathFinished;
}