using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;

public class StartController : Singleton< StartController >
{
    public GameObject PressStartText;
    public EventReference selectEvent;

    void Start()
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
        RuntimeManager.PlayOneShot(selectEvent);

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
        RuntimeManager.PlayOneShot(selectEvent);
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
