using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SummaryController : Singleton<SummaryController>
{
	public TextMeshProUGUI placementLabel;
	public TextMeshProUGUI timeLabel;

	public Button menuButton;
	public Button trackSelectButton;
	public Button playAgainButton;



    void Start()
    {
		if (GameManager.HasFinished)
		{
			placementLabel.gameObject.SetActive(true);
			timeLabel.gameObject.SetActive(true);

			placementLabel.text += GameManager.Placement.ToString();
			timeLabel.text += GameManager.Time.ToString("0");
		}

		EventSystem.current.SetSelectedGameObject(menuButton.gameObject);
		menuButton.onClick.AddListener(OnMenuButton);
		trackSelectButton.onClick.AddListener(OnTrackSelectButton);
		playAgainButton.onClick.AddListener(OnPlayAgainButton);
	}

    private void OnMenuButton()
	{
		// Remove the selected character for P1, and destroy other players
		(Player.GetPlayerByIndex(0) as Player).DestroyCharacter();
		Destroy(Player.GetPlayerBySlot(Player.PlayerSlot.P2));
		Destroy(Player.GetPlayerBySlot(Player.PlayerSlot.P3));
		Destroy(Player.GetPlayerBySlot(Player.PlayerSlot.P4));

		GameManager.ChangeState(GameManager.GameState.MENU);
	}

	private void OnTrackSelectButton()
	{
		GameManager.ChangeState(GameManager.GameState.TRACK);
	}

	private void OnPlayAgainButton()
	{
		GameManager.ChangeState(GameManager.GameState.GAME);
	}
}
