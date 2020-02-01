using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ShapeRenderer : MonoBehaviour {
    [Serializable]
    public struct ShapeAssets {
        [Serializable]
        public struct ShapeAsset {
            public ShapeType type;
            public GameObject asset;
        }

        public ShapeAsset[] Assets;
    }

    public ShapeAssets assets;

    private GameObject visualModelParent;
    private Dictionary<string, ShapeVisualModel> Objects = new Dictionary<string, ShapeVisualModel>();
    private List<string> toRemove = new List<string>();

    public void Update() {
        var allShapes = Shape.AllShapes;

        if (allShapes == null) {
            return;
        }

        // Destroy all objects that don't have a shape anymore
        foreach (var entry in Objects) {
            var idA = entry.Key;

            bool found = false;
            foreach (var shape in allShapes) {
                if (shape.Data.Id == idA) {
                    found = true;
                    break;
                }
            }

            if (!found) {
                toRemove.Add(idA);
            }
        }

        foreach (var entry in toRemove) {
            LevelAuthor.SafeDestroy(Objects[entry].gameObject);
            Objects.Remove(entry);
        }

        toRemove.Clear();

        foreach (var shape in allShapes) {
            if (!Objects.ContainsKey(shape.Data.Id)) {
                var v = CreateShapeAsset(shape, shape.Data.Type);
                Objects.Add(shape.Data.Id, v);
            }
        }
    }

    private ShapeVisualModel CreateShapeAsset(Shape shape, ShapeType type) {
        if (visualModelParent == null) {
            visualModelParent = GameObject.Find("VISUAL_MODELS");
            if (visualModelParent == null)
                visualModelParent = new GameObject("VISUAL_MODELS");
        }

        foreach (var a in assets.Assets) {
            if (a.type == type) {
                var obj = GameObject.Instantiate(a.asset, visualModelParent.transform);
                var v = obj.GetComponent<ShapeVisualModel>();
                v.Init(shape);
                return v;
            }
        }

        return null;
    }
}