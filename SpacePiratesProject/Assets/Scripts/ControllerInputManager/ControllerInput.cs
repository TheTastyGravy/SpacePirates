using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class ControllerInput : Singleton< ControllerInput >
{
    // For testing, to remove later
    public Text Text;

    public int ControllerCount
    {
        get
        {
            return Gamepad.all.Count;
        }
    }

    private void Start()
    {
        PlayerInputManager.instance.onPlayerJoined += OnControllerConnected;
        PlayerInputManager.instance.onPlayerLeft += OnControllerDisconnected;
        InputSystem.onDeviceChange += OnDeviceChange;

        Gamepad.all.ToArray().CopyTo( m_InputDevices, 0 );
        var devices = InputSystem.devices.ToArray();
        //RefreshReadout();
    }

    public void OnDeviceChange( InputDevice a_InputDevice, InputDeviceChange a_Change )
    {
        if ( a_InputDevice is Gamepad gamepad )
        {
            switch ( a_Change )
            {
                case InputDeviceChange.Added:
                    {
                        for ( int i = 0; i < 4; ++i )
                        {
                            if ( m_InputDevices[ i ] == null )
                            {
                                m_InputDevices[ i ] = a_InputDevice;
                                break;
                            }
                        }
                    }
                    break;
                case InputDeviceChange.Removed:
                    {
                        for ( int i = 0; i < 4; ++i )
                        {
                            if ( m_InputDevices[ i ] == null )
                            {
                                return;
                            }

                            if ( m_InputDevices[ i ].deviceId == a_InputDevice.deviceId )
                            {
                                m_InputDevices[ i ] = null;
                                break;
                            }
                        }
                    }
                    break;
                case InputDeviceChange.Disconnected:
                    break;
                case InputDeviceChange.Reconnected:
                    break;
                case InputDeviceChange.Enabled:
                    break;
                case InputDeviceChange.Disabled:
                    break;
                case InputDeviceChange.UsageChanged:
                    break;
                case InputDeviceChange.ConfigurationChanged:
                    break;
                case InputDeviceChange.Destroyed:
                    break;
                default:
                    break;
            }
        }

        RefreshReadout();
    }

    // test code, remove later
    public void RefreshReadout()
    {
        string text = "";

        for ( int i = 0; i < 4; ++i )
        {
            text += m_InputDevices[ i ] == null ? "none" : m_InputDevices[ i ].displayName;

            if ( m_PlayerInputs[ i ] != null )
            {
                text += " - joined";
            }
            else
            {
                text += " - not joined";
            }

            text += '\n';
        }

        Text.text = text;
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
        return;
        for ( int i = 0; i < 4; ++i )
        {
            if ( a_PlayerInput.devices[ 0 ].deviceId == m_InputDevices[ i ].deviceId )
            {
                m_PlayerInputs[ i ] = a_PlayerInput;
            }
        }

        RefreshReadout();
    }
    
    private void OnControllerDisconnected( PlayerInput a_PlayerInput )
    {

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
    private InputDevice[] m_InputDevices = new InputDevice[ 4 ];
    private Player[] m_Players = new Player[ 4 ];
}