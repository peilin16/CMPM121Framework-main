using UnityEngine;

public class RewardScreenManager : MonoBehaviour
{
    public GameObject rewardUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("wave");
        if (GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            //Debug.Log("wave111");
            rewardUI.SetActive(true);
        }
        else
        {
            rewardUI.SetActive(false);
        }
    }
}
