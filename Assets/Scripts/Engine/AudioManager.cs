using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Core")]
    public AudioSource musicAudioSource;
    public AudioSource feedbackAudioSource;

    [Header("Clips")]
    public AudioClip buttonClip;
    public AudioClip answerSelectClip;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void PlayMusic()
    {
        musicAudioSource.Play();
    }

    public void StopMusic()
    {
        musicAudioSource.Stop();
    }

    public void PlayClip(AudioClip clip)
    {
        feedbackAudioSource.PlayOneShot(clip);
    }

    #region SpecificFeedbacks
    public void PlayButtonClip()
    {
        PlayClip(buttonClip);
    }

    public void PlayAnswerSelectClip()
    {
        PlayClip(answerSelectClip);
    }

    public void PlayFameLostClip()
    {

    }

    public void PlayFameEarnedClip()
    {
            
    }

    public void PlayLikesIncrementClip()
    {

    }

    #endregion
}
