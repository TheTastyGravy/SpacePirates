using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashController : MonoBehaviour
{
    public Image Filter;
    public float FadeDuration;

    void Start()
    {
        StartCoroutine( SplashFade() );
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
        yield return new WaitForSeconds( 0.0f );
    }
}
