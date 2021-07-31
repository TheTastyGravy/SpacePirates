using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int PlayersNearby
    {
        get
        {
            return m_PlayersNearby;
        }
        private set
        {
            int beforePlayersNearby = m_PlayersNearby;
            m_PlayersNearby = value;

            if ( beforePlayersNearby == 0 && m_PlayersNearby != 0 )
            {
                Open();
            }
            else if ( beforePlayersNearby != 0 && m_PlayersNearby == 0 )
            {
                Close();
            }
        }
    }

    private void Awake()
    {
        m_DoorAnimation = GetComponent< Animation >();
    }

    private void OnTriggerEnter( Collider a_Other )
    {
        if ( a_Other.gameObject.TryGetComponent( out Character _ ) )
        {
            ++PlayersNearby;
        }
    }

    private void OnTriggerExit( Collider a_Other )
    {
        if ( a_Other.gameObject.TryGetComponent( out Character _ ) )
        {
            --PlayersNearby;
        }
    }

    public void Open() => m_DoorAnimation.Play( "Open" );

    public void Close() => m_DoorAnimation.Play( "Close" );

    private int m_PlayersNearby;
    private Animation m_DoorAnimation;
}
