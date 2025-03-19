using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource bgmSource;
    private AudioSource sfxSource;
    
    public AudioClip backgroundMusic;
    public AudioClip rainClip;
    public Dictionary<string, AudioClip> soundEffects = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // AudioSource 2개 추가 (BGM & SFX)
            bgmSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();

            bgmSource.loop = true; // 배경음 루프 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        AudioManager.Instance.AddSoundEffect("RainSound", rainClip);
    }

    // 효과음 추가 메서드
    public void AddSoundEffect(string key, AudioClip clip)
    {
        if (!soundEffects.ContainsKey(key))
        {
            soundEffects[key] = clip;
        }
    }

    // 효과음 재생
    public void PlaySFX(string key, float volume = 1.0f)
    {
        if (soundEffects.TryGetValue(key, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    // 배경음악 재생
    public void PlayBGM(AudioClip bgm, float volume = 1.0f)
    {
        if (bgmSource.clip == bgm) return; // 동일한 BGM이면 재생 X

        bgmSource.clip = bgm;
        bgmSource.volume = volume;
        bgmSource.Play();
    }

    // 배경음 페이드인/페이드아웃 기능
    public IEnumerator FadeOutBGM(float duration)
    {
        float startVolume = bgmSource.volume;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }
        bgmSource.Stop();
    }

    public IEnumerator FadeInBGM(AudioClip bgm, float duration, float targetVolume = 1.0f)
    {
        PlayBGM(bgm, 0);
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, targetVolume, t / duration);
            yield return null;
        }
    }

    // 볼륨 조절
    public void SetSFXVolume(float volume) => sfxSource.volume = volume;
    public void SetBGMVolume(float volume) => bgmSource.volume = volume;
}
