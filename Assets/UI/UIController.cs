using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    public enum Gamestates
    {
        Running,
        Pause,
        Dead
    }

    public Gamestates Gamestate { get; private set; } = Gamestates.Running;
    
    public GameObject PauseMenu;
    public GameObject IngameOverlay;
    public GameObject GameOverMenu;
    public GameObject ShopOverlay;
    public GameObject StatsOverlay;

    private TMP_Text _killCounter;
    private TMP_Text _damageCounter;
    private TMP_Text _moneyCounter;
    private TMP_Text _healthCounter;
    private TMP_Text _attackSpeedMultiplierCounter;
    private TMP_Text _damageMultiplierCounter;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Nested shyt
        _killCounter = StatsOverlay.transform.Find("Kills").transform.Find("KillCounter").GetComponent<TMP_Text>();
        _damageCounter = StatsOverlay.transform.Find("Damage").transform.Find("DamageCounter").GetComponent<TMP_Text>();
        _attackSpeedMultiplierCounter = StatsOverlay.transform.Find("Modifiers").transform.Find("AttackSpeedMultiplier").transform.Find("AttackSpeedMultiplierCounter").GetComponent<TMP_Text>();
        _damageMultiplierCounter = StatsOverlay.transform.Find("Modifiers").transform.Find("DamageMultiplier").transform.Find("DamageMultiplierCounter").GetComponent<TMP_Text>();
        _moneyCounter = IngameOverlay.transform.Find("Money").transform.Find("MoneyText").GetComponent<TMP_Text>();
        _healthCounter = IngameOverlay.transform.Find("Health").transform.Find("HealthText").GetComponent<TMP_Text>();
    }
    
    #region Stats Overlay

    public void UpdateStats(PlayerController player)
    {
        _killCounter.text = player.Kills.ToString();
        _damageCounter.text = player.TotalDamage.ToString();
        _moneyCounter.text = player.Money.ToString();
        _healthCounter.text = player.Health.ToString();
        _attackSpeedMultiplierCounter.text = Math.Round(player.AttackSpeedMultiplier * 100, 2) + "%";
        _damageMultiplierCounter.text = Math.Round(player.damageMultplier * 100, 2) + "%";
    }

    public void ShowStatsOverlay()
    {
        StatsOverlay.SetActive(true);
    }

    public void HideStatsOverlay()
    {
        StatsOverlay.SetActive(false);
    }
    
    #endregion

    public void SetShop(bool active)
    {
        ShopOverlay.SetActive(active);
    }
    
    #region Gamestate

    public void SetGameState(Gamestates state)
    {
        Gamestate = state;
        switch (state)
        {
            case Gamestates.Running:
                PauseMenu.SetActive(false);
                IngameOverlay.SetActive(true);
                Time.timeScale = 1;
                break;
            case Gamestates.Pause:
                PauseMenu.SetActive(true);
                IngameOverlay.SetActive(false);
                ShopOverlay.SetActive(false);
                Time.timeScale = 0;
                break;
            case Gamestates.Dead:
                GameOverMenu.SetActive(true);
                IngameOverlay.SetActive(false);
                ShopOverlay.SetActive(false);
                PauseMenu.SetActive(false);
                break;
        }
    }
    #endregion

    public void LoadMetaProgressionInterface()
    {
        SceneManager.LoadScene("Meta Progression");
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void LeaveGame()
    {
        Application.Quit();
    }
}
