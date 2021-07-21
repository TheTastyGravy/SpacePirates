using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerInput : Singleton< ControllerInput >
{
    private void Start()
    {
        PlayerInputManager.instance.onPlayerJoined += OnControllerConnected;
        PlayerInputManager.instance.onPlayerLeft += OnControllerDisconnected;
    }
    
    public static PlayerInput Retrieve( int a_Index )
    {
        if ( a_Index < 0 || a_Index > 3 )
        {
            return null;
        }

        return Instance.m_PlayerInputs[ a_Index ];
    }

    private void OnControllerConnected( PlayerInput a_PlayerInput )
    {
        for ( int i = 0; i < 4; ++i )
        {
            if ( m_PlayerInputs[ i ] == null )
            {
                m_PlayerInputs[ i ] = a_PlayerInput;
                a_PlayerInput.onDeviceLost += DeviceLost;
                a_PlayerInput.onDeviceRegained += DeviceGained;
                return;
            }
        }
    }
    
    private void OnControllerDisconnected( PlayerInput a_PlayerInput )
    {
        int i = Array.IndexOf( m_PlayerInputs, a_PlayerInput );

        if ( i == -1 )
        {
            return;
        }

        m_PlayerInputs[ i ] = null;
        
        for ( ; i < 3; ++i )
        {
            m_PlayerInputs[ i ] = m_PlayerInputs[ i + 1 ];
        }

        m_PlayerInputs[ 3 ] = null;
    }

    private void DeviceLost( PlayerInput playerInput)
    {
        Debug.LogWarning( "device lost" );
    }

    private void DeviceGained( PlayerInput playerInput)
    {
        Debug.LogWarning( "device gained" );
    }

    private PlayerInput[] m_PlayerInputs = new PlayerInput[ 4 ];
}