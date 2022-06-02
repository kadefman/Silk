using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip straightAttack;
    public AudioClip sweepAttack;
    public AudioClip flyAttack;
    public AudioClip death;


    public void straightAttackSFX()
    {
        audioSource.clip = straightAttack;
        audioSource.Play();
        //Debug.Log("straight attack");
    }

    public void sweepAttackSFX()
    {
        audioSource.clip = sweepAttack;
        audioSource.Play();
        //Debug.Log("sweep attack");
    }

    public void flyAttackSFX()
    {
        audioSource.clip = flyAttack;
        audioSource.Play();
        //Debug.Log("fly attack");
    }

    public void deathSFX()
    {
        //Debug.Log("deathsfx");
        audioSource.clip = death;
        audioSource.Play();
    }
}
