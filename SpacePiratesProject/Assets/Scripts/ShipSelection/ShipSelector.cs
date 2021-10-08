using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipSelector : Singleton< ShipSelector >
{
    public ShipTile[] ShipTiles;
    public SelectorTile SelectorTile;

    private void Start()
    {
        // Edge case
        if (PlayerInputManager.instance.playerCount == 0)
        {
            GameManager.ChangeState(GameManager.GameState.START);
            return;
        }

        Player primaryPlayer = Player.GetPlayerBySlot( Player.PlayerSlot.P1 );
        primaryPlayer.AddInputListener( Player.Control.DPAD_PRESSED, OnDPADPressed );
        primaryPlayer.AddInputListener( Player.Control.A_PRESSED, OnAPressed );
        primaryPlayer.AddInputListener( Player.Control.B_PRESSED, OnBPressed );

        SelectorTile.SetPosition( ShipTiles[ 0 ].RectTransform.anchoredPosition );
    }

    private void OnDestroy()
    {
        Player primaryPlayer = Player.GetPlayerBySlot( Player.PlayerSlot.P1 );
        if (primaryPlayer == null)
            return;
        primaryPlayer.RemoveInputListener( Player.Control.DPAD_PRESSED, OnDPADPressed );
        primaryPlayer.RemoveInputListener( Player.Control.A_PRESSED, OnAPressed );
        primaryPlayer.RemoveInputListener( Player.Control.B_PRESSED, OnBPressed );
    }

    private void OnDPADPressed( InputAction.CallbackContext a_CallbackContext )
    {
        Vector2 value = a_CallbackContext.ReadValue< Vector2 >();

        if ( value.x < 0 )
        {
            DecrementShipIndex( true );
        }
        else if ( value.x > 0 )
        {
            IncrementShipIndex( true );
        }
    }

    private void OnAPressed( InputAction.CallbackContext _ )
    {
        GameManager.RegisterSelectedShip( m_CurrentShipIndex, ShipTiles[ m_CurrentShipIndex ].MaxPlayers );
        GameManager.ChangeState(GameManager.GameState.CHARACTER);
    }

    private void OnBPressed( InputAction.CallbackContext _ )
    {
        GameManager.ChangeState(GameManager.GameState.MENU);
    }

    public void SetShipIndex( int a_Index )
    {
        if ( a_Index < 0 || a_Index >= ShipTiles.Length )
        {
            return;
        }

        m_CurrentShipIndex = a_Index;

        SelectorTile.SetPosition( ShipTiles[ m_CurrentShipIndex ].RectTransform.anchoredPosition );
    }

    public void IncrementShipIndex( bool a_LoopAround = false )
    {
        if ( m_CurrentShipIndex == ShipTiles.Length - 1 && !a_LoopAround )
        {
            return;
        }

        if ( ++m_CurrentShipIndex >= ShipTiles.Length )
        {
            m_CurrentShipIndex = 0;
        }

        SelectorTile.SetPosition( ShipTiles[ m_CurrentShipIndex ].RectTransform.anchoredPosition );
    }

    public void DecrementShipIndex( bool a_LoopAround = false )
    {
        if ( m_CurrentShipIndex == 0 && !a_LoopAround )
        {
            return;
        }

        if ( --m_CurrentShipIndex < 0 )
        {
            m_CurrentShipIndex = ShipTiles.Length - 1;
        }

        SelectorTile.SetPosition( ShipTiles[m_CurrentShipIndex ].RectTransform.anchoredPosition );
    }

    private int m_CurrentShipIndex;
}