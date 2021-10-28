using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrackSelector : Singleton< TrackSelector >
{
    public TrackTile[] TrackTiles;
    public SelectorTile SelectorTile;

    private void Start()
    {
        // Edge case
        if (PlayerInputManager.instance.playerCount == 0)
        {
            GameManager.ChangeState(GameManager.GameState.START, true);
            return;
        }
        // Another edge case
        if (Player.GetPlayerBySlot(Player.PlayerSlot.P1).Character == null)
		{
            GameManager.ChangeState(GameManager.GameState.CHARACTER, true);
            return;
		}

        Player primaryPlayer = Player.GetPlayerBySlot( Player.PlayerSlot.P1 );
        primaryPlayer.AddInputListener( Player.Control.DPAD_PRESSED, OnDPADPressed );
        primaryPlayer.AddInputListener( Player.Control.A_PRESSED, OnAPressed );
        primaryPlayer.AddInputListener( Player.Control.B_PRESSED, OnBPressed );

        TrackTiles[0].SetSelected(true);
    }

    private void OnDestroy()
    {
        Player primaryPlayer = Player.GetPlayerBySlot( Player.PlayerSlot.P1 );
        primaryPlayer.RemoveInputListener( Player.Control.DPAD_PRESSED, OnDPADPressed );
        primaryPlayer.RemoveInputListener( Player.Control.A_PRESSED, OnAPressed );
        primaryPlayer.RemoveInputListener( Player.Control.B_PRESSED, OnBPressed );
    }

    private void OnDPADPressed( InputAction.CallbackContext a_CallbackContext )
    {
        Vector2 value = a_CallbackContext.ReadValue< Vector2 >();

        if ( value.x < 0 )
        {
            DecrementTrackIndex( true );
        }
        else if ( value.x > 0 )
        {
            IncrementTrackIndex( true );
        }
    }

    private void OnAPressed( InputAction.CallbackContext _ )
    {
        // Check again incase the one in Start failed
        if (Player.GetPlayerBySlot(Player.PlayerSlot.P1).Character == null || Player.GetPlayerBySlot(Player.PlayerSlot.P1).Character.gameObject == null)
        {
            GameManager.ChangeState(GameManager.GameState.CHARACTER, true);
            return;
        }

        GameManager.RegisterSelectedTrack( m_CurrentTrackIndex );
        GameManager.ChangeState(GameManager.GameState.LOADING);
    }

    private void OnBPressed( InputAction.CallbackContext _ )
    {
        GameManager.ChangeState(GameManager.GameState.CHARACTER);
    }

    public void SetTrackIndex( int a_Index )
    {
        TrackTiles[ m_CurrentTrackIndex ].gameObject.SetActive( false );
        
        if ( a_Index < 0 || a_Index >= TrackTiles.Length )
        {
            return;
        }
        m_CurrentTrackIndex = a_Index;
        SelectorTile.SetPosition( TrackTiles[ m_CurrentTrackIndex ].RectTransform.anchoredPosition );
    }

    public void IncrementTrackIndex( bool a_LoopAround = false )
    {
        if ( m_CurrentTrackIndex == TrackTiles.Length - 1 && !a_LoopAround )
        {
            return;
        }

        TrackTiles[m_CurrentTrackIndex].SetSelected(false);

        if ( ++m_CurrentTrackIndex >= TrackTiles.Length )
        {
            m_CurrentTrackIndex = 0;
        }

        SelectorTile.SetPosition( TrackTiles[ m_CurrentTrackIndex ].RectTransform.anchoredPosition );
        TrackTiles[m_CurrentTrackIndex].SetSelected(true);
    }

    public void DecrementTrackIndex( bool a_LoopAround = false )
    {
        if ( m_CurrentTrackIndex == 0 && !a_LoopAround )
        {
            return;
        }

        TrackTiles[m_CurrentTrackIndex].SetSelected(false);

        if ( --m_CurrentTrackIndex < 0 )
        {
            m_CurrentTrackIndex = TrackTiles.Length - 1;
        }
        
        SelectorTile.SetPosition( TrackTiles[ m_CurrentTrackIndex ].RectTransform.anchoredPosition );
        TrackTiles[m_CurrentTrackIndex].SetSelected(true);
    }

    private int m_CurrentTrackIndex;
}
