using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using FMODUnity;

public class HUDOptionsMenu : MonoBehaviour
{
    public Button resumeButton;
    public Button menuButton;
    public Button restartButton;
    [Space]
    public EventReference returnEvent;

    private Player m_AssignedPlayer;
    private bool hasJustPaused;



    void Start()
	{
        resumeButton.onClick.AddListener(HideOptions);
        menuButton.onClick.AddListener(ReturnToMenu);
        restartButton.onClick.AddListener(RestartGame);
    }

    void OnDestroy()
    {
        resumeButton.onClick.RemoveListener(HideOptions);
        menuButton.onClick.RemoveListener(ReturnToMenu);
        restartButton.onClick.RemoveListener(RestartGame);

        if (m_AssignedPlayer != null)
        {
            m_AssignedPlayer.RemoveInputListener(Player.Control.B_PRESSED, BackAction);
            m_AssignedPlayer.RemoveInputListener(Player.Control.START_PRESSED, BackAction);
            Time.timeScale = 1.0f;
        }
    }

    public void ShowOptions( Player a_Player )
    {
        // Set the selected object to the resume button
        gameObject.SetActive(true);
        resumeButton.Select();

        hasJustPaused = true;
        m_AssignedPlayer = a_Player;
        m_AssignedPlayer.AddInputListener(Player.Control.B_PRESSED, BackAction);
        m_AssignedPlayer.AddInputListener(Player.Control.START_PRESSED, BackAction);
        Time.timeScale = 0.0f;
        ( EventSystem.current.currentInputModule as InputSystemUIInputModule ).actionsAsset = m_AssignedPlayer.actions;
    }

    public void HideOptions()
    {
        EventSystem.current.SetSelectedGameObject(null);
        m_AssignedPlayer.RemoveInputListener(Player.Control.B_PRESSED, BackAction);
        m_AssignedPlayer.RemoveInputListener(Player.Control.START_PRESSED, BackAction);
        m_AssignedPlayer = null;
        gameObject.SetActive( false );
        Time.timeScale = 1.0f;
    }

    private void BackAction( InputAction.CallbackContext context )
	{
        // If we just paused, ignore this start pressed event
        if (hasJustPaused && context.action == m_AssignedPlayer.actions.FindAction("START_PRESSED"))
		{
            hasJustPaused = false;
            return;
        }

        HideOptions();
        RuntimeManager.PlayOneShot(returnEvent);
    }

    private void ReturnToMenu()
	{
        GameManager.ChangeState(GameManager.GameState.MENU);
	}

    private void RestartGame()
    {
        GameManager.ReloadScene();
    }
}
