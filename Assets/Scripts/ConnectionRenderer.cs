using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ConnectionRenderer : MonoBehaviour {
    [Header("General Settings")]
    public int LineResolution = 10;

    [Header("Assets")]
    public GameObject ShapeConnectionPrefab;
    public GameObject SunConnectionPrefab;

    private Dictionary<(string, string), LineRenderer> shapeConnections = new Dictionary<(string, string), LineRenderer>();
    private List<(string, string)> shapeConnectionsRemoval = new List<(string, string)>();

    private Dictionary<string, LineRenderer> sunConnections = new Dictionary<string, LineRenderer>();
    private List<string> sunConnectionRemoval = new List<string>();

    private LineRenderer CreateSunConnection(string id) {
        var line = CreateConnectionObject(SunConnectionPrefab);
        sunConnections.Add(id, line);
        return line;
    }
    
    private LineRenderer CreateShapeConnection(string idA, string idB) {
        var line = CreateConnectionObject(ShapeConnectionPrefab);
        shapeConnections.Add((idA, idB), line);
        return line;
    }

    private LineRenderer CreateConnectionObject(GameObject prefab) {
#if UNITY_EDITOR
        var obj = PrefabUtility.InstantiatePrefab(prefab, transform) as GameObject;
#else
        var obj = GameObject.Instantiate(prefab, transform);
#endif

        var line = obj.GetComponent<LineRenderer>();
        line.positionCount = LineResolution;
        line.useWorldSpace = true;

        return line;
    }

    public void Update() {
        List<Shape> allShapes;
        if (Application.isPlaying) {
            allShapes = Shape.AllShapes;
        } else {
            allShapes = GameObject.FindObjectsOfType<Shape>().ToList();
        }

        if (allShapes != null) {
            UpdateShapeConnections(allShapes);
            UpdateSunConnections(allShapes);
        }
    }

    private void UpdateSunConnections(List<Shape> allShapes) {
        Sun sun;
        if (Application.isPlaying) {
            sun = GameSystem.Instance.Sun;
        } else {
            sun = GameObject.FindObjectOfType<Sun>();
        }
        
        // Gather null connections
        foreach (var entry in sunConnections) {
            if (entry.Value == null) {
                sunConnectionRemoval.Add(entry.Key);
            }
        }

        // Remove null connections
        foreach (var entry in sunConnectionRemoval) {
            sunConnections.Remove(entry);
        }
        sunConnectionRemoval.Clear();
        
        // Create connections between shapes
        foreach (var id in sun.Data.OutgoingIds) {
            if (id == Guid.Empty.ToString()) {
                continue;
            }

            if (!sunConnections.ContainsKey(id)) {
                CreateSunConnection(id);
            }
        }
        
        // Update lines, gather connections between shapes that don't exist
        foreach (var entry in sunConnections) {
            if (Shape.TryGetShape(entry.Key, out var shape)) {

                bool found = false;
                for (int i = 0; i < GameSystem.Instance.Sun.Data.OutgoingIds.Length; i++) {
                    if (GameSystem.Instance.Sun.Data.OutgoingIds[i] == shape.Data.Id) {
                        found = true;
                        break;
                    }
                }

                if (!found) {
                    sunConnectionRemoval.Add(entry.Key);
                } else {
                    var pos = shape.Data.Position;

                    for (int i = 0; i < entry.Value.positionCount; i++) {
                        var l = (float)i / (entry.Value.positionCount - 1);
                    
                        entry.Value.SetPosition(i, Vector2.Lerp(Vector2.zero, pos, l));
                    }
                }
            } else {
                sunConnectionRemoval.Add(entry.Key);
            }
        }
        
        // Remove connections between shapes that don't exist
        foreach (var entry in sunConnectionRemoval) {
            LevelAuthor.SafeDestroy(sunConnections[entry].gameObject);
        }
        shapeConnectionsRemoval.Clear();
    }

    private void UpdateShapeConnections(List<Shape> allShapes) {
        // Gather null connections
        foreach (var entry in shapeConnections) {
            if (entry.Value == null) {
                shapeConnectionsRemoval.Add(entry.Key);
            }
        }

        // Remove null connections
        foreach (var entry in shapeConnectionsRemoval) {
            shapeConnections.Remove(entry);
        }
        shapeConnectionsRemoval.Clear();
        
        // Create connections between shapes
        foreach (var shapeA in allShapes) {
            var idA = shapeA.Data.Id;

            foreach (var idB in shapeA.Data.OutgoingIds) {
                if (idB == Guid.Empty.ToString()) {
                    continue;
                }

                if (!shapeConnections.ContainsKey((idA, idB)) && !shapeConnections.ContainsKey((idA, idB))) {
                    CreateShapeConnection(idA, idB);
                }
            }
        }
        
        // Update lines, gather connections between shapes that don't exist
        foreach (var entry in shapeConnections) {
            if (Shape.TryGetShape(entry.Key.Item1, out var shapeA) && Shape.TryGetShape(entry.Key.Item2, out var shapeB)) {
                var posA = shapeA.Data.Position;
                var posB = shapeB.Data.Position;

                if (shapeB.Data.IncomingId != shapeA.Data.Id && shapeA.Data.IncomingId != shapeB.Data.Id) {
                    shapeConnectionsRemoval.Add(entry.Key);
                } else {
                    for (int i = 0; i < entry.Value.positionCount; i++) {
                        var l = (float)i / (entry.Value.positionCount - 1);
                    
                        entry.Value.SetPosition(i, Vector2.Lerp(posA, posB, l));
                    }
                }
                
            } else {
                shapeConnectionsRemoval.Add(entry.Key);
            }
        }

        // Remove connections between shapes that don't exist
        foreach (var entry in shapeConnectionsRemoval) {
            LevelAuthor.SafeDestroy(shapeConnections[entry].gameObject);
        }
        shapeConnectionsRemoval.Clear();
    }
}