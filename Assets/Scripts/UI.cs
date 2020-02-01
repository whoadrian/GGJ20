using UnityEngine;

public class UI : MonoBehaviour {
    private GameSystem system;

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
}