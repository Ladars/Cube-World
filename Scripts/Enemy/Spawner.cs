using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool devMode;//测试关卡使用
    public Wave[] waves;
    public Enemy[] enemy;
    
    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave;
    int currentWaveNumber;

    public int enemiesRemainingToSpawn;
    public float nextSpawnTime;
    public int enemiesRemainingAlive;

    MapGenerator map;

    float timeBetweenCampingChecks=2f;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 camPositionOld;
    bool isCamping;
    bool isDisabled;

    public event System.Action<int> OnNewWave;
    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;
        playerEntity.onDeath += OnPlayerDeath; 
        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        camPositionOld = playerT.position;
        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }
    private void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;
                isCamping = (Vector3.Distance(playerT.position, camPositionOld) < campThresholdDistance);
                camPositionOld = playerT.position;
            }
            nextSpawnTime -= Time.deltaTime;
            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && nextSpawnTime <= 0)
            {
                enemiesRemainingToSpawn--;//逐个生成敌人 spawn enemy respectively
                nextSpawnTime = currentWave.timeBetweenSpawns;
                StartCoroutine(SpawnEnemy());
            }
        }
        if (devMode)//用于测试游戏关卡
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                StopCoroutine("SpawnEnemy");
                foreach(Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
    }
    IEnumerator SpawnEnemy()//生成敌人
    {
        float spawnDelay = 1f;
        float tileFlashSpeed = 4;
        Transform spawnTile = map.GetRandomOpenTile();
        if (isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<MeshRenderer>().material;
        Color initialColour = Color.white;
        Color flashColour = Color.red;
        float spawnTimer = 0.2f;
      
        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.red;
            tileMat.color = Color.Lerp(initialColour, flashColour, Mathf.PingPong(spawnTimer*tileFlashSpeed,1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        if (currentWave.waveType==Wave.WaveType.Zombie)
        {
            Enemy spawnedEnemy = Instantiate(enemy[Random.Range(0, 2)], spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
            spawnedEnemy.onDeath += OnEnemyDeath;
            spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth);
        }
        if (currentWave.waveType ==Wave.WaveType.Robot)
        {
            Enemy spawnedEnemy = Instantiate(enemy[2], spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
            spawnedEnemy.onDeath += OnEnemyDeath;
            spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth);
        }
        
        //spawnedEnemy.onDeath += OnEnemyDeath;
        //spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth);
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }
    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
        if(enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }
    private void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position+Vector3.up *3f;
    }
    void NextWave()
    {
        if (currentWaveNumber > 0)
        {
            AudioManager.instance.PlaySound2D("Level Complete");
        }
        currentWaveNumber ++;

        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }
        else
        {
            SceneManager.LoadScene("End");
        }
    }
    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public float moveSpeed;
        public Color skinColour;
        public enum WaveType { Zombie, Robot };
        public WaveType waveType;
    }
}
