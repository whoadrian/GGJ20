using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeVisualModel : MonoBehaviour {
    private Shape target;
    
    public void Init(Shape shape) {
        target = shape;
    }

    public void LateUpdate() {
        transform.position = target.transform.position;
    }
}
