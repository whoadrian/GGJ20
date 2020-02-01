using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace whoa.UX {
    [CreateAssetMenu(menuName = "WHOA/UX/Gizmos/Interactable Gizmo", fileName = "InteractableGizmoConfig")]
    public class InteractableGizmoConfig : ScriptableObject {
        [Space]
        public int resolution;
        public float minRadius;
        public float maxRadius;
        public float minWidth;
        public float maxWidth;
        [Space]
        public float enterSpeed;
        public float exitSpeed;
        [Space]
        public AnimationCurve curve;
        public Gradient gradient;
        
    }
}