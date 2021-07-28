using UnityEngine.SceneManagement;

public class GameManager : Singleton< GameManager >
{
    public static int SelectedShip
    {
        get
        {
            return Instance.m_SelectedShip;
        }
    }
    public static int SelectedMap
    {
        get
        {
            return Instance.m_SelectedMap;
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
                    SceneManager.LoadSceneAsync( value.ToString(), LoadSceneMode.Additive ); 
                    Instance.m_CurrentState = value; 
                };

                return;
            }
            
            SceneManager.LoadSceneAsync( value.ToString(), LoadSceneMode.Additive );
            Instance.m_CurrentState = value;
        }
    }

    private void Start()
    {
        m_SelectedShip = -1;
        m_SelectedMap = -1;
        
        CurrentState = GameState.SPLASH;
    }

    public static void RegisterSelectedShip( int a_Index, int a_MaxPlayers )
    {
        Instance.m_SelectedShip = a_Index;
        Instance.m_MaxPlayers = a_MaxPlayers;
    }

    public static void RegisterSelectedMap( int a_Index )
    {
        Instance.m_SelectedMap = a_Index;
    }

    private int m_SelectedShip;
    private int m_SelectedMap;
    private int m_MaxPlayers;
    private GameState m_CurrentState;

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

    // summary, play again, play, return,

    // options, sound, video,
    // controls,
    // exit,
    // play
}