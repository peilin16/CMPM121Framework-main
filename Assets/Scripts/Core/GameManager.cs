using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager 
{
    public enum GameState
    {
        PREGAME,
        INWAVE,
        WAVEEND,
        COUNTDOWN,
        GAMEOVER
    }
    public GameState state;
    public enum Difficultly
    {
        EASY,
        MEDIUM,
        HARD
    }
    public Difficultly level = Difficultly.MEDIUM;
    public int currentWave = 0;

    public int countdown;
    private static GameManager theInstance;
    public static GameManager Instance {  get
        {
            if (theInstance == null)
                theInstance = new GameManager();
            return theInstance;
        }
    }

    public GameObject player;
    
    public ProjectileManager projectileManager;
    public SpellIconManager spellIconManager;
    public EnemySpriteManager enemySpriteManager;
    public PlayerSpriteManager playerSpriteManager;
    public RelicIconManager relicIconManager;

    private List<GameObject> enemies;
    public int enemy_count { get { return enemies.Count; } }

    public int defectCount;
    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }
    public void RemoveEnemy(GameObject enemy)
    {
        defectCount += 1;
        enemies.Remove(enemy);
    }
    //when player die

    public void DestroyAllEnemies()
    {
        // 创建临时列表避免修改正在遍历的集合
        List<GameObject> enemiesToDestroy = new List<GameObject>(enemies);

        foreach (GameObject enemy in enemiesToDestroy)
        {
            if (enemy != null)
            {
                EnemyController controller = enemy.GetComponent<EnemyController>();
                if (controller != null)
                {
                    controller.Die(); // 调用敌人的Die方法进行清理
                }
                else
                {
                    GameObject.Destroy(enemy); // 如果没有控制器，直接销毁
                }
            }
        }

        enemies.Clear(); // 清空列表
    }
    public GameObject GetClosestEnemy(Vector3 point)
    {
        if (enemies == null || enemies.Count == 0) return null;
        if (enemies.Count == 1) return enemies[0];
        return enemies.Aggregate((a,b) => (a.transform.position - point).sqrMagnitude < (b.transform.position - point).sqrMagnitude ? a : b);
    }

    private GameManager()
    {
        
        enemies = new List<GameObject>();
    }
}
