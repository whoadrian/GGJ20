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

    private bool alive = false;
    public bool Alive => alive;

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
    
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    public void Command(Command c) {
        switch (c) {
            case global::Command.NONE:
                break;
            case global::Command.PLAY:
                Alive = true;
                paused = false;
                routine = StartCoroutine(Routine());
                break;
            case global::Command.PAUSE:
                paused = true;
                break;
            case global::Command.STOP:
                StopCoroutine(routine);
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
        alive = true;

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