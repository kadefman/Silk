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

    private void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.volume = s.volume;
            s.source.clip = s.clip;
            s.source.pitch = s.pitch;

        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }
        s.source.pitch = UnityEngine.Random.Range(.9f, 1.1f);
        s.source.Play();
        //Debug.Log("shooting");

    }
    public void PlaySpatial(string name, Vector3 position)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(s.clip, position, 1f) ;
        //Debug.Log("shooting");

    }

}
