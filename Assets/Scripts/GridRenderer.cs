using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridRenderer : MonoBehaviour {
    [Header("Quads")]
    public Material quadMaterial;
    public Transform quadParent;
    public float Speed;
    public float minColorValue = 0.1f;
    public float maxColorValue = 0.3f;
    public float scale = 0.7f;

    private Renderer[] quadRenderers;
    private MaterialPropertyBlock _propBlock;

    [Button]
    public void ApplyQuadMaterial() {
        var renderers = quadParent.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers) {
            r.sharedMaterial = quadMaterial;
        }
    }

    // Start is called before the first frame update
    void Start() {
        _propBlock = new MaterialPropertyBlock();
        
        if (quadRenderers == null) {
            quadRenderers = quadParent.GetComponentsInChildren<Renderer>();
        }

        foreach (var q in quadRenderers) {
            q.transform.localScale = new Vector3(scale, scale, scale);
        }
        
        foreach (var r in quadRenderers) {
            Color color1 = new Color(Random.Range(minColorValue, maxColorValue), Random.Range(minColorValue, maxColorValue), Random.Range(minColorValue, maxColorValue), 1);
            Color color2 = new Color(Random.Range(minColorValue, maxColorValue), Random.Range(minColorValue, maxColorValue), Random.Range(minColorValue, maxColorValue), 1);
            StartCoroutine(QuadLoop(r, color1, color2));
        }
    }

    private IEnumerator QuadLoop(Renderer r, Color color1, Color color2) {
        while (true) {
            r.GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_Color", Color.Lerp(color1, color2, (Mathf.Sin(Time.time * Speed) + 1) / 2f));
            r.SetPropertyBlock(_propBlock);
            yield return null;
        }
    }
}