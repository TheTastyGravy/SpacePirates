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



    void Awake()
    {
        // Wait a frame
        Invoke(nameof(InitSetup), 0);
    }

    private void InitSetup()
	{
        // Get the music info for the current scene
        musicInfo = data.GetInfo(GameManager.CurrentState);

        timelineInfo = new TimelineInfo();
        timelineInfo.onBeat += OnBeat;
        timelineInfo.onMessage += OnMessage;
        // Explicitly create the delegate object and assign it to a member so it doesn't get freed
        // by the garbage collected while it's being used
        beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
        // Callback used for changing music
        SceneManager.activeSceneChanged += OnSceneChanged;

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
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
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

    private void OnSceneChanged(Scene current, Scene next)
    {
        MusicData.MusicInfo newInfo = data.GetInfo(GameManager.CurrentState);
        if (newInfo != musicInfo)
		{
            StartCoroutine(ChangeMusic(newInfo));
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

        foreach (var obj in beatEvents)
		{
            // The first beat of a bar
            if (timelineInfo.currentMusicBeat == 1)
			{
                obj.barDelay--;
			}

            if (obj.barDelay <= 0 && obj.beatCount <= timelineInfo.currentMusicBeat)
			{
                // Invoke event and remove it from the list
                obj.callback.Invoke();
                beatEvents.Remove(obj);
			}
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
