using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TMP_Text rescuedText;
    public TMP_Text inHelicopterText;
    public TMP_Text messageText;
    public TMP_Text fuelText;
    public Slider fuelBar;

    [Header("Game Data")]
    public int soldiersRescued = 0;
    public int soldiersInHelicopter = 0;

    [Header("Fuel System")]
    public float maxFuel = 100f;
    public float currentFuel = 100f;

    private bool gameEnded = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentFuel = maxFuel;
        if (fuelBar != null)
        {
            fuelBar.maxValue = maxFuel;
            fuelBar.value = currentFuel;
        }

        if (messageText != null)
            messageText.text = "";

        UpdateUI();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public bool IsGameEnded() => gameEnded;

    public bool CanPickUp() => soldiersInHelicopter < 3 && !gameEnded;

    public void PickUpSoldier()
    {
        soldiersInHelicopter++;
        UpdateUI();
    }

    public void DropOffSoldiers()
    {
        soldiersRescued += soldiersInHelicopter;
        soldiersInHelicopter = 0;
        RefuelToFull();
        UpdateUI();
        CheckWinCondition();
    }

    public void RefuelToFull()
    {
        currentFuel = maxFuel;
        UpdateFuelUI();
    }

    public void ConsumeFuel(float baseDrainPerSecond)
    {
        if (gameEnded) return;

        float multiplier = 1f + (soldiersInHelicopter * 0.5f);
        float drain = baseDrainPerSecond * multiplier * Time.deltaTime;

        currentFuel -= drain;

        if (currentFuel <= 0f)
        {
            currentFuel = 0f;
            GameOver("OUT OF FUEL");
        }

        UpdateFuelUI();
    }

    void UpdateFuelUI()
    {
        if (fuelText != null)
            fuelText.text = $"Fuel: {Mathf.CeilToInt(currentFuel)}";

        if (fuelBar != null)
            fuelBar.value = currentFuel;

        if (fuelBar != null)
        {
            Image fill = fuelBar.fillRect.GetComponent<Image>();
            float percent = currentFuel / maxFuel;

            if (percent > 0.5f)
                fill.color = Color.green;
            else if (percent > 0.25f)
                fill.color = Color.yellow;
            else
                fill.color = Color.red;
        }
    }

    public void GameOver(string reason)
    {
        if (gameEnded) return;

        gameEnded = true;
        if (messageText != null)
            messageText.text = reason;
    }

    void CheckWinCondition()
    {
        var remaining = GameObject.FindGameObjectsWithTag("Soldier");

        if (remaining.Length == 0 && soldiersInHelicopter == 0)
        {
            messageText.text = "YOU WIN!";
            gameEnded = true;
        }
    }

    void UpdateUI()
    {
        if (rescuedText != null)
            rescuedText.text = $"Rescued: {soldiersRescued}";

        if (inHelicopterText != null)
            inHelicopterText.text = $"In Helicopter: {soldiersInHelicopter}";
    }
}
