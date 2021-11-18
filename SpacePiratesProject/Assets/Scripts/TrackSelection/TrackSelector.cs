using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;

public class TrackSelector : Singleton< TrackSelector >
{
    public TrackTile[] TrackTiles;
    public SelectorTile SelectorTile;
    [Space]
    public EventReference returnEvent;
    public EventReference selectEvent;

    private int diffIndex = 0;


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
        primaryPlayer.AddInputListener( Player.Control.MENU_NAVIGATION, OnDPADPressed );
        primaryPlayer.AddInputListener( Player.Control.A_PRESSED, OnAPressed );
        primaryPlayer.AddInputListener( Player.Control.B_PRESSED, OnBPressed );

        TrackTiles[0].SetSelected(true);
    }

    private void OnDestroy()
    {
        Player primaryPlayer = Player.GetPlayerBySlot( Player.PlayerSlot.P1 );
        primaryPlayer.RemoveInputListener( Player.Control.MENU_NAVIGATION, OnDPADPressed );
        primaryPlayer.RemoveInputListener( Player.Control.A_PRESSED, OnAPressed );
        primaryPlayer.RemoveInputListener( Player.Control.B_PRESSED, OnBPressed );
    }

    private void OnDPADPressed( InputAction.CallbackContext a_CallbackContext )
    {
        Vector2 value = a_CallbackContext.ReadValue< Vector2 >().normalized;

        // Move left and right only if we are on the top row (easy, medium, hard)
        if ( value.x < -0.7f)
        {
            if (m_CurrentTrackIndex != 3)
                DecrementTrackIndex( true );
        }
        else if ( value.x > 0.7f)
        {
            if (m_CurrentTrackIndex != 3)
                IncrementTrackIndex( true );
        }
        else if (value.y < -0.7f || value.y > 0.7f)
        {
            TrackTiles[m_CurrentTrackIndex].SetSelected(false);
            if (m_CurrentTrackIndex == 3)
                m_CurrentTrackIndex = diffIndex;
            else
                m_CurrentTrackIndex = 3;

            TrackTiles[m_CurrentTrackIndex].SetSelected(true);
            SelectorTile.gameObject.SetActive(m_CurrentTrackIndex != 3);
        }
    }

    private void OnAPressed( InputAction.CallbackContext _ )
    {
        RuntimeManager.PlayOneShot(selectEvent);
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
        RuntimeManager.PlayOneShot(returnEvent);
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
        if ( m_CurrentTrackIndex == TrackTiles.Length - 2 && !a_LoopAround )
        {
            return;
        }

        TrackTiles[m_CurrentTrackIndex].SetSelected(false);

        if ( ++m_CurrentTrackIndex >= TrackTiles.Length -1 )
        {
            m_CurrentTrackIndex = 0;
        }

        SelectorTile.SetPosition( TrackTiles[ m_CurrentTrackIndex ].RectTransform.anchoredPosition );
        TrackTiles[m_CurrentTrackIndex].SetSelected(true);
        diffIndex = m_CurrentTrackIndex;
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
            m_CurrentTrackIndex = TrackTiles.Length - 2;
        }
        
        SelectorTile.SetPosition( TrackTiles[ m_CurrentTrackIndex ].RectTransform.anchoredPosition );
        TrackTiles[m_CurrentTrackIndex].SetSelected(true);
        diffIndex = m_CurrentTrackIndex;
    }

    private int m_CurrentTrackIndex;
}
