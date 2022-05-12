using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class merchantAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] clips;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = clips[Random.Range(0, clips.Length)];
    }

    // Update is called once per frame
    public void playSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = clips[Random.Range(0, clips.Length)];
            audioSource.Play();
        }
    }
}
