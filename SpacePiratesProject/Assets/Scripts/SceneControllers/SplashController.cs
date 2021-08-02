using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SplashController : Singleton< SplashController >
{
    public Image Filter;
    public float FadeDuration;

    void Start()
    {
        StartCoroutine( SplashFade() );
        InputSystem.onEvent += OnInput;
    }

    private void OnInput( UnityEngine.InputSystem.LowLevel.InputEventPtr _, InputDevice a_InputDevice )
    {
        if ( a_InputDevice is Mouse )
        {
            return;
        }

        InputSystem.onEvent -= OnInput;
        GameManager.CurrentState = GameManager.GameState.START;
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

        yield return Splash();

        while ( elapsedTime < FadeDuration )
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            float alpha = 1.0f - Mathf.Sin( elapsedTime * durationInverse * Mathf.PI );
            Filter.color = new Color( 0.0f, 0.0f, 0.0f, alpha );
        }

        GameManager.CurrentState = GameManager.GameState.START;
    }

    private IEnumerator Splash()
    {
        yield return new WaitForSeconds( 2.0f );
    }
}
