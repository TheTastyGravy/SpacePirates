using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrackSelector : Singleton< TrackSelector >
{
    public TrackTile[] TrackTiles;

    private void Start()
    {
        m_TrackTiles = new TrackTile[ TrackTiles.Length ];

        for ( int i = 0; i < m_TrackTiles.Length; ++i )
        {
            TrackTile newTrackTile = Instantiate( TrackTiles[ i ], m_ParentTrackTiles );
            newTrackTile.SetPosition( Vector2.zero );
            m_TrackTiles[ i ] = newTrackTile;
        }

        SetTrackIndex( 0 );
        IPlayer primaryPlayer = ControllerManager.RetrievePlayer( IPlayer.PlayerSlot.P1 );
        primaryPlayer.AddInputListener( IPlayer.Control.DPAD_PRESSED, OnDPADPressed );
        primaryPlayer.AddInputListener( IPlayer.Control.A_PRESSED, OnAPressed );
        primaryPlayer.AddInputListener( IPlayer.Control.B_PRESSED, OnBPressed );
    }

    private void OnDestroy()
    {
        IPlayer primaryPlayer = ControllerManager.RetrievePlayer( IPlayer.PlayerSlot.P1 );
        primaryPlayer.RemoveInputListener( IPlayer.Control.DPAD_PRESSED, OnDPADPressed );
        primaryPlayer.RemoveInputListener( IPlayer.Control.A_PRESSED, OnAPressed );
        primaryPlayer.RemoveInputListener( IPlayer.Control.B_PRESSED, OnBPressed );
    }

    private void OnDPADPressed( InputAction.CallbackContext a_CallbackContext )
    {
        Vector2 value = a_CallbackContext.ReadValue< Vector2 >();

        if ( value.x < 0 )
        {
            DecrementTrackIndex();
        }
        else if ( value.x > 0 )
        {
            IncrementTrackIndex();
        }
    }

    private void OnAPressed( InputAction.CallbackContext _ )
    {
        GameManager.RegisterSelectedTrack( m_CurrentTrackIndex );
        GameManager.CurrentState = GameManager.GameState.GAME;
    }

    private void OnBPressed( InputAction.CallbackContext _ )
    {
        GameManager.CurrentState = GameManager.GameState.CHARACTER;
    }

    public void SetTrackIndex( int a_Index )
    {
        m_TrackTiles[ m_CurrentTrackIndex ].gameObject.SetActive( false );
        
        if ( a_Index < 0 || a_Index >= m_TrackTiles.Length )
        {
            return;
        }

        m_TrackTiles[ a_Index ].gameObject.SetActive( true );
        m_CurrentTrackIndex = a_Index;
    }

    public void IncrementTrackIndex( bool a_LoopAround = false )
    {
        if ( m_CurrentTrackIndex == m_TrackTiles.Length - 1 && !a_LoopAround )
        {
            return;
        }

        m_TrackTiles[ m_CurrentTrackIndex ].gameObject.SetActive( false );

        if ( ++m_CurrentTrackIndex >= m_TrackTiles.Length )
        {
            m_CurrentTrackIndex = 0;
        }

        m_TrackTiles[ m_CurrentTrackIndex ].gameObject.SetActive( true );
    }

    public void DecrementTrackIndex( bool a_LoopAround = false )
    {
        if ( m_CurrentTrackIndex == 0 && !a_LoopAround )
        {
            return;
        }

        m_TrackTiles[ m_CurrentTrackIndex ].gameObject.SetActive( false );

        if ( --m_CurrentTrackIndex < 0 )
        {
            m_CurrentTrackIndex = m_TrackTiles.Length - 1;
        }

        m_TrackTiles[ m_CurrentTrackIndex ].gameObject.SetActive( true );
    }

    private TrackTile[] m_TrackTiles;
    private int m_CurrentTrackIndex;
    [ SerializeField ] private RectTransform m_ParentTrackTiles;
}
