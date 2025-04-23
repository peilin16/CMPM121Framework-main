using UnityEngine;
using TMPro;

public class TimeInformation : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    public float waveSpendTime = 0f;
    private bool isTiming = false;

    void Start()
    {
        tmp.text = "Spend Time: 0s";
    }

    void Update()
    {
        var state = GameManager.Instance.state;

        if (state == GameManager.GameState.INWAVE)
        {
            if (!isTiming)
            {
                isTiming = true;
                waveSpendTime = 0f;
            }

            waveSpendTime += Time.deltaTime;
            tmp.text = ""; // Hide value while wave is ongoing
        }
        else if (state == GameManager.GameState.WAVEEND || state == GameManager.GameState.GAMEOVER)
        {
            if (isTiming)
            {
                isTiming = false;
                tmp.text = "Spend Time: " + Mathf.Floor(waveSpendTime).ToString("F0") + "s";
            }
        }
        else
        {
            tmp.text = ""; // Hide in other states (PREGAME, COUNTDOWN, etc.)
        }
    }
}
