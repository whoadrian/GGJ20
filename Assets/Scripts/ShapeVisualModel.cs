using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ShapeVisualModel : MonoBehaviour {
    private Shape target;
    private ShapeRenderer manager;

    private Coroutine killRoutine;
    private Coroutine initRoutine;
    private Coroutine trigRoutine;
    
    public void Init(Shape shape, ShapeRenderer renderer) {
        target = shape;
        this.manager = renderer;
        if (killRoutine != null) {
            StopCoroutine(killRoutine);
        }
        initRoutine = StartCoroutine(InitRoutine());
        shape.OnTriggered += OnTriggered;
    }

    public void LateUpdate() {
        if (target != null) {
            transform.position = target.transform.position;
        }
    }

    public void Kill() {
        if (initRoutine != null) {
            StopCoroutine(initRoutine);
        }
        
        if (trigRoutine != null) {
            StopCoroutine(trigRoutine);
        }

        SpawnFX(manager.KillFX);
        
        killRoutine = StartCoroutine(KillRoutine());
    }

    private void SpawnFX(GameObject prefab) {
        if (prefab == null) {
            return;
        }
        
        var obj = GameObject.Instantiate(prefab);
        obj.transform.position = transform.position;
    }

    public void OnTriggered() {
        trigRoutine = StartCoroutine(TriggeredRoutine());
        SpawnFX(manager.TriggeredFX);
    }

    public IEnumerator TriggeredRoutine() {
        float t = 0;
        Vector3 initScale = transform.localScale;
        while (t < manager.TriggeredTime) {
            float l = t / manager.TriggeredTime;
            float scale = manager.TriggeredCurve.Evaluate(l);

            transform.localScale = initScale * scale;
            
            t += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator InitRoutine() {
        float t = 0;
        Vector3 initScale = transform.localScale;
        bool fx = false;
        while (t < manager.InitTime) {
            float l = t / manager.InitTime;
            float scale = manager.InitCurve.Evaluate(l);
            float rot = Mathf.Lerp(90, 0, l);

            if (l > 0.5f && fx == false) {
                fx = true;
                SpawnFX(manager.InitFX);
            }

            transform.localRotation = Quaternion.Euler(0, 0, rot);
            transform.localScale = initScale * scale;
            
            t += Time.deltaTime;
            yield return null;
        }
        
        transform.localRotation = quaternion.identity;
    }
    
    public IEnumerator KillRoutine() {
        float t = 0;
        Vector3 initScale = transform.localScale;
        while (t < manager.KillTime) {
            float l = t / manager.KillTime;
            float scale = manager.KillCurve.Evaluate(l);
            float rot = Mathf.Lerp(0, 90, l);

            transform.localRotation = Quaternion.Euler(0, 0, rot);
            transform.localScale = initScale * scale;
            
            t += Time.deltaTime;
            yield return null;
        }
        
        LevelAuthor.SafeDestroy(gameObject);
    }
}
