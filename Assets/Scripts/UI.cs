using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {
    private GameSystem system;
    public Slider tempoSlider;
    public Slider durationSlider;

    public void Start() {
        tempoSlider?.SetValueWithoutNotify(Mathf.InverseLerp(GameSystem.Instance.MinBallSpeed, GameSystem.Instance.MaxBallSpeed, GameSystem.Instance.BallSpeed));
        durationSlider?.SetValueWithoutNotify(Mathf.InverseLerp(GameSystem.Instance.MinDuration, GameSystem.Instance.MaxDuration, GameSystem.Instance.Sequence.Data.duration));
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
        durationSlider?.SetValueWithoutNotify(Mathf.InverseLerp(GameSystem.Instance.MinDuration, GameSystem.Instance.MaxDuration, GameSystem.Instance.Sequence.Data.duration));
    }

    public void Update() {
        if (GameSystem.Instance.Sequence.State != SequenceState.STOPPED) {
            durationSlider?.SetValueWithoutNotify(Mathf.InverseLerp(GameSystem.Instance.MinDuration, GameSystem.Instance.MaxDuration, GameSystem.Instance.Sequence.Data.duration - (GameSystem.Instance.Sequence.CurrentTime - GameSystem.Instance.Sequence.StartTime)));
        }
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

    public void OnDurationChange(float value) {
        if (GameSystem.Instance.Sequence.State == SequenceState.STOPPED) {
            GameSystem.Instance.Sequence.Data.duration = Mathf.Lerp(GameSystem.Instance.MinDuration, GameSystem.Instance.MaxDuration, value);
        }
    }
}