using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    
    public GameObject gameOverUI;
    public RectTransform newWaveBanner;
    public TMP_Text newWaveTitle;
    public TMP_Text newWaveEnemyCount;
    public TMP_Text scoreUI;
    public TMP_Text gameOverScoreUI;
    public TMP_Text bulletDisplay;
    public TMP_Text EnemyRemain;
    public UIDoTween uIDoTween;
    

    Spawner spawner;
    Player player;
    Gun gun;
    Robot robot;
    public RectTransform healthBar;
    public Slider reloadBar;

    public event EventHandler OnReloadBar;
    [Header("GunUI")]
    public Image[] gunUI;
    public TMP_Text[] cooldownTime;
    private int gunIndex;
    public Image Flash;

    private int newWaveNumber;
    public GameObject bossHPBar;
    public GameObject PauseMenu;
    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
      //  spawner.OnNewWave += OnNewWave;
        spawner.OnNewWave += OnEnemyRemain;
        robot = FindObjectOfType<Robot>();
      //  robot.onDeath += DeactiveBossHPBar;
    }
    private void Start()
    {
        uIDoTween = FindObjectOfType<UIDoTween>();
        gun = FindObjectOfType<Gun>();
        player = FindObjectOfType<Player>();
        player.onDeath += OnGameOver;
        //gun.onReload += ReloadBar;
        player.onGunChange += onGunUIChange;
        player.onFlashUse += onFlashCooldown;
    }
    private void Update()
    {
        if(player == null)
        {
            player.onGunChange -= onGunUIChange;
        }
        scoreUI.text = ScoreKeeper.score.ToString("D6");
        float healthPercent = 0;
        if (player != null)
        {
            healthPercent=player.health / player.startingHealth;        
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
        OnBulletDisplay();
        ReloadBar();
        if (!player.FlashCanUse)
        {
            FlashcooldownTime();
        }
        onFlashCooldown();
        cooldownTimeDisplay(gunIndex);
        OnEnemyRemain(newWaveNumber);
        PauseGame();
    }
    void ReloadBar()
    {       
        reloadBar.value -= Time.deltaTime * (1/gun.reloadTime*0.75f);  
        if(reloadBar.value <= 0 &&gun.isReloading==true)
        {
            reloadBar.value = 1;
        }
    }
    void OnEnemyRemain(int waveNumber)
    {
        newWaveNumber = waveNumber;
        EnemyRemain.text = spawner.enemiesRemainingAlive.ToString() + "/" + spawner.waves[waveNumber-1].enemyCount;
    }
    void OnBulletDisplay()//显示子弹数
    {
        if(gun != null)
        {
            bulletDisplay.text = gun.projectilesRemainingInMag.ToString() + "/" + gun.projectilesPerMag.ToString();
        }
        else
        {
            if(player != null)
            {
                gun = FindObjectOfType<Gun>();
                bulletDisplay.text = gun.projectilesRemainingInMag.ToString() + "/" + gun.projectilesPerMag.ToString();
            }         
        }
        
    }
    void OnGameOver()//游戏结束界面
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear,new Color(0,0,0,0.9f),1));
        gameOverScoreUI.text = scoreUI.text;
        scoreUI.gameObject.SetActive(false);
        healthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }
    void OnNewWave(int waveNumber)//横幅
    {
        string[] numbers = { "One", "Two", "Three", "Four", "Five" };
        newWaveTitle.text = "- Wave" + numbers[waveNumber - 1] + "-";
        string enemyCountString =spawner.waves[waveNumber-1].infinite?"Infinite": spawner.waves[waveNumber - 1].enemyCount+"";
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;
        StopCoroutine(AnimateNewWaveBanner());
        StartCoroutine(AnimateNewWaveBanner());
    }
    IEnumerator AnimateNewWaveBanner()//横幅平移动画
    {
        float delayTime = 2f;
        float speed = 1f;
        float animatePercent = 0;
        int dir=1;
        float endDelayTime = Time.time + 1 / speed+delayTime;
        while (animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed*dir;
            if(animatePercent >= 1)
            {
                animatePercent = 1;
                if (Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }
            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-250, 50, animatePercent);
            yield return null;
        }
    }
    IEnumerator Fade(Color from,Color to,float time)
    {
        float speed = 1 / time;
        float percent = 0;
        while(percent < 1)
        {
            percent += Time.deltaTime *speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }
    //UI Input
    public void startNewGame()
    {
        SceneManager.LoadScene("GameLevel");
        gameOverUI.SetActive(false);
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void Resume()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale != 0)
            {
                PauseMenu.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                PauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
         
        }
      
       
    }
    public void onGunUIChange(int index)
    {
       // gunIndex = index;
        for(int i = 0; i < gunUI.Length; i++)
        {
           
            if (i == index)
            {
                
                gunUI[i].color = new Color(255, 255, 255, 1f);
                uIDoTween.GunDoShakeEffect(i);
                Debug.Log("gun change");
            }
            else
            {
                gunIndex = i;
                gunUI[i].color = new Color(255, 255, 255, 0.2f);
            }
        }     
    }
    public void onFlashCooldown()
    {
        if (player.FlashCanUse)
        {
            Flash.color = new Color(255, 255, 255, 1f);           
        }
        else
        {
            Flash.color = new Color(255, 255, 255, 0.2f);
        }
    }
    private void cooldownTimeDisplay(int _index)
    {
        int second = (int)player.GuncooldownTimeCount;
        cooldownTime[_index].text = second.ToString();
        if (second <= 0)
        {
            cooldownTime[_index].color= new Color(255, 255, 255, 0);
        }
        else
        {
            cooldownTime[_index].color = new Color(255, 255, 255, 1);
        }
    }
    private void FlashcooldownTime()
    {
        int second = (int)player.FlashcooldownTimeCount;
        cooldownTime[2].text = second.ToString();
        if (second <= 0)
        {
            cooldownTime[2].color = new Color(255, 255, 255, 0);
        }
        else
        {
            cooldownTime[2].color = new Color(255, 255, 255, 1);
        }
    }
    private void DeactiveBossHPBar()
    {
        bossHPBar.SetActive(false);
    }
    
   
}
