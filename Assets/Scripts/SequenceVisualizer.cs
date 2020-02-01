using System;
using System.Collections.Generic;
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
   private GameSystem system;
   private Sequence sequence;

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

    [Header("Pooling")] 
    public NotePool notePool;

    private List<Vector2> notePositions;
    private List<GameObject> notes = new List<GameObject>();

    private void Start() {
        system = GameSystem.Instance;
        sequence = system.Sequence;
        
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

    private void UpdateBeatPlacements() {
        foreach (var note in notes) 
            notePool.Return(note);
        notes.Clear();
        
        var rect = sequenceVisualizationPanelContent.rect;
        var width = rect.width;
        
        foreach (var outId in system.Sun.Data.OutgoingIds) {
            if (Shape.TryGetShape(outId, out var shape)) {
                CreateNote(shape, rect.center, width); 
                //  var distanceTravelledThisBranch = 0f;
                //  var distance = DistanceBetween(system.Sun.transform, shape.transform);
                //  var speed = system.BallSpeed;
                //  var velocity = distance / speed;
                //  distanceTravelledThisBranch += velocity;
                //  shape.TraverseTree();
                //TODO: the recursive loop in here
            }
        }
        
        float DistanceBetween(Transform a, Transform b) => (a.position - b.position).magnitude; 
    }

    private void CreateNote(Shape shape, Vector2 panelCenter, float width) {
        var note = notePool.Fetch(shape);
        note.GetComponent<Image>().sprite = shapesToSpritesDict[shape.Data.Type];
        note.transform.SetParent(shapesToTracksDict[shape.Data.Type]);
        notes.Add(note);
        //TODO: this isn't responsive to progress through the sequence
        note.GetComponent<RectTransform>().position = panelCenter + Vector2.left * (width / 2f);  
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
