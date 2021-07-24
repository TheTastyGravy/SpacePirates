using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : Singleton< GameManager >
{
    public int SelectedShip
    {
        get
        {
            return m_SelectedShip;
        }
    }
    public int PlayerCount
    {
        get
        {
            return m_RegisteredPlayers.Length;
        }
    }
    public int SelectedMap
    {
        get
        {
            return m_SelectedMap;
        }
    }
    public GameState CurrentState
    {
        get
        {
            return m_CurrentState;
        }
    }

    private void Start()
    {
        m_SelectedShip = -1;
        m_SelectedMap = -1;
        
        SwitchToState( GameState.SPLASH );
    }

    public void SwitchToState( GameState a_GameState )
    {
        if ( m_CurrentState == a_GameState )
        {
            return;
        }

        if ( m_CurrentState != GameState.NONE )
        {
            SceneManager.UnloadSceneAsync( m_CurrentState.ToString() ).completed += asyncOperation => 
            { 
                SceneManager.LoadSceneAsync( a_GameState.ToString(), LoadSceneMode.Additive ); 
                m_CurrentState = a_GameState; 
            };

            return;
        }
        
        SceneManager.LoadSceneAsync( a_GameState.ToString(), LoadSceneMode.Additive );
        m_CurrentState = a_GameState;
    }

    public void RegisterSelectedShip( int a_Index )
    {
        m_SelectedShip = a_Index;
    }

    public void RegisterSelectedMap( int a_Index )
    {
        m_SelectedMap = a_Index;
    }

    public Player GetPlayer( Player.PlayerSlot a_PlayerSlot )
    {
        if ( ( int )a_PlayerSlot >= m_RegisteredPlayers.Length )
        {
            return null;
        }

        return m_RegisteredPlayers[ ( int )a_PlayerSlot ];
    }

    private int m_SelectedShip;
    private int m_SelectedMap;
    private Player[] m_RegisteredPlayers;
    private GameState m_CurrentState;

    public enum GameState
    {
        NONE,
        SPLASH,
        START,
        MAIN,
        SHIP,
        CHARACTER,
        TRACK,
        GAME,
        SUMMARY
    }

    // summary, play again, play, return,

    // options, sound, video,
    // controls,
    // exit,
    // play
}