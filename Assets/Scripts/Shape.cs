using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public struct ShapeData {
    [SerializeField]
    public string Id;
    public ShapeType Type;
    public Vector2 Position;
    public string IncomingId;
    public string[] OutgoingIds;

    public ShapeData(ShapeType type) {
        string empty = Guid.Empty.ToString();

        Id = Guid.NewGuid().ToString();
        Type = type;
        Position = Vector2.zero;
        IncomingId = empty;
        OutgoingIds = new[] {empty, empty, empty, empty, empty, empty};
    }
}

public enum ShapeType {
    CIRCLE = 1,
    TRIANGLE = 3,
    SQUARE = 4
}

[Serializable]
public struct ShapeAssets {
    [Serializable]
    public struct ShapePrefab {
        public ShapeType Type;
        public GameObject Prefab;
    }

    public ShapePrefab[] Prefabs;
}

[ExecuteInEditMode]
public class Shape : MonoBehaviour {
    [ReadOnly]
    public ShapeData Data;

    public static List<Shape> AllShapes;

    private List<Ball> balls = new List<Ball>();

    void Start() {
        if (AllShapes == null) {
            AllShapes = new List<Shape>();
        }

        AllShapes.Add(this);
    }

    private void OnDestroy() {
        AllShapes?.Remove(this);
        LevelAuthor.ValidateConnections();
    }

    void Update() {
        Data.Position = transform.position;
    }

    public void TriggerShape(string id = null) {

        bool outgoingDirection = true;
        if (!string.IsNullOrEmpty(id) && id != Guid.Empty.ToString()) {
            foreach (var outId in Data.OutgoingIds) {
                if (outId == id) {
                    outgoingDirection = false;
                    break;
                }
            }
        }

        bool hasOutgoing = false;
        foreach (var o in Data.OutgoingIds) {
            if (o != Guid.Empty.ToString()) {
                hasOutgoing = true;
                break;
            }
        }

        bool hasIncoming = Data.IncomingId != Guid.Empty.ToString();

        if (outgoingDirection) {
            if (hasOutgoing) {
                SpawnOutgoingBalls();
            } else if (hasIncoming) {
                SpawnBall(Data.Id, Data.IncomingId);
            }
        } else {
            if (hasIncoming) {
                SpawnBall(Data.Id, Data.IncomingId);
            } else {
                SpawnOutgoingBalls();
            }
        }

        // TODO: Play sound
    }

    private void SpawnOutgoingBalls() {
        for (int i = 0; i < (int)Data.Type; i++) {
            if (Data.OutgoingIds[i] == Guid.Empty.ToString()) {
                continue;
            }

            SpawnBall(Data.Id, Data.OutgoingIds[i]);
        }
    }

    private void SpawnBall(string shapeA, string shapeB) {
        var obj = Instantiate(GameSystem.Instance.BallPrefab);
        var newBall = obj.GetComponent<Ball>();
        newBall.data = new BallData(shapeA, shapeB);
        balls.Add(newBall);
    }

    public void Command(Command c) {
        switch (c) {
            case global::Command.NONE:
                break;
            case global::Command.PLAY:
                break;
            case global::Command.PAUSE:
                break;
            case global::Command.STOP:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(c), c, null);
        }

        for (int i = balls.Count - 1; i >= 0; i--) {
            var b = balls[i];
            if (b != null) {
                b.Command(c);
            } else {
                balls.Remove(b);
            }
        }

        foreach (var outId in Data.OutgoingIds) {
            if (outId == Guid.Empty.ToString()) {
                continue;
            }
            
            if (!TryGetShape(outId, out var outShape)) {
                Debug.LogError("Shape couldn't find outgoing connection ID " + outId);
            }
            
            outShape.Command(c);
        }
    }

    public static bool TryGetShape(string id, out Shape shape) {
        foreach (var s in AllShapes) {
            if (s.Data.Id == id) {
                shape = s;
                return true;
            }
        }

        shape = null;
        return false;
    }
}