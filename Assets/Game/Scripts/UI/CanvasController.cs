using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class CanvasController : Singleton<CanvasController>
{
    [SerializeField, BoxGroup("Panels")] private GameObject panelMenu, panelInGame, panelEndGame;
    [SerializeField] private TextMeshProUGUI textStackIndicator;

    private void OnEnable()
    {
        GameManager.ActionGameStart += SetInGameUI;
        GameManager.ActionMiniGame += SetMiniGameUI;
        GameManager.ActionLevelPassed += SetEndGameUI;
    }

    private void SetInGameUI()
    {
        panelMenu.SetActive(false);
        panelInGame.SetActive(true);
    }

    private void SetMiniGameUI()
    {
        panelInGame.SetActive(false);
    }

    private void SetEndGameUI()
    {
        panelEndGame.SetActive(true);
    }

    #region UI Buttons' methods
    public void ButtonRestartPressed()
    {
        GameManager.Instance.RestartLevel();
    }

    public void ButtonStartPressed()
    {
        GameManager.ActionGameStart?.Invoke();
    }

    public void ButtonNextLevelPressed()
    {
        GameManager.Instance.LoadNextLevel();
    }
    #endregion

    public void UpdateStackIndicatorText(int stackSize)
    {
        textStackIndicator.text = stackSize.ToString();
    }

    private void OnDisable()
    {
        GameManager.ActionGameStart -= SetInGameUI;
        GameManager.ActionMiniGame -= SetMiniGameUI;
        GameManager.ActionLevelPassed -= SetEndGameUI;
    }
}
