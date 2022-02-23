using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public static UnityAction ActionGameStart, ActionMiniGame, ActionLevelPassed;
    private bool _enteredMiniGame = false;
    public bool EnteredMiniGame => _enteredMiniGame;
    private bool _gameStarted = false;
    public bool GameStarted => _gameStarted;

    private void OnEnable()
    {
        ActionMiniGame += SetMiniGame;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        int nextLevel = PlayerPrefs.GetInt("LEVEL", 0);
        nextLevel++;
        PlayerPrefs.SetInt("LEVEL", nextLevel); 
        SceneManager.LoadScene(nextLevel);     
    }

    private void SetMiniGame()
    {
        _enteredMiniGame = true;
    }

    private void OnDisable()
    {
        ActionMiniGame -= SetMiniGame;
    }
}
