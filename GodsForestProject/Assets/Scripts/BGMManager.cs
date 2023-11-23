using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BGMManager : MonoBehaviour
{
    [SerializeField]
    protected List<AudioClip> musicClips = new List<AudioClip>();

    private int currentIndex;

    private AudioSource bgmSource;

    public bool multipleTracks;

    public static BGMManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        bgmSource = GetComponent<AudioSource>();
        UpdateVolume();

        if (multipleTracks)
        {
            currentIndex = Random.Range(0, musicClips.Count);
            StartCoroutine(TrackIncrementer());
            bgmSource.Play();
        }
    }

    public void UpdateVolume()
    {
        try
        {
            bgmSource.volume = GameManager.instance.bgmVolume;
        }
        catch
        {

        }
    }

    private IEnumerator TrackIncrementer()
    {
        if(!bgmSource.isPlaying)
        {
            bgmSource.clip = musicClips[currentIndex];
            bgmSource.Play();
            currentIndex++;

            if(currentIndex >= musicClips.Count)
            {
                currentIndex = 0;
            }
        }
        yield return new WaitForSeconds(.25f);
        StartCoroutine(TrackIncrementer());
    }



}
