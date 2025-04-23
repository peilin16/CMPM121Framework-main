using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RestartUI : MonoBehaviour
{
    public GameObject restartUI;
    public static RestartUI Instance;
    void Awake()
    {
        Instance = this;
        restartUI.SetActive(false); // Hide at start
    }

    public void Show()
    {
        Debug.Log("1111");
        restartUI.SetActive(true);
    }
    public void RestartGame()
    {
        GameManager.Instance.state = GameManager.GameState.PREGAME;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Restart current scene
    }
}