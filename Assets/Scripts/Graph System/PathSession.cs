using System.Collections.Generic;
using UnityEngine;

public class PathSession: MonoBehaviour
{
    private HitJudge judge;
    private PathPoint targetPoint;
    private Vector2 travelDirection;
    private bool isHolding;
    private List<string> heldButtons = new List<string>();
    private bool isFinished;
    public GameObject floatTextPrefab;

    private PathResult ignoredResult = new PathResult();
    public static event System.Action OnPathFinished;

    public PathPoint CurrentTarget
    {
        get
        {
            return targetPoint;
        }
    }

    public Vector2 TravelDirection
    {
        get
        {
            return travelDirection;
        }
    }

    public bool IsHolding
    {
        get
        {
            return isHolding;
        }
    }

    public bool IsFinished
    {
        get
        {
            return isFinished;
        }
    }

    public void Initialize(PathGraph graph, HitJudge judgeInput)
    {
        judge = judgeInput;
        targetPoint = graph.StartPoint;
        travelDirection = Vector2.right;

        if (targetPoint == null)
        {
            isFinished = true;
            return;
        }

        PathEdge firstEdge = GetDefaultEdge(targetPoint);
        if (firstEdge != null)
        {
            travelDirection = firstEdge.GetDirection();
        }
    }

    public PathResult SubmitTap(List<string> pressedButtons, Vector2 playerPosition){
        return SubmitPress(pressedButtons, playerPosition, false);
    }
    
    public PathResult SubmitHoldStart(List<string> pressedButtons, Vector2 playerPosition){
        return SubmitPress(pressedButtons, playerPosition, true);
    }
    
    // isHold is a variable only used in the SubmitPress function, it should not be used anywhere else. The variable you are looking for is called "isHolding" and is declared at the top of the script
    private PathResult SubmitPress(List<string> pressedButtons, Vector2 playerPosition, bool isHold)
    {
        if (targetPoint == null || pressedButtons == null || pressedButtons.Count == 0)
        {
            return ignoredResult;
        }

        if (IsHoldEndType(targetPoint.Type))
        {
            return ignoredResult;
        }

        if (IsHoldStartType(targetPoint.Type) != isHold)
        {
            return ignoredResult;
        }
        
        HitGrade grade = judge.Judge(playerPosition, targetPoint);
        
        if (grade == HitGrade.Miss)
        {
            return ignoredResult;
        }
        
        PathEdge chosenEdge = null;

        if (targetPoint.Type == PathPointType.Choice)
        {
            if (pressedButtons.Count == 1)
            {
                chosenEdge = targetPoint.GetEdgeForButton(pressedButtons[0]);
            }

            if (chosenEdge == null)
            {
                grade = HitGrade.Miss;
            }
        }
        else
        {
            bool countMatches = MatchesPressCount(pressedButtons, targetPoint.Type);
            if (countMatches == false)
            {
                grade = HitGrade.Miss;
            }
        }

        if (IsHoldStartType(targetPoint.Type) && grade != HitGrade.Miss)
        {
            isHolding = true;
            heldButtons = new List<string>(pressedButtons);
            Debug.Log("HOLD ON at " + targetPoint.UniquePointId);
        }

        ShowPopup(grade, playerPosition);
        return CompletePoint(grade, chosenEdge, playerPosition);
    }

    public PathResult SubmitRelease(List<string> releasedButtons, Vector2 playerPosition)
    {
        if (isHolding == false || targetPoint == null)
        {
            return ignoredResult;
        }

        if (IsHoldEndType(targetPoint.Type) == false)
        {
            return ignoredResult;
        }

        if (SameButtons(releasedButtons, heldButtons) == false)
        {
            return ignoredResult;
        }

        isHolding = false;
        
        HitGrade grade = judge.Judge(playerPosition, targetPoint);
        ShowPopup(grade, playerPosition);
        return CompletePoint(grade, null, playerPosition);
    }

    public PathResult CheckForMiss(Vector2 playerPosition)
    {
        if (targetPoint == null)
        {
            return ignoredResult;
        }

        bool passedWindow = judge.HasPassedWindow(playerPosition, targetPoint, travelDirection);
        if (passedWindow == false)
        {
            return ignoredResult;
        }

        if (isHolding)
        {
            Debug.Log("HOLD CLEARED by a miss at " + targetPoint.UniquePointId + " " + targetPoint.Type);
        }
        isHolding = false;
        ShowPopup(HitGrade.Miss, playerPosition);
        return CompletePoint(HitGrade.Miss, null, playerPosition);
    }

    private void ShowPopup(HitGrade grade, Vector2 position)
    {
        GameObject popupObject = Instantiate(floatTextPrefab, position, Quaternion.identity);
        FloatAndFade floatAndFade = popupObject.GetComponent<FloatAndFade>();

        Color textColor = Color.green;
        if (grade == HitGrade.Miss)
        {
            textColor = Color.red;
        }
        
        floatAndFade.SetText(grade.ToString(), textColor);

    }

    private PathResult CompletePoint(HitGrade grade, PathEdge chosenEdge, Vector2 playerPosition)
    {
        PathResult result = new PathResult();
        result.WasJudged = true;
        result.Grade = grade;
        result.JudgedPoint = targetPoint;
        NoiseGenerator generator = targetPoint.GetComponent<NoiseGenerator>();
        if (generator != null)
        {
            generator.ApplyGrade(grade);
        }
        result.SignedOffset = judge.GetSignedOffset(playerPosition, targetPoint, travelDirection);

        PathEdge nextEdge = chosenEdge;
        if (nextEdge == null)
        {
            nextEdge = GetDefaultEdge(targetPoint);
        }
        result.NextEdge = nextEdge;

        if (nextEdge == null)
        {
            targetPoint = null;
            isFinished = true;
        }
        else
        {
            targetPoint = nextEdge.Destination;
            travelDirection = nextEdge.GetDirection();
        }

        result.NextTarget = targetPoint;
        result.PathFinished = isFinished;
        
        if (isFinished && OnPathFinished != null)
        {
            OnPathFinished.Invoke();
        }
        
        return result;
    }

    private PathEdge GetDefaultEdge(PathPoint point)
    {
        if (point.Edges.Count == 0)
        {
            return null;
        }
        return point.Edges[0];
    }

    private bool MatchesPressCount(List<string> pressedButtons, PathPointType pointType)
    {
        int requiredCount = 1;
        if (pointType == PathPointType.Chord || pointType == PathPointType.ChordHoldStart)
        {
            requiredCount = 2;
        }

        return pressedButtons.Count == requiredCount;
    }

    private bool IsHoldStartType(PathPointType type)
    {
        return type == PathPointType.HoldStart || type == PathPointType.ChordHoldStart;
    }

    private bool IsHoldEndType(PathPointType type)
    {
        return type == PathPointType.HoldEnd || type == PathPointType.ChordHoldEnd;
    }

    private bool SameButtons(List<string> a, List<string> b)
    {
        if (a == null || b == null || a.Count != b.Count)
        {
            return false;
        }

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i])
            {
                return false;
            }
        }
        return true;
    }
}