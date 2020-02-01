using Unity.Mathematics;
using UnityEngine;

namespace whoa.UX {
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(SphereCollider))]
    [ExecuteInEditMode]
    public class InteractableGizmo : MonoBehaviour {
        private InteractableGizmoConfig config;
        private InteractableGizmoConfig Config {
            get {
                if (config == null) {
                    if (Application.isPlaying) {
                        config = GameSystem.Instance.GizmoConfig;
                    } else {
                        var system = GameObject.FindObjectOfType<GameSystem>();
                        config = system.GizmoConfig;
                    }
                }

                return config;
            }
        }
        
        private Camera Cam {
            get {
                if (Application.isPlaying) {
                    return GameSystem.Instance.Cam;
                } else {
                    var system = GameObject.FindObjectOfType<GameSystem>();
                    return system.Cam;
                }
            }
        }

        public bool Hovered => hovered;
        public bool Dragging => dragging;

        private Transform tr;
        private LineRenderer line;
        private new SphereCollider collider;
        private float3[] directions;
        private float hoverTransition;
        private bool hovered;
        private bool dragging;
        private float3 startDragDiff;
        private Plane dragPlane;

        private void Start() {
            tr = transform;
            
            collider = GetComponent<SphereCollider>();
            collider.radius = Config.maxRadius;
            
            line = GetComponent<LineRenderer>();
            line.loop = true;
            line.positionCount = Config.resolution;
            line.useWorldSpace = false;
            directions = new float3[Config.resolution];
            
            for (int i = 0; i < Config.resolution; i++) {
                float a = (float)i / Config.resolution * (math.PI * 2);
                float x = math.sin(a);
                float y = math.cos(a);
                float3 dir = math.float3(x, y, 0);
                
                line.SetPosition(i, float3.zero);
                directions[i] = dir;
            }
        }
        
        private void OnEnable() {
            line = GetComponent<LineRenderer>();
            line.enabled = true;
        }

        private void OnDisable() {
            line.enabled = false;
        }

        private void LateUpdate() {
            var ray = Cam.ScreenPointToRay(Input.mousePosition);
            hovered = dragging || collider.Raycast(ray, out var hitInfo, float.MaxValue);

            var dT = Time.deltaTime;
            var d = hovered ? 1f : 0f;
            var s = hovered ? Config.enterSpeed * dT : -Config.exitSpeed * dT;
            
            if (math.abs(hoverTransition - d) > 0.0001f) {
                hoverTransition = math.clamp(hoverTransition + s, 0, 1);
                
                var c = Config.curve.Evaluate(hoverTransition);
                var g = Config.gradient.Evaluate(hoverTransition);

                line.startWidth = line.endWidth = math.lerp(Config.minWidth, Config.maxWidth, c);
                line.startColor = line.endColor = g;
                
                for (int i = 0; i < line.positionCount; i++) {
                    line.SetPosition(i, directions[i] * math.lerp(Config.minRadius, Config.maxRadius, c));
                }
            }

            if (hovered) {
                bool click = Input.GetMouseButtonDown(0);
                bool hold = Input.GetMouseButton(0);

                if (!dragging) {
                    if (click) {
                        dragging = true;
                        dragPlane = new Plane(tr.forward, tr.position);
                        dragPlane.Raycast(ray, out float enter);
                        var point = ray.GetPoint(enter);
                        startDragDiff = tr.position - point;
                    }
                } else {
                    if (!hold) {
                        dragging = false;
                    } else {
                        if (dragPlane.Raycast(ray, out float enter)) {
                            tr.position = (float3)ray.GetPoint(enter) + startDragDiff;
                        }
                    }
                }
            }
        }
    }
}