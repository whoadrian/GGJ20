using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public struct BallData {
    public string ShapeA;
    public string ShapeB;

    public BallData(string shapeA, string shapeB) {
        this.ShapeA = shapeA;
        this.ShapeB = shapeB;
    }
}

public class Ball : MonoBehaviour {

    [ReadOnly, ShowInInspector]
    public BallData data;
    public bool Alive = false;

    private Coroutine routine;
    private bool paused = false;

    private Vector2 Position {
        get {
            return transform.position;
        }

        set { transform.position = value; }
    }

    public void Awake() {
        Alive = true;
    }

    public void Command(Command c) {
        switch (c) {
            case global::Command.NONE:
                break;
            case global::Command.PLAY:
                Alive = true;
                paused = false;
                if (routine == null) {
                    routine = StartCoroutine(Routine());
                }
                break;
            case global::Command.PAUSE:
                paused = true;
                break;
            case global::Command.STOP:
                if (routine != null) {
                    StopCoroutine(routine);
                }
                routine = null;
                Kill();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(c), c, null);
        }
    }

    public void Kill() {
        Alive = false;
        Debug.LogWarning("KILL BALL");
        if (routine != null) {
            StopCoroutine(routine);
        }
        routine = null;
        Destroy(gameObject);
    }

    private IEnumerator Routine() {
        if (!Shape.TryGetShape(data.ShapeA, out var a)) {
            Debug.LogError("Ball Shape A is null! ID : " + data.ShapeA);
            Kill();
            yield break;
        }

        if (!Shape.TryGetShape(data.ShapeB, out var b)) {
            Debug.LogError("Ball Shape B is null! ID : " + data.ShapeB);
            Kill();
            yield break;
        }

        var startPos = a.Data.Position;
        var endPos = b.Data.Position;

        Position = startPos;
        var distanceToTravel = (endPos - startPos).magnitude;
        var distanceTravelled = 0f;

        while (distanceTravelled <= distanceToTravel) {
            var percentageDistanceTravelled = distanceTravelled / distanceToTravel;
            Position = Vector2.Lerp(startPos, endPos, percentageDistanceTravelled);

            if (!paused) {
                distanceTravelled += GameSystem.Instance.BallSpeed * Time.deltaTime;
            }
            yield return null;
        }
        
        Kill();
        
        b.TriggerShape(a.Data.Id);
        b.Command(global::Command.PLAY);
    }
}