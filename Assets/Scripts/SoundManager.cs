using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Singleton
    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion
    AudioSource currentAudioSource;

    List<AudioSource> audioQueue;

    public bool isPlaying = false;
    public int queueSize = 0;

    public AudioSource nonMusicSoundsRef;

    public void PlayMusic(AudioSource source)
    {
        if (source.clip == null)
            return;

        isPlaying = true;
        if (currentAudioSource != null)
        {
            //Put in queue
            if (queueSize > 0)
            {
                if (audioQueue.Count < queueSize)
                    audioQueue.Add(source);
            }
            else
            {
                currentAudioSource.Stop();

                currentAudioSource = source;
                currentAudioSource.Play();
            }
        }
        else
        {
            currentAudioSource = source;
            currentAudioSource.Play();

            //if queue>0
            if (queueSize > 0)
                StartCoroutine(WaitForAudioSourceEnd());
            else
                StartCoroutine(WaitEndOfPlaying());
        }
    }

    public void PlayAudio(AudioClip clip)
    {
        nonMusicSoundsRef.clip = clip;
        nonMusicSoundsRef.Play();
    }
    public void InterruptAllAudios()
    {
        //make the queue empty
        audioQueue = new List<AudioSource>();

        if (currentAudioSource != null)
        {
            //stop current audio
            if (currentAudioSource.isPlaying)
            {
                StopAllCoroutines();
                currentAudioSource.Stop();
                currentAudioSource = null;
                isPlaying = false;
            }
        }
    }

        IEnumerator WaitForAudioSourceEnd()
        {
            //Check audio source end

            while (currentAudioSource.isPlaying)
            {
                Debug.Log("Audio Playing...");
                yield return null;
            }

            Debug.Log("Audio finished");

            currentAudioSource = null;

            //if the queue is not empty, we play the next audio and we remove the audio from queue
            if (audioQueue.Count > 0)
            {
                PlayMusic(audioQueue[0]);
                audioQueue.RemoveAt(0);
            }
            else
            {
                isPlaying = false;
            }

        }

        IEnumerator WaitEndOfPlaying()
        {
            while (currentAudioSource.isPlaying)
            {
                Debug.Log("Audio Playing...");
                yield return null;
            }
            Debug.Log("Audio finished");
            currentAudioSource = null;

            isPlaying = false;

        }
    }
