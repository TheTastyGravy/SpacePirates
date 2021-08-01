using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton< GameManager >
{
    public static InputActionMap DefaultActionMap
    {
        get
        {
            return Instance.m_DefaultActionAsset.actionMaps.Count > 0 ? Instance.m_DefaultActionAsset.actionMaps[ 0 ] : null;
        }
    }
    public static int SelectedShip
    {
        get
        {
            return Instance.m_SelectedShip;
        }
    }
    public static int SelectedTrack
    {
        get
        {
            return Instance.m_SelectedTrack;
        }
    }
    public static int MaxPlayers
    {
        get
        {
            return Instance.m_MaxPlayers;
        }
    }
    public static GameState CurrentState
    {
        get
        {
            return Instance.m_CurrentState;
        }
        set
        {
            if ( Instance.m_CurrentState == value )
            {
                return;
            }

            if ( Instance.m_CurrentState != GameState.NONE )
            {
                SceneManager.UnloadSceneAsync( Instance.m_CurrentState.ToString() ).completed += asyncOperation => 
                { 
                    SceneManager.LoadSceneAsync( value.ToString(), LoadSceneMode.Additive ).completed += asyncOperation =>
                    {
                        SceneManager.SetActiveScene( SceneManager.GetSceneByName( value.ToString() ) );
                    };

                    Instance.m_CurrentState = value;
                };

                return;
            }
            
            SceneManager.LoadSceneAsync( value.ToString(), LoadSceneMode.Additive ).completed += asyncOperation =>
            {
                SceneManager.SetActiveScene( SceneManager.GetSceneByName( value.ToString() ) );
            };
            
            Instance.m_CurrentState = value;
        }
    }

    private void Start()
    {
        m_SelectedShip = -1;
        m_SelectedTrack = -1;
        CurrentState = GameState.SPLASH;
        PlayerInputManager.instance.onPlayerJoined += player => DontDestroyOnLoad( player );
    }

    public static void RegisterSelectedShip( int a_Index, int a_MaxPlayers )
    {
        Instance.m_SelectedShip = a_Index;
        Instance.m_MaxPlayers = a_MaxPlayers;
    }

    public static void RegisterSelectedTrack( int a_Index )
    {
        Instance.m_SelectedTrack = a_Index;
    }

    private int m_SelectedShip;
    private int m_SelectedTrack;
    private int m_MaxPlayers;
    private GameState m_CurrentState;

    [ SerializeField ] private InputActionAsset m_DefaultActionAsset;

    public enum GameState
    {
        NONE,
        SPLASH,
        START,
        MENU,
        SHIP,
        CHARACTER,
        TRACK,
        GAME,
        SUMMARY
    }
}