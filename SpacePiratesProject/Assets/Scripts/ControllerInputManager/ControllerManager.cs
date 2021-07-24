using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class ControllerManager : Singleton< ControllerManager >
{
    public int ControllerCount
    {
        get
        {
            return Gamepad.all.Count;
        }
    }

    private void Start()
    {
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        PlayerInputManager.instance.onPlayerLeft += OnPlayerLeft;
    }
    
    public static Player RetrievePlayer( Player.PlayerSlot a_PlayerSlot )
    {
        return Instance.m_Players[ ( int )a_PlayerSlot ];
    }

    private void OnPlayerJoined( PlayerInput a_PlayerInput )
    {
        
    }

    private void OnPlayerLeft( PlayerInput a_PlayerInput )
    {

    }

    private Player[] m_Players = new Player[ 4 ];
}