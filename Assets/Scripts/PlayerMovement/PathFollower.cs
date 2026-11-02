using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public MapBuilder MapBuilder;
    public float PlayerSpeed = 5.0f;
    public PathResult LastMissResult;
    public PathSession Session;
    public PlayerLauncher Launcher;
    private bool isMoving = false;

    public bool IsMoving
    {
        get { return isMoving; }
    }

    void Start()
    {
        Launcher.Launch(StartMoving);
    }

    public void StartMoving(float speed)
    {
        PlayerSpeed = speed;
        isMoving = true;
    }

    public void Awake()
    {
        PathGraph graph = MapBuilder.Build();
        Debug.Log("Created PathGraph: " + graph.ToString());
        
        HitJudge hitJudge = new HitJudge(0.5f, 0.8f);

        Session.Initialize(graph, hitJudge);

        if (Session.CurrentTarget != null)
        {
            transform.position = Session.CurrentTarget.Position;
        }
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
        if (Session == null || Session.IsFinished || isMoving == false)
        {
            return;
        }
        PathPoint target = Session.CurrentTarget;

        if (target == null)
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