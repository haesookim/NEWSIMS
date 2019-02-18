using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages every music/SFX
/// </summary>
public class AudioManager : MonoBehaviour
{

    public static AudioManager instance = null;
    public AudioSource musicSou;
    public AudioSource sfxSou;
    public AudioSource sfxSou2;
    public AudioClip[] SFX;
    public AudioClip[] music;
    private bool change = true;

    public float fadeSpeed = 1f;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject); 
    }

    private AudioSource nextSou()
    {
        change = !change;
        if (change)
            return sfxSou;
        else
            return sfxSou2;
    }

    public void StartSFX(string name)
    {
        AudioSource sou = nextSou();
        foreach (AudioClip clip in SFX)
            if (clip.name == name)
            {
                sou.clip = clip;
                sou.Play();
                return;
            }
    }

    public void StartMusic(string name)
    {
        /*if (musicSou.clip != null)
        {
            StartCoroutine(FadeOut(musicSou));
            musicSou.Stop();
        }*/

        foreach (AudioClip clip in music)
            if (clip.name == name)
            {
                Debug.Log("changed clip");
                musicSou.clip = clip;
                musicSou.Play();
                return;
            }
    }

    //NOT USED
    IEnumerator FadeOut(AudioSource sou)
    {
        float volume = 1;
        while(volume > 0.1)
        {
            sou.volume = volume;
            volume -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        Debug.Log("faded");
        sou.volume = 1;
    }
}
