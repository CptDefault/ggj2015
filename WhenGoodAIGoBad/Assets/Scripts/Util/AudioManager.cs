using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    private readonly List<AudioSource> _soundClipPool = new List<AudioSource>();

    public AudioClipContainer AlarmSound;
    private AudioSource _alarm;
    private float _lastAlarm;

    public static AudioSource PlayClip(AudioClip clip, float volume = 1, float pitch = 1)
    {
        if (clip == null)
            return null;
        if (_instance == null)
            _instance = FindObjectOfType<AudioManager>();

        AudioSource audioSource = null;
        foreach (var source in _instance._soundClipPool)
        {
            if (!source.isPlaying)
            {
                audioSource = source;
                break;
            }
        }
        if (audioSource == null)
        {
            audioSource = _instance.gameObject.AddComponent<AudioSource>();
            _instance._soundClipPool.Add(audioSource);
        }

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.loop = false;
        audioSource.enabled = true;

        audioSource.Play();

        return audioSource;
    }

    protected void Awake()
    {
        _instance = this;
    }

    public static void PlayAlarm()
    {
        _instance.Alarm();
    }

    private void Alarm()
    {
        _lastAlarm = Time.time;
        if (_alarm == null)
        {
            _alarm = AlarmSound.Play();
            StartCoroutine(AlarmRoutine());
        }
    }

    private IEnumerator AlarmRoutine()
    {
        float vol = _alarm.volume;

        while (Time.time - _lastAlarm < 1f)
        {
            _alarm.volume = vol * Mathf.Clamp01(2 - (Time.time - _lastAlarm) * 4);
            yield return null;
        }
        _alarm.Stop();
        _alarm = null;
    }
}
