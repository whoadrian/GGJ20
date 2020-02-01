using UnityEngine;

public class ConnectionAudio : MonoBehaviour
{
    public AudioConfig config;
    private ConnectionManager connectionManager;
    private AudioSource source;

    private void OnEnable() {
        connectionManager = GetComponent<ConnectionManager>();
        source = GetComponent<AudioSource>();
        connectionManager.OnConnectionCreated += PlayConnectionSound;
    }

    private void OnDisable() {
        connectionManager.OnConnectionCreated -= PlayConnectionSound;
    }

    private void PlayConnectionSound() {
        source.PlayOneShot(config.ConnectionSound);
    }
}
