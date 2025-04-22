using UnityEngine;
using TMPro;

public class RestartUI : MonoBehaviour
{
    public GameObject restartUI; // TMP_Text
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        enabled = false; // Ω˚”√Ω≈±æ±‹√‚¥ÌŒÛ
                         // restartUI.SetActive(false);
    }

    // Update is called once per frame
    public void Update()
    {
        Debug.Log("1111");
        if (GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            enabled = true; // 
            Debug.Log("You Lost");
           // restartUI.SetActive(true);
        }
        else
        {
            //restartUI.SetActive(false);
        }
    }


}
