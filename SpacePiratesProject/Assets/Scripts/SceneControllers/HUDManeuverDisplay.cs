using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManeuverDisplay : MonoBehaviour
{
    [ Header( "Order should be: Straight, Left90, Left45, Right90, Right45." ) ]
    public Sprite[] TrackManeuverSprites;
    public TextMeshProUGUI TimeUntilManeuver;
    public Image[] ManeuverCards;

    private void Awake()
    {
        m_Animation = GetComponent< Animation >();
        UpdateCards();
    }

    public void TriggerSlide()
    {
        m_Animation.Play( "SlideManeuverCards" );
        m_Animation.Rewind();
    }

    public void UpdateETADisplay( int a_Seconds )
    {
        TimeUntilManeuver.text = ( a_Seconds < 10 ? "0" + a_Seconds.ToString() : a_Seconds.ToString() ) + "s";
    }

    private void UpdateCards()
    {
        int currentTrack = TrackManager.Instance.PlayerShipPosition.trackIndex;
        int trackCount = TrackManager.Instance.track.Length;

        for ( int i = 0; i < 4; ++i )
        {
            int trackType = ++currentTrack < trackCount ? ( int )TrackManager.Instance.track[ currentTrack ] : -1;

            if ( trackType > -1 )
            {
                if ( ManeuverCards[ i ].color.a != 1.0f )
                {
                    Color color = ManeuverCards[ i ].color;
                    color.a = 1.0f;
                    ManeuverCards[ i ].color = color;
                }

                ManeuverCards[ i ].sprite = TrackManeuverSprites[ trackType ];
                ManeuverCards[ i ].enabled = true;
            }
            else
            {
                if ( ManeuverCards[ i ].color.a != 0.0f )
                {
                    Color color = ManeuverCards[ i ].color;
                    color.a = 0.0f;
                    ManeuverCards[ i ].color = color;
                }
                
                ManeuverCards[ i ].enabled = false;
            }
        }
    }

    private void AfterSlide()
    {
        UpdateCards();
    }

    private Animation m_Animation;
}
