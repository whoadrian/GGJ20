using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NotePool : MonoBehaviour
{
    public int initialPoolSize = 50;
    public int poolGrowSize = 25;

    private Dictionary<ShapeType, List<GameObject>> poolsByShape = new Dictionary<ShapeType, List<GameObject>>();
    public GameObject notePrefab;

    private void Start() => InitializePools();

    private void InitializePools() {
        InitializePool(ShapeType.CIRCLE);
        InitializePool(ShapeType.TRIANGLE);
        InitializePool(ShapeType.SQUARE);

        void InitializePool(ShapeType type) {
            poolsByShape.Add(type, new List<GameObject>());
            AddItemsToPool(type, initialPoolSize);
        }
    }

    private void AddItemsToPool(ShapeType shapeType, int itemsToAdd)
    {
        for (var i = 0; i < itemsToAdd; i++) {
            var instance = PrefabUtility.InstantiatePrefab(notePrefab) as GameObject;
            instance.gameObject.SetActive(false);
            poolsByShape[shapeType].Add(instance);
        }
    }

    public GameObject Fetch(Shape shape) {
        foreach (var poolItem in poolsByShape[shape.Data.Type])
            if (!poolItem.activeSelf)
                return poolItem;
        
        //if not returned at this point
        AddItemsToPool(shape.Data.Type, poolGrowSize);
        return Fetch(shape);
    }

    public void Return(GameObject note) => note.SetActive(false);
}
