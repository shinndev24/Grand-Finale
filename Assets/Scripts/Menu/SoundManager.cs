using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource source;

    [Header("Clips")]
    public AudioClip click;
    public AudioClip choose; 
    public AudioClip curtain; 
    public AudioClip reveal;
    public AudioClip roundStart;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void Play(AudioClip clip, float volume = 1f)
    {
        if (Instance == null || clip == null) return;
        Instance.source.PlayOneShot(clip, volume);
    }

    public static void PlayChoose()    { if (Instance != null) Play(Instance.choose); }
    public static void PlayCurtain()   { if (Instance != null) Play(Instance.curtain); }
    public static void PlayReveal()    { if (Instance != null) Play(Instance.reveal); }
    public static void PlayRoundStart(){ if (Instance != null) Play(Instance.roundStart); }
}