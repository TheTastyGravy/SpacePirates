using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ CreateAssetMenu( fileName = "NewLevel", menuName = "", order = 1 ) ]
public class Level : ScriptableObject
{
    public float length = 100;
    public Event[] events;

    [Serializable]
    public class Event
	{
        public float start, end;
        public Type type;

        public enum Type
		{
            AstroidField,
            PlasmaStorm,
            ShipAttack
		}
	}


    public static Level[] All
    {
        get
        {
            m_Levels = m_Levels ?? Resources.LoadAll< Level >( "Levels" );
            return m_Levels;
        }
    }

    public Event this[ int a_Index ] => events[ a_Index ];

    public static Level GetLevel( int a_Index )
    {
        return a_Index < 0 || a_Index >= All.Length ? null : All[ a_Index ];
    }

    private static Level[] m_Levels;
}