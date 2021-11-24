using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;
using FMODUnity;

public class SplashController : Singleton< SplashController >
{
    public GameObject logoObject;
    public float logoTime = 1;
    public Image Filter;
    public float FadeDuration;
    [Space]
    public EventReference videoMusicEvent;
    public float musicLength = 33;
    public VideoPlayer videoPlayer;

    private FMOD.Studio.EventInstance videoMusicInstance;



    void Start()
    {
        StartCoroutine( SplashFade() );
        InputSystem.onEvent += OnInput;
        // Setup music instance
        videoMusicInstance = RuntimeManager.CreateInstance(videoMusicEvent);
        videoMusicInstance.set3DAttributes(RuntimeUtils.To3DAttributes(Vector3.zero));
    }

	void OnDestroy()
	{
        InputSystem.onEvent -= OnInput;
        videoMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        videoMusicInstance.release();
    }

	private void OnInput( UnityEngine.InputSystem.LowLevel.InputEventPtr _, InputDevice a_InputDevice )
    {
        // Skip splash with E key or A button
        if (a_InputDevice is Keyboard && (a_InputDevice as Keyboard).eKey.isPressed || 
            a_InputDevice is Gamepad && (a_InputDevice as Gamepad).aButton.isPressed)
		{
            GameManager.ChangeState(GameManager.GameState.START);
        }
    }

    private IEnumerator SplashFade()
    {
        yield return Fade(false);
        yield return new WaitForSeconds(logoTime);
        yield return Fade(true);

        logoObject.SetActive(false);
        yield return PlayVideo();

        yield return new WaitForSeconds(0.5f);
        GameManager.ChangeState(GameManager.GameState.START);
    }

    private IEnumerator PlayVideo()
    {
        // Wait for the video to start playing
        videoPlayer.Play();
        yield return new WaitWhile(() => videoPlayer.time <= 0);
        // Start fading in
        StartCoroutine(Fade(false));

        // Make the video use the time assigned to it
        videoPlayer.timeReference = VideoTimeReference.ExternalTime;
        float startTime = Time.time;
        videoMusicInstance.start();
        while (Time.time < startTime + musicLength)
        {
            FMOD.RESULT res = videoMusicInstance.getTimelinePosition(out int time);
            if (res == FMOD.RESULT.OK)
            {
                // Convert from milliseconds to seconds
                float musicTime = time * 0.001f;
                videoPlayer.externalReferenceTime = musicTime;
            }
            yield return null;
        }

        // Stop the music and until video is about to end
        videoMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        yield return new WaitWhile(() => Time.time < startTime + videoPlayer.length - FadeDuration * 0.5f);
        // Fade to black
        yield return Fade(true);
    }

    private IEnumerator Fade(bool toBlack)
    {
        float durationInverse = 1f / FadeDuration;
        float time = 0;
        while (time < FadeDuration * 0.5f)
        {
            yield return null;
            time += Time.deltaTime;
            float alpha = 1.0f - Mathf.Sin((time + (toBlack ? FadeDuration * 0.5f : 0)) * durationInverse * Mathf.PI);
            Filter.color = new Color(0.0f, 0.0f, 0.0f, alpha);
        }
    }
}
