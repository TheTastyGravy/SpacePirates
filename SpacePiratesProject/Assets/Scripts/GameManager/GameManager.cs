using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public LevelDificultyData diffData;
    [Space]
    public float fadeInGame = 0.5f;
    public float fadeOutGame = 0.5f;
    [Space]
    public float fadeInOther = 0.5f;
    public float fadeOutOther = 0.5f;
    [HideInInspector]
    public float realFadeIn;
    [HideInInspector]
    public float realFadeOut;

    public delegate void SceneDelegate(Scene scene, GameState otherScene);
    public static SceneDelegate OnStartTransition;
    public static SceneDelegate OnEndTransition;

    public static InputActionMap DefaultActionMap => Instance.m_DefaultActionAsset.actionMaps.Count > 0 ? Instance.m_DefaultActionAsset.actionMaps[0] : null;
    public static int SelectedShip => Instance.m_SelectedShip;
    public static int SelectedTrack => Instance.m_SelectedTrack;
    public static int MaxPlayers => Instance.m_MaxPlayers;
    public static float Time => Instance.m_Time;
    public static bool HasWon => Instance.m_HasWon;
    public static bool IsLoadingScene => Instance.m_IsLoadingScene;
    public static GameState CurrentState => Instance.m_CurrentState;
    public static LevelDificultyData DiffData => Instance.diffData;



    public static void ChangeState(GameState newState, bool overrideStored = false)
	{
        if (Instance.m_CurrentState == newState)
            return;
        
        // Prevent changing scenes too quickly to allow start logic to run
        if (Instance.m_IsLoadingScene)
        {
            string message = "Attempting to change scene while still loading";
            if ((CurrentState != GameState.GAME && CurrentState != GameState.LOADING && Instance.storedState == GameState.NONE) || overrideStored)
			{
                message += "\n<color=green>State has been stored</color>";
                Instance.storedState = newState;
            }
            Debug.LogWarning(message);
            return;
        }
        Instance.m_IsLoadingScene = true;

        // Set fade times
        Instance.realFadeIn = newState == GameState.GAME ? Instance.fadeInGame : Instance.fadeInOther;
        Instance.realFadeOut = newState == GameState.GAME ? Instance.fadeOutGame : Instance.fadeOutOther;

        if (Instance.m_CurrentState != GameState.NONE)
        {
            if (OnStartTransition != null)
                OnStartTransition.Invoke(SceneManager.GetSceneByName(Instance.m_CurrentState.ToString()), newState);

            // When moving from the game scene, move players to DontDestroyOnLoad so inputs can still be used
            if (Instance.m_CurrentState == GameState.GAME)
            {
                foreach (PlayerInput playerInput in PlayerInput.all)
                {
                    Player player = playerInput as Player;
                    player.Character.gameObject.SetActive(false);
                    player.Character.enabled = false;
                    player.transform.parent = null;
                    DontDestroyOnLoad(player.gameObject);
                }
            }
            if (newState == GameState.GAME || newState == GameState.LOADING)
			{
                (Player.GetPlayerBySlot(Player.PlayerSlot.P1).Character as Character).ResetCheat();
                Instance.m_Time = 0;
                Instance.m_ShouldTrackTime = true;  //move this when intro is added
                // Use a fade transition when entering the game scene because it has a longer load
                Instance.StartCoroutine(LoadUnload(Instance.m_CurrentState, newState));
                Instance.m_CurrentState = newState;
                return;
            }

            // Make the INIT scene active for now
            SceneManager.SetActiveScene(Instance.gameObject.scene);

            // If we are loading a menu and the menu base isnt loaded, load it imediatly
            if (IsMenuScene(newState) && !SceneManager.GetSceneByName("MENU_BASE").IsValid())
			{
                SceneManager.LoadSceneAsync("MENU_BASE", LoadSceneMode.Additive).priority = 100;
			}

            RegularLoad(Instance.m_CurrentState, newState);
        }
        else
        {
            SceneManager.LoadSceneAsync(newState.ToString(), LoadSceneMode.Additive).completed += asyncOperation =>
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(newState.ToString()));
                if (OnEndTransition != null)
                    OnEndTransition.Invoke(SceneManager.GetSceneByName(newState.ToString()), GameState.NONE);
                Instance.Invoke(nameof(SetLoading), 0.01f);
            };
        }

        Instance.m_CurrentState = newState;
    }

    private static void RegularLoad(GameState oldState, GameState newState)
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(newState.ToString(), LoadSceneMode.Additive);
        loadOp.completed += asyncOperation =>
        {
            Scene newScene = SceneManager.GetSceneByName(newState.ToString());
            SceneManager.SetActiveScene(newScene);

            GameObject[] newSceneObjects = newScene.GetRootGameObjects();
            foreach (GameObject obj in newSceneObjects)
            {
                obj.SetActive(false);
            }

            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(oldState.ToString());
            unloadOp.completed += asyncOperation =>
            {
                foreach (GameObject obj in newSceneObjects)
                {
                    obj.SetActive(true);
                }
                
                if (OnEndTransition != null)
                    OnEndTransition.Invoke(SceneManager.GetSceneByName(newState.ToString()), oldState);
                Instance.Invoke(nameof(SetLoading), 0.01f);
            };
        };
    }

    public static bool IsMenuScene(GameState state)
	{
        return state == GameState.START ||
                state == GameState.MENU ||
                state == GameState.SHIP ||
                state == GameState.CHARACTER ||
                state == GameState.TRACK ||
                state == GameState.SUMMARY;
	}

    public static void ReloadScene()
    {
        Instance.realFadeIn = CurrentState == GameState.GAME ? Instance.fadeInGame : Instance.fadeInOther;
        Instance.realFadeOut = CurrentState == GameState.GAME ? Instance.fadeOutGame : Instance.fadeOutOther;
        if (OnStartTransition != null)
            OnStartTransition.Invoke(SceneManager.GetSceneByName(Instance.m_CurrentState.ToString()), Instance.m_CurrentState);

        foreach (PlayerInput playerInput in PlayerInput.all)
        {
            Player player = playerInput as Player;
            player.Character.gameObject.SetActive(false);
            player.Character.enabled = false;
            player.transform.parent = null;
            DontDestroyOnLoad(player.gameObject);
        }
        if (Instance.m_CurrentState == GameState.GAME)
            (Player.GetPlayerBySlot(Player.PlayerSlot.P1).Character as Character).ResetCheat();

        Instance.m_IsLoadingScene = true;
        Instance.StartCoroutine(Reload(Instance.m_CurrentState));
    }

    private void SetLoading()
	{
        m_IsLoadingScene = false;
        UnityEngine.Time.timeScale = 1;

        if (storedState != GameState.NONE)
		{
            ChangeState(storedState);
            storedState = GameState.NONE;
        }
    }

    private static IEnumerator LoadUnload(GameState oldState, GameState newState)
	{
        // Start loading the new scene, then wait for fade
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(newState.ToString(), LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;
        shouldFade = true;
        yield return new WaitForSecondsRealtime(Instance.realFadeIn);

        // Fades are only used for transitioning to non-menu scenes (currently...)
        if (SceneManager.GetSceneByName("MENU_BASE").IsValid())
		{
            SceneManager.UnloadSceneAsync("MENU_BASE").priority = 100;
		}

        SceneManager.SetActiveScene(Instance.gameObject.scene);
        // Finish loading and start unloading, then wait for them to finish
        SceneManager.UnloadSceneAsync(oldState.ToString());
        loadOp.completed += asyncOperation =>
        {
            // Fade in and set active scene
            shouldFade = false;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(newState.ToString()));
            if (OnEndTransition != null)
                OnEndTransition.Invoke(SceneManager.GetSceneByName(newState.ToString()), oldState);
            Instance.Invoke(nameof(SetLoading), 0.01f);
        };
        loadOp.allowSceneActivation = true;
    }

    private static IEnumerator Reload(GameState state)
	{
        shouldFade = true;
        yield return new WaitForSecondsRealtime(Instance.realFadeIn);
        SceneManager.SetActiveScene(Instance.gameObject.scene);

        // Because we need to finish unloading before we finish loading, the unload operation needs 
        // to be started first so the load operation can have allowSceneActivation = false. This 
        // is because allowSceneActivation will stop all AsyncOperations that come after it once 
        // it reaches 0.9 compleation.
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(state.ToString()));
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(state.ToString(), LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;

        unloadOp.completed += asyncOperation =>
        {
            loadOp.allowSceneActivation = true;
        };
        loadOp.completed += asyncOperation =>
        {
            shouldFade = false;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(state.ToString()));
            if (OnEndTransition != null)
                OnEndTransition.Invoke(SceneManager.GetSceneByName(state.ToString()), state);
            Instance.Invoke(nameof(SetLoading), 0.01f);
        };
    }

    private void Start()
    {
        m_SelectedShip = -1;
        m_SelectedTrack = -1;
        ChangeState(GameState.SPLASH);
        PlayerInputManager.instance.onPlayerJoined += player => DontDestroyOnLoad( player );

        // Only allow controllers in builds
        if (!Application.isEditor)
            InputSystem.settings.supportedDevices = new string[] { "Gamepad" };
    }

    public static LevelDificultyData.DiffSetting GetDifficultySettings()
	{
        return Instance.diffData.GetSetting((LevelDificultyData.Difficulty)SelectedTrack, Player.all.Count);
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

    public static void SetGameOverInfo(bool hasWon)
	{
        Instance.m_HasWon = hasWon;
        Instance.m_ShouldTrackTime = false;
    }

	void OnGUI()
	{
        // We dont need to draw anything
        if (!shouldFade && fadeTimePassed < 0)
            return;

        // State has changed, so we need to update fadeTimePassed
        if (wasFading != shouldFade)
        {
            fadeTimePassed = shouldFade ? 0 : realFadeOut;
            wasFading = shouldFade;
        }
        fadeTimePassed += UnityEngine.Time.unscaledDeltaTime * (shouldFade ? 1 : -1);
        float alpha = fadeTimePassed / (shouldFade ? realFadeIn : realFadeOut);

        // Fade texture using alpha and draw it over the screen
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, new Color(0, 0, 0, alpha));
        texture.Apply();
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
    }

	void Update()
	{
        // While in the game scene, track the time
		if (m_CurrentState == GameState.GAME && !m_IsLoadingScene && m_ShouldTrackTime)
		{
            m_Time += UnityEngine.Time.deltaTime;
        }
	}

	private int m_SelectedShip;
    private int m_SelectedTrack;
    private int m_MaxPlayers;
	private float m_Time;
	private bool m_HasWon;
    private GameState m_CurrentState;
    private bool m_IsLoadingScene = false;
    private bool m_ShouldTrackTime = false;

    private GameState storedState = GameState.NONE;

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
        LOADING = 64,
        GAME = 128,
        SUMMARY = 256
    }
}