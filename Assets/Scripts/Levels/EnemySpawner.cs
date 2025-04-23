using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using System.Linq;


public class EnemySpawner : MonoBehaviour
{
    public Image level_selector;
    public GameObject button;
    public GameObject enemy;
    public SpawnPoint[] SpawnPoints;

    private LevelData easyLevelData;


    //private int currentSequenceIndex = 0;
    private int currentWave = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //EnemyManager.Instance.Awake();
        LoadLevelData();
        GameObject selector = Instantiate(button, level_selector.transform);
        selector.transform.localPosition = new Vector3(0, 130);
        selector.GetComponent<MenuSelectorController>().spawner = this;
        selector.GetComponent<MenuSelectorController>().SetLevel("Start");
    }

    void LoadLevelData()
    {
        // 1. loa dJSON
        TextAsset jsonFile = Resources.Load<TextAsset>("levels");
        if (jsonFile == null)
        {
            Debug.LogError("can not load levels.json ");
            return;
        }

        try
        {
            // 2. 
            JArray levelsArray = JArray.Parse(jsonFile.text);


            foreach (JObject levelObj in levelsArray)
            {
                if (levelObj["name"]?.ToString() == "Easy")
                {
                    List<SpawnConfig> spawnsList = new List<SpawnConfig>();
                    JArray spawnsArray = (JArray)levelObj["spawns"];

                    if (spawnsArray != null)
                    {
                        foreach (JObject spawnObj in spawnsArray)
                        {
                            SpawnConfig config = new SpawnConfig
                            {
                                enemy = spawnObj["enemy"]?.ToString(),
                                count = spawnObj["count"]?.ToString(),
                                hp = spawnObj["hp"]?.ToString(),
                                speed = spawnObj["speed"]?.ToString(),
                                damage = spawnObj["damage"]?.ToString(),
                                delay = spawnObj["delay"]?.ToString(),
                                location = spawnObj["location"]?.ToString()
                            };

                            JArray sequenceArray = (JArray)spawnObj["sequence"];
                            if (sequenceArray != null)
                            {
                                config.sequence = new int[sequenceArray.Count];
                                for (int i = 0; i < sequenceArray.Count; i++)
                                {
                                    config.sequence[i] = (int)sequenceArray[i];
                                }
                            }

                            spawnsList.Add(config);
                        }
                    }
                    
                    easyLevelData = new LevelData
                    {
                        name = levelObj["name"]?.ToString(),
                        waves = levelObj["waves"]?.ToObject<int>() ?? 0,
                        spawns = spawnsList
                    };

                    Debug.Log($"successful load {easyLevelData.spawns.Count} enemy");
                    //Debug.Log($" {easyLevelData.spawns[0].enemy} ");
                    return;
                }
            }
            
        }
        catch (System.Exception e)
        {
            Debug.LogError($"fail to load JSON: {e.Message}\n{e.StackTrace}");
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevel(string levelname)
    {

        
        level_selector.gameObject.SetActive(false);
        // this is not nice: we should not have to be required to tell the player directly that the level is starting
        GameManager.Instance.player.GetComponent<PlayerController>().StartLevel();

        currentWave = 0;


        StartCoroutine(SpawnWave(currentWave));
        //Debug.Log(currentWave);
    }

    public void NextWave()
    {
        //currentWave = currentWave + 1;
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            currentWave++;
            if (currentWave < easyLevelData.waves)
            {
                StartCoroutine(SpawnWave(currentWave));
            }
            else
            {
                Debug.Log("All waves complete!");
                // Optionally show final screen
            }
        }
    }


    IEnumerator SpawnWave(int waveIndex)
    {
        GameManager.Instance.state = GameManager.GameState.INWAVE;
        GameManager.Instance.countdown = 3;
        foreach (var config in easyLevelData.spawns)
        {
            if (config.sequence == null || waveIndex >= config.sequence.Length) continue;

            int waveNum = config.sequence[waveIndex];
            int count = RPNCalculator.CalculateEnemyCount(config.count, waveNum);
            count = Mathf.Max(1, count);

            Debug.Log($"Spawning {config.enemy} for wave {waveNum}, count={count}");

            for (int i = 0; i < count; i++)
            {
                if (config.enemy == "zombie") SpawnZombie(config, waveNum);
                else if (config.enemy == "skeleton") SpawnSkeleton(config, waveNum);

                yield return new WaitForSeconds(0.1f);
            }
        }

        // Wait until all enemies are cleared
        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);

        GameManager.Instance.state = GameManager.GameState.WAVEEND;
    }

    
    float GetSpawnDelay(SpawnConfig config)
    {
        //Debug.Log(config.delay);
        if (string.IsNullOrEmpty(config.delay)) return 10f; // 
        return float.Parse(config.delay);
    }



    IEnumerator SpawnBySequence(SpawnConfig config, string enemyType)
    {

        float delay = GetSpawnDelay(config);// GetSpawnDelay(config);

        int sequenceIndex = 0;

        while (sequenceIndex < config.sequence.Length)
        {

            int currentWaveValue = config.sequence[sequenceIndex];
            
            int count = RPNCalculator.CalculateEnemyCount(config.count, currentWaveValue);
            count = Mathf.Max(1, count); 

            Debug.Log($"generate {enemyType}: wave={currentWaveValue} num={count}");


            for (int i = 0; i < count; i++)
            {
                switch (enemyType)
                {
                    case "zombie":
                        SpawnZombie(config, currentWaveValue);
                        break;
                    case "skeleton":
                        SpawnSkeleton(config, currentWaveValue);
                        break;
                }
                yield return new WaitForSeconds(0.1f); 
            }

            sequenceIndex++;
            
            if (sequenceIndex < config.sequence.Length)
            {
                yield return new WaitForSeconds(delay);
            }
        }
    }

   


    /*
     * 
     *     IEnumerator SpawnWave()
    {
        GameManager.Instance.state = GameManager.GameState.COUNTDOWN;
        GameManager.Instance.countdown = 3;
        for (int i = 3; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            GameManager.Instance.countdown--;
        }
        GameManager.Instance.state = GameManager.GameState.INWAVE;
        for (int i = 0; i < 10; ++i)
        {
            yield return SpawnZombie();
        }
        yield return new WaitWhile(() => GameManager.Instance.enemy_count > 0);
        GameManager.Instance.state = GameManager.GameState.WAVEEND;
    }


    IEnumerator SpawnZombie()
    {
        SpawnPoint spawn_point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * 1.8f;
                
        Vector3 initial_position = spawn_point.transform.position + new Vector3(offset.x, offset.y, 0);
        GameObject new_enemy = Instantiate(enemy, initial_position, Quaternion.identity);

        new_enemy.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.enemySpriteManager.Get(0);
        EnemyController en = new_enemy.GetComponent<EnemyController>();
        en.hp = new Hittable(50, Hittable.Team.MONSTERS, new_enemy);
        en.speed = 10;
        GameManager.Instance.AddEnemy(new_enemy);
        yield return new WaitForSeconds(0.5f);
    }*/

    void SpawnZombie(SpawnConfig config, int wave)
    {
        SpawnEnemy(config, wave, 0); // sprite
    }

    void SpawnSkeleton(SpawnConfig config, int wave)
    {
        SpawnEnemy(config, wave, 1); 
    }

    void SpawnEnemy(SpawnConfig config, int wave, int spriteIndex)
    {
        
        EnemySprite enemyData = GameManager.Instance.enemySpriteManager.GetEnemyData(config.enemy);
        // 1. generate point
        SpawnPoint spawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
        Vector2 offset = Random.insideUnitCircle * 1.8f;
        Vector3 position = spawnPoint.transform.position + new Vector3(offset.x, offset.y, 0);
        
        GameObject enemyObj = Instantiate(enemy, position, Quaternion.identity);

        // 3. setting sprit
        enemyObj.GetComponent<SpriteRenderer>().sprite =
            GameManager.Instance.enemySpriteManager.Get(spriteIndex);

  
        var variables = new Dictionary<string, float>
        {
            { "base", 20 },
            { "wave", wave }
        };

        enemyData.hp = RPNCalculator.CalculateEnemyCount(config.hp, wave,enemyData.hp);


        Debug.Log(enemyData.name+":" + enemyData.hp +" | "+ enemyData.speed);

        EnemyController controller = enemyObj.GetComponent<EnemyController>();
        controller.hp = new Hittable(enemyData.hp, Hittable.Team.MONSTERS, enemy);
        controller.speed = enemyData.speed;

        GameManager.Instance.AddEnemy(enemyObj);
    }
   
}
