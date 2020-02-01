using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ShapeToSprite {
    public ShapeType type;
    public Sprite sprite;
}

[Serializable]
public class ShapeToTrack {
    public ShapeType type;
    public Transform track;
}

public class SequenceVisualizer : MonoBehaviour
{
    [HideInInspector] public GameSystem system;
    [HideInInspector] public Sequence sequence;

    [Header("Global Configuration")] 
    public List<ShapeToSprite> shapesToSprites;
    private Dictionary<ShapeType, Sprite> shapesToSpritesDict;
    public List<ShapeToTrack> shapesToTracks;
    private Dictionary<ShapeType, Transform> shapesToTracksDict;
    
    [Header("Prefabs")] 
    public GameObject noteVisualPrefab;

    [Header("UI References")] 
    public RectTransform sequenceVisualizationPanelContent;
    public RectTransform playHead;
    private Vector2 center;

    private void Start() {
        // create the dictionaries to index into later
        shapesToSpritesDict = new Dictionary<ShapeType, Sprite>();
        foreach (var shapeToSprite in shapesToSprites)
            shapesToSpritesDict.Add(shapeToSprite.type, shapeToSprite.sprite);
        
        shapesToTracksDict = new Dictionary<ShapeType, Transform>();
        foreach (var shapeToTrack in shapesToTracks)
            shapesToTracksDict.Add(shapeToTrack.type, shapeToTrack.track);
    }

    private void Update() {
        UpdateBeatPlacements();
        SetPlayHeadPosition();
    }

    private void UpdateBeatPlacements()
    {
        var rect = sequenceVisualizationPanelContent.rect;
        var width = rect.width;
        
        foreach (var outId in system.Sun.Data.OutgoingIds) {
            if (Shape.TryGetShape(outId, out var shape)) {
                var distance = DistanceBetween(system.Sun.transform, shape.transform);
                var speed = system.BallSpeed;
                var note = PrefabUtility.InstantiatePrefab(noteVisualPrefab) as GameObject;
                note.GetComponent<Image>().sprite = shapesToSpritesDict[shape.Data.Type];
                note.transform.parent = shapesToTracks[(int)shape.Data.Type].track;
                //note.GetComponent<RectTransform>().position = width;
            }
        }
        
        float DistanceBetween(Transform a, Transform b) => (a.position - b.position).magnitude; 
    }

    private void SetPlayHeadPosition()
    {
        var endTime = sequence.StartTime + sequence.Data.duration;
        var timeLeft = endTime - sequence.CurrentTime;
        var percentage = 1 - (timeLeft / sequence.Data.duration);

        var rect = sequenceVisualizationPanelContent.rect;
        var halfOfWidth = rect.width / 2;
        center = rect.center;

        playHead.anchoredPosition = Vector2.Lerp(
            center + Vector2.left * halfOfWidth,
            center + Vector2.right * halfOfWidth,
            percentage);
    }
}
