using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;

public class ShipSelector : Singleton< ShipSelector >
{
    public ShipTile[] ShipTiles;
    public SelectorTile SelectorTile;
    [Space]
    public EventReference returnEvent;
    public EventReference selectEvent;

    private void Awake()
    {
        // Edge case
        if (PlayerInputManager.instance.playerCount == 0)
        {
            GameManager.ChangeState(GameManager.GameState.START);
            return;
        }

        Player primaryPlayer = Player.GetPlayerBySlot( Player.PlayerSlot.P1 );
        primaryPlayer.AddInputListener( Player.Control.MENU_NAVIGATION, OnDPADPressed );
        primaryPlayer.AddInputListener( Player.Control.A_PRESSED, OnAPressed );
        primaryPlayer.AddInputListener( Player.Control.B_PRESSED, OnBPressed );

        SelectorTile.SetPosition( ShipTiles[ 0 ].RectTransform.anchoredPosition );
        foreach (var obj in ShipTiles)
		{
            obj.Init();
		}
        ShipTiles[0].SetSelected(true);
    }

    private void OnDestroy()
    {
        Player primaryPlayer = Player.GetPlayerBySlot( Player.PlayerSlot.P1 );
        if (primaryPlayer == null)
            return;
        primaryPlayer.RemoveInputListener( Player.Control.MENU_NAVIGATION, OnDPADPressed );
        primaryPlayer.RemoveInputListener( Player.Control.A_PRESSED, OnAPressed );
        primaryPlayer.RemoveInputListener( Player.Control.B_PRESSED, OnBPressed );
    }

    private void OnDPADPressed( InputAction.CallbackContext a_CallbackContext )
    {
        Vector2 value = a_CallbackContext.ReadValue< Vector2 >().normalized;

        if ( value.x < -0.7f)
        {
            DecrementShipIndex( true );
        }
        else if ( value.x > 0.7f)
        {
            IncrementShipIndex( true );
        }
    }

    private void OnAPressed( InputAction.CallbackContext _ )
    {
        GameManager.RegisterSelectedShip( m_CurrentShipIndex, ShipTiles[ m_CurrentShipIndex ].MaxPlayers );
        GameManager.ChangeState(GameManager.GameState.CHARACTER);
        RuntimeManager.PlayOneShot(selectEvent);
    }

    private void OnBPressed( InputAction.CallbackContext _ )
    {
        GameManager.ChangeState(GameManager.GameState.MENU);
        RuntimeManager.PlayOneShot(returnEvent);
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

        ShipTiles[m_CurrentShipIndex].SetSelected(false);

        if ( ++m_CurrentShipIndex >= ShipTiles.Length )
        {
            m_CurrentShipIndex = 0;
        }

        SelectorTile.SetPosition( ShipTiles[ m_CurrentShipIndex ].RectTransform.anchoredPosition );
        ShipTiles[m_CurrentShipIndex].SetSelected(true);
    }

    public void DecrementShipIndex( bool a_LoopAround = false )
    {
        if ( m_CurrentShipIndex == 0 && !a_LoopAround )
        {
            return;
        }

        ShipTiles[m_CurrentShipIndex].SetSelected(false);

        if ( --m_CurrentShipIndex < 0 )
        {
            m_CurrentShipIndex = ShipTiles.Length - 1;
        }

        SelectorTile.SetPosition( ShipTiles[m_CurrentShipIndex ].RectTransform.anchoredPosition );
        ShipTiles[m_CurrentShipIndex].SetSelected(true);
    }

    private int m_CurrentShipIndex;
}