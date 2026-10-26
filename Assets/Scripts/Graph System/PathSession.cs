using System.Collections.Generic;
using UnityEngine;

public class PathSession: MonoBehaviour
{
    private HitJudge judge;
    private PathPoint targetPoint;
    private Vector2 travelDirection;
    private bool isHolding;
    private string heldButton;
    private bool isFinished;
    public GameObject floatTextPrefab;

    // Shared result returned when an input did not apply to any point. Do not modify it.
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

        if (targetPoint.Type == PathPointType.HoldEnd)
        {
            return ignoredResult;
        }

        HitGrade grade = judge.Judge(playerPosition, targetPoint);
        
        if (grade == HitGrade.Miss)
        {
            // Outside the window, so this press does not belong to the target point
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

        if (targetPoint.Type == PathPointType.HoldStart && grade != HitGrade.Miss)
        {
            isHolding = true;
            heldButton = pressedButtons[0];
        }

        return CompletePoint(grade, chosenEdge, playerPosition);
    }

    public PathResult SubmitRelease(string releasedButton, Vector2 playerPosition)
    {
        if (isHolding == false || targetPoint == null)
        {
            return ignoredResult;
        }

        if (targetPoint.Type != PathPointType.HoldEnd)
        {
            return ignoredResult;
        }

        if (releasedButton != heldButton)
        {
            return ignoredResult;
        }

        isHolding = false;

        // A release outside the window is early, because late ones are caught by CheckForMiss
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
        if (pointType == PathPointType.Chord)
        {
            requiredCount = 2;
        }

        return pressedButtons.Count == requiredCount;
    }
}
