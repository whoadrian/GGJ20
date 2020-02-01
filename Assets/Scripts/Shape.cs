using System;
using System.Collections;
using System.Collections.Generic;
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
    }

    void Update() {
        Data.Position = transform.position;
    }

    public void TriggerShape() {
        for (int i = 0; i < (int)Data.Type; i++) {
            if (Data.OutgoingIds[i] == Guid.Empty.ToString()) {
                continue;
            }
            
            var obj = Instantiate(GameSystem.Instance.BallPrefab);
            var newBall = obj.GetComponent<Ball>();
            newBall.data = new BallData(Data.Id, Data.OutgoingIds[i]);
            balls.Add(newBall);
        }

        // TODO: Play sound
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

        foreach (var b in balls) {
            if (b != null) {
                b.Command(c);
            } else {
                balls.Remove(b);
            }
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