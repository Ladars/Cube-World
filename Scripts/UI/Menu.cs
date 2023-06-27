using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Menu : MonoBehaviour
{
    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;
    public Toggle[] resolutionToggles;
    public int[] screenWidths;
    public Slider[] volumeSlider;
    public Toggle fullScreenToggle;
    int activeScreenResIndex;

    private void Start()
    {
        activeScreenResIndex = PlayerPrefs.GetInt("screen res index");
      //  bool isFullscreen = (PlayerPrefs.GetInt("fullscreen") == 1) ?true:false;
        Screen.fullScreen = true;
        volumeSlider[0].value = AudioManager.instance.masterVolumePercent;
        volumeSlider[1].value = AudioManager.instance.musicVolumePercent;
        volumeSlider[2].value = AudioManager.instance.sfxVolumePercent;
        for(int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].isOn = i == activeScreenResIndex;
        }
        //fullScreenToggle.isOn = isFullscreen;
    }
    public void Play()
    {
        SceneManager.LoadScene("GameLevel");
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void OptionMenu()
    {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }
    public void MainMenu()
    {
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
    }
    public void SetScreenResolution(int i)
    {
        if (resolutionToggles[i].isOn)
        {
            activeScreenResIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidths[i],(int)(screenWidths[i]/aspectRatio),false);
            PlayerPrefs.SetInt("screen res index",activeScreenResIndex);
            PlayerPrefs.Save();
        }
    }
    public void SetFullscreen(bool isFullscreen)
    {
        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].interactable = isFullscreen;
        }
        if (isFullscreen)
        {
            Resolution[] allResolution = Screen.resolutions;
            Resolution maxResolution = allResolution[allResolution.Length - 1];
            Screen.SetResolution(1920, 1080, true);
        }
        else
        {
            SetScreenResolution(activeScreenResIndex);
        }
        PlayerPrefs.SetInt("fullscreen", (isFullscreen) ? 1 : 0);
        PlayerPrefs.Save();
    }
    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }
    public void SetMusicVolume(float value)//更改背景音大小
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }
    public void SetSfxVolume(float value)//更改音效大小
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }
    private void Update()
    {
        Screen.fullScreen = true;
    }
}
