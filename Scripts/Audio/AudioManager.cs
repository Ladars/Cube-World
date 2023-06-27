using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Range(0,1)] public float masterVolumePercent;
    public float sfxVolumePercent { get; private set; }
    [Range(0, 1)] public float musicVolumePercent;

    AudioSource sfx2DSources;
    public enum AudioChannel { Master,Sfx,Music}
    AudioSource[] musicSources;
    int activeMusicSourceIndex;
    public static AudioManager instance;
    Transform audioListener;
    Transform playerTran;
    SoundLibrary library;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)//创建声源文件
            {
                GameObject newMusicSource = new GameObject("Music source" + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }
            GameObject new2DSfxSource = new GameObject("2D sfx source" );//创建2D声源文件
            sfx2DSources = new2DSfxSource.AddComponent<AudioSource>();
            new2DSfxSource.transform.parent = transform;

            audioListener = FindObjectOfType<AudioListener>().transform;
            if (FindObjectOfType<Player>() != null)
            {
                playerTran = FindObjectOfType<Player>().transform;
            }           
            library = FindObjectOfType<SoundLibrary>();
            masterVolumePercent=PlayerPrefs.GetFloat("master vol", 0.4f);
            sfxVolumePercent=PlayerPrefs.GetFloat("sfx vol", 1f);
            musicVolumePercent=PlayerPrefs.GetFloat("music vol", 2f);
        }     
    }
    private void Start()
    {
        for(int i = 0; i < musicSources.Length - 1; i++)
        {
            musicSources[i].loop = true;
        }
    }
    private void Update()
    {
        if(playerTran != null)
        {
            audioListener.position = playerTran.position; 
        }
    }
    public void SetVolume(float volumePercent,AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                break;
        }
        musicSources[0].volume = musicVolumePercent * masterVolumePercent;
        musicSources[1].volume = musicVolumePercent * masterVolumePercent;
        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);
        PlayerPrefs.Save();
    }
    public void PlaySound(AudioClip clip,Vector3 pos)//播放3D声音片段
    {
        if(clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }        
    }
    public void PlaySound(string soundName,Vector3 pos)
    {
        PlaySound(library.GetClipFromName(soundName), pos);
    }
    public void PlayerMusic(AudioClip clip,float fadeDuration=1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();
        StartCoroutine(AnimateMusicCrossFade(fadeDuration));
    }
    public void PlaySound2D(string soundName)//播放2D声音片段
    {
        sfx2DSources.PlayOneShot(library.GetClipFromName(soundName),sfxVolumePercent);
    }
    IEnumerator AnimateMusicCrossFade(float duration)//使声音渐变淡出
    {
        float percent = 0;
        while(percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1-activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent,0, percent);
            yield return null;
        }
    }
    void OnLevelWasLoaded(int index)
    {
        if (playerTran == null)
        {
            if (FindObjectOfType<Player>() != null)
            {
                playerTran = FindObjectOfType<Player>().transform;
            }
        }
    }
}
