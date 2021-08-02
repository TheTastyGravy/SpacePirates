using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManeuverDisplay : MonoBehaviour
{
    public TextMeshPro TimeUntilManeuver;
    public GameObject[] TrackManeuvers;

    private void Awake()
    {
        m_Animation = GetComponent< Animation >();
        TriggerSlideAnimation();
    }

    public void TriggerSlideAnimation()
    {
        m_Animation.Play( "SlideManeuverCards" );
    }

    private void AfterFade()
    {

    }

    private void AfterSlide()
    {

    }

    private Animation m_Animation;
}
