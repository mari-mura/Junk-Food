using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public float PlayerSpeed = 5.0f;
    public PathResult LastMissResult;
    public PathSession Session;


    public void Awake()
    {
        PathGraph demomap = DemoMapBuilder.Build();
        HitJudge hitJudge = new HitJudge(0.5f, 0.8f);
        StartPath(demomap, hitJudge);
        Session.Initialize(demomap, hitJudge);
    }
    
    public void StartPath(PathGraph graph, HitJudge judge)
    {
        if (Session.CurrentTarget != null)
        {
            transform.position = Session.CurrentTarget.Position;
        }
    }

    void Update()
    {
        if (Session == null || Session.IsFinished)
        {
            return;
        }

        Vector2 currentPosition = transform.position;
        float stepDistance = PlayerSpeed * Time.deltaTime;
        Vector2 newPosition = currentPosition;

        float signedOffset = Vector2.Dot(currentPosition - Session.CurrentTarget.Position,
            Session.TravelDirection);

        if (signedOffset < 0.0f)
        {
            newPosition = Vector2.MoveTowards(currentPosition, Session.CurrentTarget.Position,
                stepDistance);
        }
        else
        {
            newPosition = currentPosition + Session.TravelDirection * stepDistance;
        }

        transform.position = newPosition;

        PathResult missResult = Session.CheckForMiss(newPosition);
        if (missResult.WasJudged)
        {
            LastMissResult = missResult;
        }
    }
}