using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("기본 효과음 출력용 AudioSource")]
    [SerializeField] private AudioSource sfxSource;

    [Header("기본 버튼 클릭 효과음")]
    [SerializeField] private AudioClip buttonClickClip;

    [Header("전체 효과음 볼륨")]
    [Range(0f, 1f)]
    [SerializeField] private float masterVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (sfxSource == null)
                sfxSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 기본 버튼 클릭 효과음 재생
    /// </summary>
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickClip);
    }

    /// <summary>
    /// 원하는 클립 재생
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("재생할 AudioClip이 없습니다.");
            return;
        }

        if (sfxSource == null)
        {
            Debug.LogWarning("SFX AudioSource가 연결되지 않았습니다.");
            return;
        }

        sfxSource.PlayOneShot(clip, masterVolume);
    }

    /// <summary>
    /// 원하는 클립을 원하는 볼륨으로 재생
    /// </summary>
    public void PlaySFX(AudioClip clip, float volume)
    {
        if (clip == null)
        {
            Debug.LogWarning("재생할 AudioClip이 없습니다.");
            return;
        }

        if (sfxSource == null)
        {
            Debug.LogWarning("SFX AudioSource가 연결되지 않았습니다.");
            return;
        }

        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume) * masterVolume);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
    }
}