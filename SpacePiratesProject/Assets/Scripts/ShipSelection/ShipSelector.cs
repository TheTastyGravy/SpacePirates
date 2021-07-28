using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipSelector : MonoBehaviour
{
    public ShipTile[] ShipTiles;

    private void Start()
    {
        m_ShipTiles = new ShipTile[ ShipTiles.Length ];

        for ( int i = 0; i < m_ShipTiles.Length; ++i )
        {
            ShipTile newShipTile = Instantiate( ShipTiles[ i ], m_ParentShipTiles );
            newShipTile.SetPosition( Vector2.zero );
            m_ShipTiles[ i ] = newShipTile;
        }

        SetShipIndex( 0 );
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
            DecrementShipIndex();
        }
        else if ( value.x > 0 )
        {
            IncrementShipIndex();
        }
    }

    private void OnAPressed( InputAction.CallbackContext _ )
    {
        GameManager.RegisterSelectedShip( m_CurrentShipIndex, ShipTiles[ m_CurrentShipIndex ].MaxPlayers );
        GameManager.CurrentState = GameManager.GameState.CHARACTER;
    }

    private void OnBPressed( InputAction.CallbackContext _ )
    {
        GameManager.CurrentState = GameManager.GameState.MENU;
    }

    public void SetShipIndex( int a_Index )
    {
        m_ShipTiles[ m_CurrentShipIndex ].gameObject.SetActive( false );
        
        if ( a_Index < 0 || a_Index >= m_ShipTiles.Length )
        {
            return;
        }

        m_ShipTiles[ a_Index ].gameObject.SetActive( true );
        m_CurrentShipIndex = a_Index;
    }

    public void IncrementShipIndex( bool a_LoopAround = false )
    {
        if ( m_CurrentShipIndex == m_ShipTiles.Length - 1 && !a_LoopAround )
        {
            return;
        }

        m_ShipTiles[ m_CurrentShipIndex ].gameObject.SetActive( false );

        if ( ++m_CurrentShipIndex >= m_ShipTiles.Length )
        {
            m_CurrentShipIndex = 0;
        }

        m_ShipTiles[ m_CurrentShipIndex ].gameObject.SetActive( true );
    }

    public void DecrementShipIndex( bool a_LoopAround = false )
    {
        if ( m_CurrentShipIndex == 0 && !a_LoopAround )
        {
            return;
        }

        m_ShipTiles[ m_CurrentShipIndex ].gameObject.SetActive( false );

        if ( --m_CurrentShipIndex < 0 )
        {
            m_CurrentShipIndex = m_ShipTiles.Length - 1;
        }

        m_ShipTiles[ m_CurrentShipIndex ].gameObject.SetActive( true );
    }

    private ShipTile[] m_ShipTiles;
    private int m_CurrentShipIndex;
    [ SerializeField ] private RectTransform m_ParentShipTiles;
}