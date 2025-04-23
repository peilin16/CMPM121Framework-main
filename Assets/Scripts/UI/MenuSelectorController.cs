using UnityEngine;
using TMPro;

public class MenuSelectorController : MonoBehaviour
{
    public TextMeshProUGUI label;
    public string level;
    public EnemySpawner spawner;
    public string defectNum;
    public int timeSec;
    public int difficultly;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(this.difficultly == 1)
        {
            this.SetLevel("Easy");
            GameManager.Instance.level = GameManager.Difficultly.EASY;
        }
        
       // this.SetLevel("Medium");
        //this.SetLevel("Endless");
    }

 
    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevel(string text)
    {
        level = text;
        //label.text = text;
    }
    public void RestartLevel()
    {
        
    }
    public void StartLevel()
    {
        
        spawner.StartLevel(level);
    }
}
