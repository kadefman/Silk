using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip EnemyDeath;
    //private AudioSource source;
    public Sound[] sounds;
    public static AudioManager instance;

    public AudioSource musicSource;
    public AudioClip forestMusic;
    public AudioClip bossMusic;

    public Animator musicAnim;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.volume = s.volume;
            s.source.clip = s.clip;
            s.source.pitch = s.pitch;

        }
    }

    /*private void Update()
    {
         if (Input.GetKeyDown(KeyCode.A))
        {
            PlayBoss();
        }

         if (Input.GetKeyDown(KeyCode.D))
        {
            PlayForest();
        }
    }*/

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }
        s.source.pitch = UnityEngine.Random.Range(.9f, 1.1f);
        if (name == "Currency")
        {
            s.source.volume = .35f;
            s.source.pitch = UnityEngine.Random.Range(1.1f, 1.2f);
            
        }
        if (name == "Big Currency")
        {
            s.source.volume = .35f;
            
        }

        s.source.Play();
        //Debug.Log("shooting");

    }
    public void PlaySpatial(string name, Vector3 position, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(s.clip, position, volume) ;
        //Debug.Log("shooting");

    }

    public void PlayBoss()
    {
        StartCoroutine(waitForBoss());
    }

    public void PlayForest()
    {
        StartCoroutine(waitForAnim());
    }

    IEnumerator waitForAnim()
    {
        musicAnim.SetTrigger("bossFade");
        yield return new WaitForSeconds(1);
        musicSource.Stop();
        musicSource.volume = .36f;
        musicSource.clip = forestMusic;
        yield return new WaitForSeconds(.02f);
        musicSource.Play();
    }

    IEnumerator waitForBoss()
    {
        musicAnim.SetTrigger("forestFade");
        yield return new WaitForSeconds(.9f);
        musicSource.Stop();
        
        musicSource.volume = .24f;
        musicSource.clip = bossMusic;
        yield return new WaitForSeconds(.1f);
        musicSource.Play();
    }
}
