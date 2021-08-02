using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton< GameManager >
{
    public static InputActionMap DefaultActionMap => Instance.m_DefaultActionAsset.actionMaps.Count > 0 ? Instance.m_DefaultActionAsset.actionMaps[0] : null;
    public static int SelectedShip => Instance.m_SelectedShip;
	public static int SelectedTrack => Instance.m_SelectedTrack;
	public static int MaxPlayers => Instance.m_MaxPlayers;
	public static int Placement => Instance.m_Placement;
	public static float Time => Instance.m_Time;
	public static bool HasFinished => Instance.m_HasFinished;
    
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

	public static void RegisterFinalGameState( bool a_HasFinished, int a_Placement, float a_Time )
	{
		Instance.m_Placement = a_Placement;
		Instance.m_Time = a_Time;
		Instance.m_HasFinished = a_HasFinished;
	}

    private int m_SelectedShip;
    private int m_SelectedTrack;
    private int m_MaxPlayers;
	private int m_Placement;
	private float m_Time;
	private bool m_HasFinished;
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

    // To do:
    // Skip splash screen
    // Character wrap around
    // Level loadin
    // Update maneuver cards to show current and time foldout
    // Video
    // Edit slide
    // Mixamo
}