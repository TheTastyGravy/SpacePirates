using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartController : MonoBehaviour
{
    public GameObject PressStartText;
    public PlayerInput PlayerInput;

    private void Start()
    {
        m_PressStart = StartCoroutine( PressStart() );
        PlayerInputManager.instance.EnableJoining();
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
    }

    public void OnPlayerJoined( PlayerInput a_PlayerInput )
    {
        PlayerInputManager.instance.DisableJoining();

    }

    private void OnStartPressed()
    {
        StopCoroutine( m_PressStart );
        //ControllerManager.Instance.RegisterInputDevice( Player.PlayerSlot.P1, m_LastUsedInputDevice );
        GameManager.Instance.SwitchToState( GameManager.GameState.MAIN );
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
