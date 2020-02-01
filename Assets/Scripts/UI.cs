using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {
    private GameSystem system;
    public Slider tempoSlider;

    public void Start() {
        tempoSlider?.SetValueWithoutNotify(Mathf.InverseLerp(GameSystem.Instance.MinBallSpeed, GameSystem.Instance.MaxBallSpeed, GameSystem.Instance.BallSpeed));
    }

    public void Play() {
        if (!CheckSanity()) {
            return;
        }
        
        system.Command(Command.PLAY);
    }

    public void Pause() {
        if (!CheckSanity()) {
            return;
        }
        
        system.Command(Command.PAUSE);
    }

    public void Stop() {
        if (!CheckSanity()) {
            return;
        }
        
        system.Command(Command.STOP);
    }

    private bool CheckSanity() {
        system = GameSystem.Instance;
        return system != null;
    }
    
    public void SpawnCircle() {
        LevelAuthor.CreateShape(UnityEngine.Random.onUnitSphere * 4, ShapeType.CIRCLE);
    }

    public void SpawnTriangle() {
        LevelAuthor.CreateShape(UnityEngine.Random.onUnitSphere * 4, ShapeType.TRIANGLE);
    }

    public void SpawnSquare() {
        LevelAuthor.CreateShape(UnityEngine.Random.onUnitSphere * 4, ShapeType.SQUARE);
    }

    public void OnTempoChange(float value) {
        GameSystem.Instance.BallSpeed = Mathf.Lerp(GameSystem.Instance.MinBallSpeed, GameSystem.Instance.MaxBallSpeed, value);
    }
}