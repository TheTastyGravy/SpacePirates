using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartController : Singleton< StartController >
{
    public GameObject PressStartText;

    private void Start()
    {
        m_PressStart = StartCoroutine( PressStart() );
        PlayerInputManager.instance.EnableJoining();
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
    }

    private void OnDestroy()
    {
        PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
    }

    public void OnPlayerJoined( PlayerInput a_PlayerInput )
    {
        PlayerInputManager.instance.DisableJoining();
        StopCoroutine( m_PressStart );
        GameManager.CurrentState = GameManager.GameState.MENU;
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
    private InputDevice m_LastUsedInputDevice;
}
