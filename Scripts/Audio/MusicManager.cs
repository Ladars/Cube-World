using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme;
    public AudioClip menuTheme;

    string sceneName;
    private void Start()
    {
        AudioManager.instance.PlayerMusic(menuTheme, 2);
        OnLevelWasLoaded(0);
    }
    void OnLevelWasLoaded(int sceneIndex)
    {
        string newSceneName = SceneManager.GetActiveScene().name;
        if(newSceneName != sceneName)
        {
            sceneName = newSceneName;
            Invoke("PlayMusic", .2f);
        }
    }
    void PlayMusic()
    {
        AudioClip clipToPlay = null;
        if (sceneName == "Menu")
        {
            clipToPlay = menuTheme;
        }else if(sceneName == "GameLevel")
        {
            clipToPlay = mainTheme;
        }
        if(clipToPlay != null)
        {
            AudioManager.instance.PlayerMusic(clipToPlay, 2);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            AudioManager.instance.PlayerMusic(mainTheme, 3);
        }
    }
}
