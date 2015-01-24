using UnityEngine;

[System.Serializable]
public class AudioClipContainer
{
    public AudioClip clip;

    [Range(0, 1)]
    public float volume = 1;

    [Range(0, 2)]
    public float minPitch = 1;

    [Range(0, 2)]
    public float maxPitch = 1;

    public AudioSource Play()
    {
        if (clip != null) return AudioManager.PlayClip(clip, volume, Random.Range(minPitch, maxPitch));
        return null;
    }
}