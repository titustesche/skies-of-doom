using UnityEngine;

public static class GamestateManager
{
    public enum Gamestates
    {
        Running,
        Pause,
        Dead,
        Shop,
    }

    public static Gamestates Gamestate = Gamestates.Pause;
    
    public static void SetGameState(Gamestates state)
    {
        UIController.Instance.UpdateStats(PlayerController.Instance);
        if (state == Gamestate) return;
        Gamestate = state;
        switch (state)
        {
            case Gamestates.Running:
                UIController.Instance.PauseMenu.SetActive(false);
                UIController.Instance.IngameOverlay.SetActive(true);
                UIController.Instance.ShopOverlay.SetActive(false);
                TimeManager.Instance.Running = true;
                Time.timeScale = 1;
                break;
            case Gamestates.Shop:
                UIController.Instance.PauseMenu.SetActive(false);
                UIController.Instance.ShopOverlay.SetActive(true);
                ShopController.Instance.UpdateTileShop();
                TimeManager.Instance.Running = true;
                Time.timeScale = 0.7f;
                break;
            case Gamestates.Pause:
                UIController.Instance.PauseMenu.SetActive(true);
                UIController.Instance.IngameOverlay.SetActive(false);
                UIController.Instance.ShopOverlay.SetActive(false);
                TimeManager.Instance.Running = false;
                Time.timeScale = 0;
                break;
            case Gamestates.Dead:
                UIController.Instance.GameOverMenu.SetActive(true);
                UIController.Instance.IngameOverlay.SetActive(false);
                UIController.Instance.ShopOverlay.SetActive(false);
                UIController.Instance.PauseMenu.SetActive(false);
                TimeManager.Instance.Running = false;
                Time.timeScale = 0;
                break;
        }
    }
}
