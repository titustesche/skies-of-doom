using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControllerDummy : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadMetaProgressionInterface()
    {
        SceneManager.LoadScene("Meta Progression");
    }

    public void ResumeGame()
    {
        UIController.Instance.SetGameState(UIController.Gamestates.Running);
    }

    public void PauseGame()
    {
        UIController.Instance.SetGameState(UIController.Gamestates.Pause);
    }
}
