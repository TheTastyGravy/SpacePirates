using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class HUDOptionsMenu : MonoBehaviour
{
    public Button resumeButton;
    public Button menuButton;
    public Button restartButton;



	void Start()
	{
        resumeButton.onClick.AddListener(HideOptions);
        menuButton.onClick.AddListener(ReturnToMenu);
        restartButton.onClick.AddListener(RestartGame);
    }

	public void ShowOptions( Player a_Player )
    {
        // Set the selected object to the resume button
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        resumeButton.OnSelect(null);

        startFlag = false;
        gameObject.SetActive( true );
        m_AssignedPlayer = a_Player;
        m_AssignedPlayer.AddInputListener(Player.Control.B_PRESSED, BackAction);
        m_AssignedPlayer.AddInputListener(Player.Control.START_PRESSED, BackAction);
        Time.timeScale = 0.0f;
        ( EventSystem.current.currentInputModule as InputSystemUIInputModule ).actionsAsset = m_AssignedPlayer.actions;
    }

    public void HideOptions()
    {
        m_AssignedPlayer.RemoveInputListener(Player.Control.B_PRESSED, BackAction);
        m_AssignedPlayer.RemoveInputListener(Player.Control.START_PRESSED, BackAction);
        m_AssignedPlayer = null;
        gameObject.SetActive( false );
        Time.timeScale = 1.0f;
    }


    bool startFlag = false;
    private void BackAction( InputAction.CallbackContext context )
	{
        if (!startFlag && context.action == m_AssignedPlayer.actions.FindAction("START_PRESSED"))
		{
            startFlag = true;
            return;
        }

        HideOptions();
    }

    private void ReturnToMenu()
	{
        GameManager.ChangeState(GameManager.GameState.MENU);
	}

    private void RestartGame()
    {
        GameManager.ReloadScene();
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

	private Player m_AssignedPlayer;
}
