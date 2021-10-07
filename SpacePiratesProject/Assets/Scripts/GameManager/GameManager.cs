using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton< GameManager >
{
    public float fadeInTime = 0.5f;
    public float fadeOutTime = 0.5f;

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
                // When moving from the game scene, move players to DontDestroyOnLoad so inputs can still be used
                if (Instance.m_CurrentState == GameState.GAME)
				{
                    foreach (var playerInput in PlayerInput.all)
					{
                        Player player = playerInput as Player;
                        player.Character.gameObject.SetActive(false);
                        player.Character.enabled = false;
                        player.transform.parent = null;
                        DontDestroyOnLoad(player.gameObject);
                    }
				}

                // Make the INIT scene active for now
                SceneManager.SetActiveScene(Instance.gameObject.scene);

                // This will do the transition with a fade, but kind of looks crap...
                //Instance.StartCoroutine(LoadUnload(Instance.m_CurrentState, value));
                // ...so just keep doing it like this instead
                GameState oldState = Instance.m_CurrentState;
                SceneManager.LoadSceneAsync(value.ToString(), LoadSceneMode.Additive).completed += asyncOperation =>
                {
                    SceneManager.UnloadSceneAsync(oldState.ToString());
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(value.ToString()));
                };
                Instance.m_CurrentState = value;
            }
			else
			{
                SceneManager.LoadSceneAsync(value.ToString(), LoadSceneMode.Additive).completed += asyncOperation =>
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(value.ToString()));
                };
                Instance.m_CurrentState = value;
            }
        }
    }

    private static IEnumerator LoadUnload(GameState oldState, GameState newState)
	{
        // Start loading the new scene, then wait for fade
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(newState.ToString(), LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;
        shouldFade = true;
        yield return new WaitForSeconds(Instance.fadeInTime);

        // Finish loading and start unloading, then wait for them to finish
        SceneManager.UnloadSceneAsync(oldState.ToString());
        loadOp.completed += asyncOperation =>
        {
            // Fade in and set active scene
            shouldFade = false;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(newState.ToString()));
        };
        loadOp.allowSceneActivation = true;
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

	void OnGUI()
	{
        // We dont need to draw anything
        if (!shouldFade && fadeTimePassed < 0)
            return;

        // State has changed, so we need to update fadeTimePassed
        if (wasFading != shouldFade)
        {
            fadeTimePassed = shouldFade ? 0 : fadeOutTime;
            wasFading = shouldFade;
        }
        fadeTimePassed += UnityEngine.Time.deltaTime * (shouldFade ? 1 : -1);
        float alpha = fadeTimePassed / (shouldFade ? fadeInTime : fadeOutTime);

        // Fade texture using alpha and draw it over the screen
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, new Color(0, 0, 0, alpha));
        texture.Apply();
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
    }

    private int m_SelectedShip;
    private int m_SelectedTrack;
    private int m_MaxPlayers;
	private int m_Placement;
	private float m_Time;
	private bool m_HasFinished;
    private GameState m_CurrentState;

    private static bool shouldFade = false;
    private bool wasFading = false;
    private float fadeTimePassed = 0;

    [ SerializeField ] private InputActionAsset m_DefaultActionAsset;

    public enum GameState
    {
        NONE = 0,
        SPLASH = 1,
        START = 2,
        MENU = 4,
        SHIP = 8,
        CHARACTER = 16,
        TRACK = 32,
        GAME = 64,
        SUMMARY = 128
    }
}