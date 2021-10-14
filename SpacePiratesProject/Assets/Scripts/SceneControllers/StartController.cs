using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartController : Singleton< StartController >
{
    public GameObject PressStartText;

    private void Awake()
    {
        // Use delay to prevent skip splash bug
        Invoke(nameof(Init), 0.05f);
    }

    private void Init()
	{
        m_PressStart = StartCoroutine(PressStart());

        // Edge case
        playerExists = PlayerInputManager.instance.playerCount > 0;
        if (playerExists)
        {
            (PlayerInput.GetPlayerByIndex(0) as Player).AddInputListener(Player.Control.A_PRESSED, OnStartPressed);
        }
        else
        {
            PlayerInputManager.instance.EnableJoining();
            PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        }
    }

    private void OnDestroy()
    {
        StopCoroutine(m_PressStart);
        PlayerInputManager.instance.DisableJoining();
        if (playerExists)
		{
            (PlayerInput.GetPlayerByIndex(0) as Player).RemoveInputListener(Player.Control.A_PRESSED, OnStartPressed);
        }
		else
		{
            PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
        }
    }

    public void OnPlayerJoined( PlayerInput a_PlayerInput )
    {
        PlayerInputManager.instance.DisableJoining();
        GameManager.ChangeState(GameManager.GameState.MENU);

        // Button spam fix
        Invoke(nameof(ChangeControl), 0.01f);
    }

    private void ChangeControl()
	{
        // Remove event for player join and change it to pressing A, since the player has already joined
        PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
        (PlayerInput.GetPlayerByIndex(0) as Player).AddInputListener(Player.Control.A_PRESSED, OnStartPressed);
        playerExists = true;

        // Try again. Hopfully it works this time, otherwise the player will need to press A again
        GameManager.ChangeState(GameManager.GameState.MENU);
    }

    private void OnStartPressed(InputAction.CallbackContext context)
	{
        GameManager.ChangeState(GameManager.GameState.MENU);
    }

    private IEnumerator PressStart()
    {
        while ( true )
        {
            yield return new WaitForSeconds( 0.5f );
            PressStartText.SetActive( false );
            yield return new WaitForSeconds( 0.5f );
            PressStartText.SetActive( true );
        }
    }

    private Coroutine m_PressStart;
    private bool playerExists;
}
