using UnityEngine;

//A singleton class that manages audio. It uses an object pool to play one shot SFX.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public ObjectPool sfxPool;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Plays a one shot sound effect at the specified position with the given volume and pitch. Returns the AudioSource used to play the sound, or null if no AudioSource is available.
    public AudioSource PlaySFXOneShot(AudioClip clip, float volume = 1f, float pitch = 1f, Vector3 position = default(Vector3))
    {
        AudioSource audioSource = sfxPool.PlaceNextObject(position, Quaternion.identity)?.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.Play();
        }
        return audioSource;
    }
}
