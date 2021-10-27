using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : Singleton<MusicManager>
{
    public MusicData data;
    public bool printBeatLogs = false;
    public int CurrentBar => timelineInfo.currentMusicBar;
    public int CurrentBeat => timelineInfo.currentMusicBeat;
    
    private class TimelineInfo
    {
        public int currentMusicBar = 0;
        public int currentMusicBeat = 0;
        public float tempo = 0;
        public int upperSignature = 4;  // Beats per bar
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();

        public BasicDelegate onBeat;
        public BasicDelegate onMessage;
    }
    private TimelineInfo timelineInfo;
    private GCHandle timelineHandle;
    private FMOD.Studio.EVENT_CALLBACK beatCallback;
    private FMOD.Studio.EventInstance musicInstance;
    private class Entry
	{
        public BasicDelegate callback;
        public int barDelay;
        public int beatCount;
	}
    private List<Entry> beatEvents = new List<Entry>();

    private MusicData.MusicInfo musicInfo;
    private bool inGameScene = false;
    private bool setIntensity = true;
    private string lastEvent = "";

    private float musicVolume = 100;

    private float baseFadeTime;

    [Header("Intensity")]
    public float maxOxygenDrainRate = 10;
    [Range(0,1)]
    public float oxygenDrainFactor = 0.5f;
    [Range(0,1)]
    public float speedFactor = 0.5f;
    [Range(0,1)]
    public float progressFactor = 0.5f;



    void Awake()
    {
        // Wait a frame
        Invoke(nameof(InitSetup), 0);
    }

    private void InitSetup()
	{
        baseFadeTime = GameManager.Instance.fadeInTime;

        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 100) * 0.01f;
        // Get the music info for the current scene
        musicInfo = data.GetInfo(GameManager.CurrentState);

        timelineInfo = new TimelineInfo();
        timelineInfo.onBeat += OnBeat;
        timelineInfo.onMessage += OnMessage;
        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
        // Callback used for changing music
        GameManager.OnStartTransition += OnSceneExit;
        GameManager.OnEndTransition += OnSceneEnter;

        SetupMusic();
    }

    private void SetupMusic()
	{
        musicInstance = FMODUnity.RuntimeManager.CreateInstance(musicInfo.musicEvent);

        // Pin the class that will store the data modified during the callback
        timelineHandle = GCHandle.Alloc(timelineInfo);
        // Pass the object through the userdata of the instance. This allows the callback to access our variables
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
        // Setup callback for beats and markers
        musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        if (enabled)
        {
            musicInstance.start();
        }
        musicInstance.setVolume(musicVolume);
    }

    void OnDestroy()
    {
        GameManager.OnStartTransition -= OnSceneExit;
        GameManager.OnEndTransition -= OnSceneEnter;
        musicInstance.setUserData(IntPtr.Zero);
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
        timelineHandle.Free();
    }

	void OnEnable()
	{
        if (musicInstance.isValid())
		{
            musicInstance.start();
        }
    }

    void OnDisable()
	{
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void OnSceneExit(Scene scene, GameManager.GameState otherScene)
	{
        // Make sure music has enough time to fade out
        GameManager.Instance.fadeInTime = Mathf.Max(baseFadeTime, musicInfo.fadeTime);

        MusicData.MusicInfo newInfo = data.GetInfo(otherScene);
        if (newInfo != musicInfo)
        {
            StartCoroutine(ChangeMusic(newInfo));
            if (scene.name == "GAME")
			{
                EventManager.Instance.OnEventChange -= OnEventChange;
                inGameScene = false;
            }
        }
    }

    private void OnSceneEnter(Scene scene, GameManager.GameState otherScene)
	{
        // We have entered the game scene
        if (scene.name == "GAME")
		{
            // Add callback next frame to ensure its not null
            IEnumerator NextFrame()
			{
                yield return null;
                EventManager.Instance.OnEventChange += OnEventChange;
            }
            StartCoroutine(NextFrame());
            inGameScene = true;
            setIntensity = true;
            // TEMP
            musicInstance.setParameterByName("Intensity", 1);
        }
	}

    private IEnumerator ChangeMusic(MusicData.MusicInfo newMusicInfo)
	{
        // Stop with fade
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        yield return new WaitForSeconds(musicInfo.fadeTime);

        // Hard stop and destroy everything
        musicInstance.setUserData(IntPtr.Zero);
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance.release();
        timelineHandle.Free();
        // Setup using the new info
        musicInfo = newMusicInfo;
        SetupMusic();
    }

    private void OnEventChange(Level.Event.Type eventType)
	{
        string name = eventType switch
        {
            Level.Event.Type.AstroidField => "Asteroids",
            Level.Event.Type.PlasmaStorm => "Plasma Storm",
            Level.Event.Type.ShipAttack => "The Fuzz",
            _ => "",
        };


        StopAllCoroutines();
        if (name == "")
		{
            StartCoroutine(FadeEvent(lastEvent, false, 2, 1));
            lastEvent = "";
        }
        else
		{
            StartCoroutine(FadeEvent(name, true, 1, 1));
            lastEvent = name;
        }
    }

    private IEnumerator FadeEvent(string paramName, bool fadeInParam, int fadeInBars, int fadeOutBars, float endIntensity = 1)
	{
        if (paramName == "")
            yield break;
		
        // Get the current intensity value
        FMOD.RESULT result = musicInstance.getParameterByName("Intensity", out float start);
        if (result != FMOD.RESULT.OK)
		{
            start = 1;
        }


        setIntensity = false;
        // Convert BPM to seconds per bar
        float barTime = 1f / (timelineInfo.tempo / (60 * timelineInfo.upperSignature));

        //  ----- Fade out -----
        float fadeOutTime = barTime * (fadeOutBars - 1 + (timelineInfo.upperSignature + 1 - timelineInfo.currentMusicBeat) / (float)timelineInfo.upperSignature);
        float t = 0;
        while (t < fadeOutTime)
        {
            float val = t / fadeOutTime;
            musicInstance.setParameterByName("Intensity", Mathf.Lerp(start, 0, val));
            if (!fadeInParam)
                musicInstance.setParameterByName(paramName, Mathf.Lerp(1, 0, val));

            t += Time.deltaTime;
            yield return null;
        }
        if (!fadeInParam)
            musicInstance.setParameterByName(paramName, 0);
        
        //  ----- Fade in -----
        float fadeInTime = barTime * fadeInBars;
        t = 0;
        while (t < fadeInTime)
        {
            float val = t / fadeInTime;
            musicInstance.setParameterByName("Intensity", Mathf.Lerp(0, endIntensity, val));
            if (fadeInParam)
                musicInstance.setParameterByName(paramName, Mathf.Lerp(0, 1, val));

            t += Time.deltaTime;
            yield return null;
        }
        musicInstance.setParameterByName("Intensity", endIntensity);
        if (fadeInParam)
            musicInstance.setParameterByName(paramName, 1);

        setIntensity = true;
    }

	void Update()
	{
        if (!inGameScene)
            return;

        musicInstance.setParameterByName("Oxygen Low", 1 - (ShipManager.Instance.OxygenLevel / ShipManager.Instance.maxOxygenLevel));

        if (setIntensity)
		{
            float oxygenLoss = Mathf.Max(-ShipManager.Instance.oxygenDrain, 0) / maxOxygenDrainRate;
            float shipSpeed = ShipManager.Instance.GetShipSpeed() / ShipManager.Instance.GetMaxSpeed();
            float progress = LevelController.Instance.PlayerPosition / LevelController.Instance.level.length;

            float intensity = oxygenLoss * oxygenDrainFactor + shipSpeed * speedFactor + progress * progressFactor;
            musicInstance.setParameterByName("Intensity", intensity);
        }
    }

    public void SetVolume(float value)
	{
        // Convert from 0-100 to 0-1
        musicVolume = value * 0.01f;
        if (musicInstance.setVolume(musicVolume) != FMOD.RESULT.OK)
		{
            Debug.LogWarning("Failed to set music volume");
		}
    }


    /// <summary>
    /// Add a callback to be called on a music beat
    /// </summary>
    /// <param name="barDelay">How many bars should pass before invoking</param>
    /// <param name="beatCount">What beat to invoke on, starting at 1. If barDelay = 0 and the beat has passed, the next beat will be used</param>
    public void AddBeatEvent(BasicDelegate callback, int barDelay = 0, int beatCount = 1)
	{
        Entry entry = new Entry()
        {
            callback = callback,
            barDelay = barDelay,
            beatCount = beatCount
        };
        beatEvents.Add(entry);
    }

    private void OnBeat()
	{
        if (printBeatLogs)
		{
            Debug.Log("\tCurrent Bar: " + timelineInfo.currentMusicBar + "\n\t\tCurrent Beat: " + timelineInfo.currentMusicBeat);
        }

        List<Entry> used = new List<Entry>();
        foreach (var obj in beatEvents)
		{
            // The first beat of a bar
            if (timelineInfo.currentMusicBeat == 1)
			{
                obj.barDelay--;
			}

            if (obj.barDelay <= 0 && obj.beatCount <= timelineInfo.currentMusicBeat)
			{
                // Invoke event and mark it for removal
                obj.callback.Invoke();
                used.Add(obj);
			}
		}
        foreach(var obj in used)
		{
            beatEvents.Remove(obj);
        }
	}

    private void OnMessage()
	{
        // A message has occured in the timeline. Check timelineInfo.lastMarker
    }


    // Callback method for timeline events
    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        timelineInfo.currentMusicBar = parameter.bar;
                        timelineInfo.currentMusicBeat = parameter.beat;
                        timelineInfo.tempo = parameter.tempo;
                        timelineInfo.upperSignature = parameter.timesignatureupper;
                        timelineInfo.onBeat.Invoke();
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.lastMarker = parameter.name;
                        timelineInfo.onMessage.Invoke();
                    }
                    break;
            }
        }
        return FMOD.RESULT.OK;
    }
}
