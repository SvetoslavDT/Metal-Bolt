using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource menuMusicSource;
    public AudioSource normalGameMusic;
    public AudioSource alertGameMusic;

    public AudioSource btnClickSource;
    public AudioSource alertSource;
    public AudioSource playerShootSource;
    public AudioSource reloadSource;
    public AudioSource grenadeSource;
    public AudioSource doorPassSource;
    public AudioSource meleeAttackSource;
    public AudioSource dieSource;

    private Coroutine fadeMusicCoroutine;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void PlayMenuMusic()
    {
        if (normalGameMusic != null) normalGameMusic.Stop();
        if (alertGameMusic != null) alertGameMusic.Stop();

        if (menuMusicSource != null && !menuMusicSource.isPlaying)
        {
            menuMusicSource.Play();
            StartFade(menuMusicSource, 2f, 0.5f);
        }
    }

    public void PlayGameMusicNormal()
    {
        if (menuMusicSource != null) menuMusicSource.Stop();
        if (alertGameMusic != null) alertGameMusic.Stop();

        if (normalGameMusic != null && !normalGameMusic.isPlaying)
        {
            normalGameMusic.Play();
            StartFade(normalGameMusic, 1f, 0.4f);
        }
    }

    public void SwitchToAlertMusic()
    {
        if (!MainMenu.isMusicOn) return;

        if (normalGameMusic != null) normalGameMusic.Stop();
        if (alertGameMusic != null && !alertGameMusic.isPlaying)
        {
            alertGameMusic.Play();
            StartFade(alertGameMusic, 0.5f, 0.5f);
        }
    }

    public void SwitchToNormalMusic()
    {
        if (!MainMenu.isMusicOn) return;

        if (alertGameMusic != null) alertGameMusic.Stop();
        if (normalGameMusic != null && !normalGameMusic.isPlaying)
        {
            normalGameMusic.Play();
            StartFade(normalGameMusic, 1.5f, 0.4f);
        }
    }

    private void StartFade(AudioSource source, float duration, float targetVolume)
    {
        if (!MainMenu.isMusicOn)
        {
            source.volume = 0f;
            return;
        }

        if (fadeMusicCoroutine != null) StopCoroutine(fadeMusicCoroutine);
        fadeMusicCoroutine = StartCoroutine(FadeMusic(source, duration, targetVolume));
    }

    IEnumerator FadeMusic(AudioSource source, float duration, float targetVolume)
    {
        float currentTime = 0;
        source.volume = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, currentTime / duration);
            yield return null;
        }
        source.volume = targetVolume;
    }
    public void UpdateMusicVolume()
    {
        if (!MainMenu.isMusicOn)
        {
            if (menuMusicSource != null) menuMusicSource.volume = 0f;
            if (normalGameMusic != null) normalGameMusic.volume = 0f;
            if (alertGameMusic != null) alertGameMusic.volume = 0f;
        }
        else
        {
            if (menuMusicSource != null && menuMusicSource.isPlaying) menuMusicSource.volume = 0.5f;
            if (normalGameMusic != null && normalGameMusic.isPlaying) normalGameMusic.volume = 0.4f;
            if (alertGameMusic != null && alertGameMusic.isPlaying) alertGameMusic.volume = 0.5f;
        }
    }

    public void PlayBtnClick() { if (MainMenu.isSFXOn && btnClickSource != null) btnClickSource.Play(); }
    public void PlayAlert() { if (MainMenu.isSFXOn && alertSource != null) alertSource.Play(); }
    public void PlayPlayerShoot() { if (MainMenu.isSFXOn && playerShootSource != null) playerShootSource.Play(); }
    public void PlayReload() { if (MainMenu.isSFXOn && reloadSource != null) reloadSource.Play(); }
    public void PlayGrenade() { if (MainMenu.isSFXOn && grenadeSource != null) grenadeSource.Play(); }
    public void PlayDoorPass() { if (MainMenu.isSFXOn && doorPassSource != null) doorPassSource.Play(); }
    public void PlayMeleeAttack() { if (MainMenu.isSFXOn && meleeAttackSource != null) meleeAttackSource.Play(); }
    public void PlayDie() { if (MainMenu.isSFXOn && dieSource != null) dieSource.Play(); }
}
