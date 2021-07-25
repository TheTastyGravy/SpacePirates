using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipSelector : MonoBehaviour
{
    public ShipTile[] ShipTiles;
    public int CurrentShipIndex;

    private void Start()
    {
        m_ShipTiles = new ShipTile[ ShipTiles.Length ];
        m_ParentShipTiles = m_ParentShipTiles ?? transform.Find( "ShipTiles" )?.GetComponent< RectTransform >();

        for ( int i = 0; i < m_ShipTiles.Length; ++i )
        {
            ShipTile newShipTile = Instantiate( ShipTiles[ i ], m_ParentShipTiles );
            newShipTile.SetPosition( Vector2.zero );
            m_ShipTiles[ i ] = newShipTile;
        }

        SetShipIndex( 0 );
        IPlayer primaryPlayer = ControllerManager.RetrievePlayer( IPlayer.PlayerSlot.P1 );
        primaryPlayer.GetInputAction( IPlayer.Control.DPAD_PRESSED ).performed += OnDPADPressed;
        primaryPlayer.GetInputAction( IPlayer.Control.A_PRESSED ).performed += OnAPressed;
        primaryPlayer.GetInputAction( IPlayer.Control.B_PRESSED ).performed += OnBPressed;
    }

    private void OnDestroy()
    {
        IPlayer primaryPlayer = ControllerManager.RetrievePlayer( IPlayer.PlayerSlot.P1 );
        primaryPlayer.GetInputAction( IPlayer.Control.DPAD_PRESSED ).performed -= OnDPADPressed;
        primaryPlayer.GetInputAction( IPlayer.Control.A_PRESSED ).performed -= OnAPressed;
        primaryPlayer.GetInputAction( IPlayer.Control.B_PRESSED ).performed -= OnBPressed;
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
        GameManager.RegisterSelectedShip( CurrentShipIndex );
        GameManager.CurrentState = GameManager.GameState.CHARACTER;
    }

    private void OnBPressed( InputAction.CallbackContext _ )
    {
        GameManager.CurrentState = GameManager.GameState.MENU;
    }

    public void SetShipIndex( int a_Index )
    {
        m_ShipTiles[ CurrentShipIndex ].gameObject.SetActive( false );
        
        if ( a_Index < 0 || a_Index >= m_ShipTiles.Length )
        {
            return;
        }

        m_ShipTiles[ a_Index ].gameObject.SetActive( true );
        CurrentShipIndex = a_Index;
    }

    public void IncrementShipIndex( bool a_LoopAround = false )
    {
        if ( CurrentShipIndex == m_ShipTiles.Length - 1 && !a_LoopAround )
        {
            return;
        }

        m_ShipTiles[ CurrentShipIndex ].gameObject.SetActive( false );

        if ( ++CurrentShipIndex >= m_ShipTiles.Length )
        {
            CurrentShipIndex = 0;
        }

        m_ShipTiles[ CurrentShipIndex ].gameObject.SetActive( true );
    }

    public void DecrementShipIndex( bool a_LoopAround = false )
    {
        if ( CurrentShipIndex == 0 && !a_LoopAround )
        {
            return;
        }

        m_ShipTiles[ CurrentShipIndex ].gameObject.SetActive( false );

        if ( --CurrentShipIndex < 0 )
        {
            CurrentShipIndex = m_ShipTiles.Length - 1;
        }

        m_ShipTiles[ CurrentShipIndex ].gameObject.SetActive( true );
    }

    private ShipTile[] m_ShipTiles;
    private RectTransform m_ParentShipTiles;
}