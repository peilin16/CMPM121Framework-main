using UnityEngine;
using TMPro;
public class TimeInformation : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public float waveSpendTime;
    private bool isTiming = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        tmp.text = "Spend Time: " + waveSpendTime;
    }

    void Update()
    {

        if (GameManager.Instance.state == GameManager.GameState.INWAVE)
        {
            if (!isTiming)
            {
                isTiming = true; 
                waveSpendTime = 0f; // reset
            }
            waveSpendTime += Time.deltaTime; // accuate
            
        }
        else
        {
            isTiming = false; // stop
        }
        if(GameManager.Instance.state == GameManager.GameState.PREGAME || GameManager.Instance.state == GameManager.GameState.WAVEEND || GameManager.Instance.state == GameManager.GameState.GAMEOVER)
        {
            tmp.text = "Spend Time: " + Mathf.Floor(waveSpendTime).ToString("F0") + "s"; //
            tmp.enabled = true;
        }
        else
        {
            tmp.enabled = false;
        }
           // Debug.Log(GameManager.Instance.state);
    }
}
