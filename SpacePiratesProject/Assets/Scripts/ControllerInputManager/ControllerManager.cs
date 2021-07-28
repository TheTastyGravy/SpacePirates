using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class ControllerManager : Singleton< ControllerManager >
{
    public static int ControllerCount
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
        m_ParentPlayers = m_ParentPlayers ?? transform.Find( "Players" );

        if ( m_ParentPlayers == null )
        {
            GameObject newParentPlayers = new GameObject( "Players" );
            newParentPlayers.transform.parent = transform;
        }

        m_Players = new List< IPlayer >();
    }
    
    public static IPlayer RetrievePlayer( IPlayer.PlayerSlot a_PlayerSlot )
    {
        //return Instance.m_Players[ ( int )a_PlayerSlot ];
        return PlayerInput.GetPlayerByIndex( ( int )a_PlayerSlot ) as IPlayer;
    }

    private void OnPlayerJoined( PlayerInput a_PlayerInput )
    {
        m_Players.Add( a_PlayerInput as IPlayer );
        a_PlayerInput.gameObject.name = "Player" + m_Players.Count;
    }

    private void OnPlayerLeft( PlayerInput a_PlayerInput )
    {
        m_Players.Remove( a_PlayerInput as IPlayer );

        int index = 0;

        foreach ( IPlayer player in m_Players )
        {
            player.gameObject.name = "Player" + ++index;
        }

        if ( PlayerInput.all.Count < GameManager.MaxPlayers )
        {
            PlayerInputManager.instance.EnableJoining();
        }
    }

    private List< IPlayer > m_Players;
    private Transform m_ParentPlayers;
}