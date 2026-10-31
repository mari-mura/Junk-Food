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

    public PathResult SubmitPress(List<string> pressedButtons, Vector2 playerPosition)
    {
        if (targetPoint == null || pressedButtons == null || pressedButtons.Count == 0)
        {
            return ignoredResult;
        }

        if (IsHoldEndType(targetPoint.Type))
        {
            return ignoredResult;
        }

        HitGrade grade = judge.Judge(playerPosition, targetPoint);
        
        if (grade == HitGrade.Miss)
        {
            return ignoredResult;
        }

        GameObject popupObject = Instantiate(floatTextPrefab, playerPosition, Quaternion.identity);
        FloatAndFade floatAndFade = popupObject.GetComponent<FloatAndFade>();
        floatAndFade.SetText(grade.ToString(),Color.green);
        
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
        }

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
        GameObject popupObject = Instantiate(floatTextPrefab, playerPosition, Quaternion.identity);
        FloatAndFade floatAndFade = popupObject.GetComponent<FloatAndFade>();
        floatAndFade.SetText(grade.ToString(),Color.green);
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

        isHolding = false;
        GameObject popupObject = Instantiate(floatTextPrefab, playerPosition, Quaternion.identity);
        FloatAndFade floatAndFade = popupObject.GetComponent<FloatAndFade>();
        floatAndFade.SetText("Miss", Color.red);
        return CompletePoint(HitGrade.Miss, null, playerPosition);
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