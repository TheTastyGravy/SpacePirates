using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton< T > : MonoBehaviour where T : Singleton< T >
{
    public static T Instance
    {
        get
        {
            if ( m_Instance == null )
            {
                m_Instance = FindObjectOfType< T >();
                DontDestroyOnLoad( m_Instance.gameObject );
            }

            return m_Instance;
        }
    }
    private static T m_Instance;
}