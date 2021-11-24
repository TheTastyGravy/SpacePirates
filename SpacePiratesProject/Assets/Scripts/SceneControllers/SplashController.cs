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
        float durationInverse = 1.0f / FadeDuration;
        float elapsedTime = 0.0f;

        while ( elapsedTime < FadeDuration * 0.5f )
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            float alpha = 1.0f - Mathf.Sin( elapsedTime * durationInverse * Mathf.PI );
            Filter.color = new Color( 0.0f, 0.0f, 0.0f, alpha );
        }

        yield return new WaitForSeconds(logoTime);

        while ( elapsedTime < FadeDuration )
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            float alpha = 1.0f - Mathf.Sin( elapsedTime * durationInverse * Mathf.PI );
            Filter.color = new Color( 0.0f, 0.0f, 0.0f, alpha );
        }

        logoObject.SetActive(false);
        // Dont start the music untill the video has actualy started
        videoPlayer.Play();
        yield return new WaitWhile(() => videoPlayer.time <= 0);
        videoMusicInstance.start();
        float startTime = Time.time;
        elapsedTime = 0;

        while (elapsedTime < FadeDuration  * 0.5f)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            float alpha = 1.0f - Mathf.Sin(elapsedTime * durationInverse * Mathf.PI);
            Filter.color = new Color(0.0f, 0.0f, 0.0f, alpha);
        }

        yield return new WaitWhile(() => Time.time < startTime + musicLength);
        videoMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        yield return new WaitWhile(() => Time.time < startTime + videoPlayer.length - FadeDuration * 0.5f);

        while (elapsedTime < FadeDuration)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            float alpha = 1.0f - Mathf.Sin(elapsedTime * durationInverse * Mathf.PI);
            Filter.color = new Color(0.0f, 0.0f, 0.0f, alpha);
        }

        GameManager.ChangeState(GameManager.GameState.START);
    }
}
