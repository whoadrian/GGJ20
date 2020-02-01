using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ConnectionManager : MonoBehaviour {
    public enum State {
        IDLE,
        DRAGGING
    }

    private Shape shapeA;
    private Shape shapeB;

    private State state;
    
    public Action OnConnectionCreated;

    public void Update() {
        // Stop editing while playing
        if (GameSystem.Instance && GameSystem.Instance.Sequence.State != SequenceState.STOPPED) {
            return;
        }

        var ray = GameSystem.Instance.Cam.ScreenPointToRay(Input.mousePosition);

        switch (state) {
            case State.IDLE:

                if (Input.GetMouseButtonDown(1)) {
                    if (Physics.Raycast(ray, out var hitInfo, float.MaxValue)) {
                        shapeA = hitInfo.transform.GetComponent<Shape>();
                        if (shapeA == null) {
                            break;
                        }

                        state = State.DRAGGING;
                    }
                }

                break;
            case State.DRAGGING:
                if (Input.GetMouseButtonUp(1)) {
                    state = State.IDLE;

                    if (Physics.Raycast(ray, out var hitInfo, float.MaxValue)) {
                        shapeB = hitInfo.transform.GetComponent<Shape>();
                        var sun = hitInfo.transform.GetComponent<Sun>();
                        if (shapeB == null) {
                            if (sun == null) {
                                break;
                            }

                            if (GameSystem.Instance.Sun.Data.RemoveOutgoingId(shapeA.Data.Id)) {
                                break;
                            }

                            GameSystem.Instance.Sun.Data.AddOutgoingId(shapeA.Data.Id);
#if UNITY_EDITOR
                            EditorUtility.SetDirty(shapeA);
#endif            
                            OnConnectionCreated?.Invoke();
                            LevelAuthor.ValidateConnections();
                            break;
                        }

                        if (shapeA.Data.IncomingId == shapeB.Data.Id) {
                            break;
                        }

                        if (shapeB.Data.IncomingId != Guid.Empty.ToString()) {
                            if (shapeB.Data.IncomingId == shapeA.Data.Id) {
                                shapeB.Data.IncomingId = Guid.Empty.ToString();

                                for (int i = 0; i < (int)shapeA.Data.Type; i++) {
                                    if (shapeA.Data.OutgoingIds[i] == shapeB.Data.Id) {
                                        shapeA.Data.OutgoingIds[i] = Guid.Empty.ToString();
                                    }
                                }
                            }

#if UNITY_EDITOR
                            EditorUtility.SetDirty(shapeB);
                            EditorUtility.SetDirty(shapeA);
#endif
                        } else {
                            int shapeAConnectionIndex = -1;
                            for (int i = 0; i < (int)shapeA.Data.Type; i++) {
                                if (shapeA.Data.OutgoingIds[i] == Guid.Empty.ToString()) {
                                    shapeAConnectionIndex = i;
                                }
                            }

                            if (shapeAConnectionIndex > -1) {
                                shapeB.Data.IncomingId = shapeA.Data.Id;
                                shapeA.Data.OutgoingIds[shapeAConnectionIndex] = shapeB.Data.Id;
#if UNITY_EDITOR
                                EditorUtility.SetDirty(shapeB);
                                EditorUtility.SetDirty(shapeA);
#endif
                                OnConnectionCreated?.Invoke();
                                LevelAuthor.ValidateConnections();
                            }
                        }
                    }
                }

                break;
        }
    }
}