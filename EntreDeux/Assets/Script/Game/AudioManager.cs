using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class AudioManager : MonoBehaviour
{
    static public AudioManager instance;

    //static public AudioManager Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            instance = FindAnyObjectByType<AudioManager>();
    //        }
    //        if (instance == null)
    //        {
    //            GameObject obj = new GameObject(typeof(AudioManager).Name);
    //            instance = obj.AddComponent<AudioManager>();
    //            instance.soundtrackObject = new GameObject("Soundtrack");
    //            instance.soundtrackObject.transform.SetParent(obj.transform);
    //            instance.sfxObject = new GameObject("Sfx");
    //            instance.sfxObject.transform.SetParent(obj.transform);
    //        }

    //        return instance;
    //    }
    //}

    public float mainVolume = 1.0f;
    public float stVolume = 1.0f;
    public float sfxVolume = 1.0f;
    public float clockTimer = 1.0f;

    [SerializeField] GameObject soundtrackObject;
    [SerializeField] GameObject sfxObject;

    [SerializeField] AudioSource soundtrackAudioSource;
    [SerializeField] List<AudioSource> sfxAudioSources;
    [SerializeField] List<AudioClip> soundtrackClips;
    [SerializeField] Dictionary<string, AudioResource> soundEffects;

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (soundtrackObject)
        {
            soundtrackAudioSource = soundtrackObject.AddComponent<AudioSource>();
        }

        Scene scene = SceneManager.GetActiveScene();

        ChangeScene(scene, scene);

        foreach(KeyValuePair<string, AudioResource> ressource in soundEffects)
        {
            AudioSource source = sfxObject.AddComponent<AudioSource>();
            sfxAudioSources.Add(source);
            source.resource = ressource.Value;
            source.volume = 0.5f * mainVolume * sfxVolume;
        }

        SceneManager.activeSceneChanged += ChangeScene;
    }

    void ChangeScene(Scene current, Scene next)
    {
        int sceneIndex = next.buildIndex;

        if (soundtrackAudioSource != null)
        {
            StopAllSounds();

            soundtrackAudioSource.volume = 0.5f * mainVolume * stVolume;

            switch(sceneIndex)
            {
                case 0:
                case 4:
                    if (soundtrackClips[0])
                    {
                        soundtrackAudioSource.clip = soundtrackClips[0];
                    }
                    break;
                case 1:
                    if (soundtrackClips[1])
                    { 
                        soundtrackAudioSource.clip = soundtrackClips[1]; 
                    }
                    break;
                case 2:
                    if (soundtrackClips[2])
                    {
                        soundtrackAudioSource.clip = soundtrackClips[2];
                    }
                    break;
                case 3:
                case 5:
                    if (soundtrackClips[3])
                    {
                        soundtrackAudioSource.clip = soundtrackClips[3];
                    }
                    break;
                default:
                    break;
            }

            soundtrackAudioSource.loop = true;
            soundtrackAudioSource.Play();
        }
    }

    public void StopAllSounds()
    {
        if (soundtrackAudioSource != null)
        {
            soundtrackAudioSource.Stop();
        }
        foreach (AudioSource sources in sfxAudioSources)
        {
            sources.Stop();
        }
    }

    public void PlaySound(string sound)
    {
        if (!sfxObject)
        {
            return;
        }

        if (soundEffects.ContainsKey(sound))
        {
            //TODO If enough time use the PlayOneShot() or the static function PlayAsPoint or something (forgot the name)
            bool hasSound = false;

            foreach(AudioSource audio in sfxAudioSources)
            {
                if (audio.resource == soundEffects[sound])
                {
                    audio.volume = 0.5f * mainVolume * sfxVolume;
                    audio.Play();
                    hasSound = true;
                }
            }

            if (!hasSound)
            {
                AudioSource source = sfxObject.AddComponent<AudioSource>();
                sfxAudioSources.Add(source);
                source.resource = soundEffects[sound];
                source.volume = 0.5f * mainVolume * sfxVolume;
                source.Play();
            }
        }
    }

    public void SetMainVolume(float volume)
    {
        mainVolume = Math.Clamp(volume, 0.0f, 1.0f);

        UpdateVolume();
    }

    public void SetSoundtrackVolume(float volume)
    {
        stVolume = Math.Clamp(volume, 0.0f, 1.0f);

        UpdateVolume();
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = Math.Clamp(volume, 0.0f, 1.0f);

        UpdateVolume();
    }

    public void UpdateVolume()
    {
        soundtrackAudioSource.volume = 0.5f * mainVolume * stVolume;

        foreach (AudioSource audio in sfxAudioSources)
        {
            audio.volume = 0.5f * mainVolume * sfxVolume;
        }
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= ChangeScene;
    }

    // Update is called once per frame
    void Update()
    {
        //delta += Time.deltaTime;

        //if (delta >= clockTimer)
        //{
        //    delta = 0.0f;
        //}
        //else
        //{
        //    return;
        //}

        //if (sfxObject != null)
        //{
        //    for (int i = sfxAudioSources.Count - 1; i >= 0; i--)
        //    {
        //        if (!sfxAudioSources[i].isPlaying)
        //        {
        //            Destroy(sfxAudioSources[i]);
        //            sfxAudioSources.RemoveAt(i);
        //        }
        //    }
        //}
    }
}
