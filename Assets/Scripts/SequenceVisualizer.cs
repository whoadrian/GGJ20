using System;
using UnityEngine;

public class SequenceVisualizer : MonoBehaviour
{
    public Sequence sequence;

    [Header("UI References")] 
    public RectTransform sequenceVisualizationPanelContent;
    public RectTransform playHead;
    private Vector2 center;

    private void Update()
    {
        var endTime = sequence.StartTime + sequence.Data.duration;
        var timeLeft = endTime - sequence.CurrentTime;
        var percentage = 1 - (timeLeft / sequence.Data.duration);

        var rect = sequenceVisualizationPanelContent.rect;
        var halfOfWidth = rect.width/2;
        center = rect.center;
        
        playHead.anchoredPosition = Vector2.Lerp(
            center + Vector2.left * halfOfWidth,
            center + Vector2.right * halfOfWidth,
            percentage);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(sequenceVisualizationPanelContent.anchoredPosition, .2f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(sequenceVisualizationPanelContent.anchoredPosition +
                          sequenceVisualizationPanelContent.sizeDelta, .3f);
        
    }
}
