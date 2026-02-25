using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public TMP_Text rescuedText;
    public TMP_Text inHelicopterText;
    public TMP_Text messageText;

    [Header("Game Data")]
    public int soldiersRescued = 0;
    public int soldiersInHelicopter = 0;

    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        messageText.text = "";
        UpdateUI();
    }

    private void Update()
    {
        // Input System: R key reset
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public bool CanPickUp() => soldiersInHelicopter < 3 && !gameEnded;

    public void PickUpSoldier()
    {
        if (gameEnded) return;
        soldiersInHelicopter++;
        UpdateUI();
    }

    public void DropOffSoldiers()
    {
        if (gameEnded) return;

        soldiersRescued += soldiersInHelicopter;
        soldiersInHelicopter = 0;
        UpdateUI();
        CheckWinCondition();
    }

    public void GameOver()
    {
        if (gameEnded) return;
        gameEnded = true;
        messageText.text = "GAME OVER";
    }

    private void CheckWinCondition()
    {
        var remaining = GameObject.FindGameObjectsWithTag("Soldier");
        if (remaining.Length == 0 && soldiersInHelicopter == 0 && !gameEnded)
        {
            gameEnded = true;
            messageText.text = "YOU WIN!";
        }
    }

    private void UpdateUI()
    {
        if (rescuedText != null) rescuedText.text = $"Soldiers Rescued: {soldiersRescued}";
        if (inHelicopterText != null) inHelicopterText.text = $"In Helicopter: {soldiersInHelicopter}";
    }

    public bool IsGameEnded() => gameEnded;
}
