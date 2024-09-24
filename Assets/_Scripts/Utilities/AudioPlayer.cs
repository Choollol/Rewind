using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    private static Dictionary<string, AudioSource> sounds;

    private void Awake()
    {
        if (sounds == null)
        {
            sounds = new Dictionary<string, AudioSource>();
            GameObject soundsHolder = GameObject.Find("Sounds Holder");
            for (int i = 0; i < soundsHolder.transform.GetChild(0).childCount; i++)
            {
                sounds.Add(soundsHolder.transform.GetChild(0).GetChild(i).name, soundsHolder.transform.GetChild(0).GetChild(i).GetComponent<AudioSource>());
            }
            for (int i = 0; i < soundsHolder.transform.GetChild(1).childCount; i++)
            {
                sounds.Add(soundsHolder.transform.GetChild(1).GetChild(i).name, soundsHolder.transform.GetChild(1).GetChild(i).GetComponent<AudioSource>());
            }
        }
    }
    public static void ButtonClick()
    {
        sounds["Button Click"].Play();
    }
    public static void PlaySound(string key)
    {
        sounds[key].Play();
    }
    public static void PlaySound(string key, float minPitch, float maxPitch, bool doRoundSemitone = false)
    {
        sounds[key].pitch = Random.Range(minPitch, maxPitch);
        if (doRoundSemitone)
        {
            sounds[key].pitch = Mathf.Pow(1.05946f, (int)Mathf.Log(sounds[key].pitch, 1.05946f));
        }
        sounds[key].Play();
    }
    public static void StopSound(string key)
    {
        sounds[key].Stop();
    }
    public static AudioSource GetSound(string key)
    {
        return sounds[key];
    }
    public static bool IsSoundPlaying(string key)
    {
        return sounds[key].isPlaying;
    }
    public static IEnumerator FadeAudio(string key, float duration, float targetVolume)
    {
        if (sounds[key].volume == targetVolume) { yield break; }
        float currentTime = 0;
        float start = sounds[key].volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            sounds[key].volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        if (sounds[key].volume == 0)
        {
            sounds[key].Stop();
        }
        yield break;
    }
}
