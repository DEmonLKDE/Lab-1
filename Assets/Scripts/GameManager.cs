using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Text rescuedText;
    public Text inHelicopterText;
    public Text messageText;

    public int soldiersRescued = 0;
    public int soldiersInHelicopter = 0;

    private bool gameEnded = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public bool CanPickUp()
    {
        return soldiersInHelicopter < 3 && !gameEnded;
    }

    public void PickUp()
    {
        soldiersInHelicopter++;
        UpdateUI();
        CheckWin();
    }

    public void DropOff()
    {
        soldiersRescued += soldiersInHelicopter;
        soldiersInHelicopter = 0;
        UpdateUI();
        CheckWin();
    }

    public void GameOver()
    {
        gameEnded = true;
        messageText.text = "Game Over";
    }

    void CheckWin()
    {
        GameObject[] soldiers = GameObject.FindGameObjectsWithTag("Soldier");
        if (soldiers.Length == 0 && !gameEnded)
        {
            messageText.text = "You Win!";
            gameEnded = true;
        }
    }

    void UpdateUI()
    {
        rescuedText.text = "Soldiers Rescued: " + soldiersRescued;
        inHelicopterText.text = "In Helicopter: " + soldiersInHelicopter;
    }

    public bool IsGameEnded()
    {
        return gameEnded;
    }
}
