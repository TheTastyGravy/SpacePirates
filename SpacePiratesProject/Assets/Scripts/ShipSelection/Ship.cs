using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ CreateAssetMenu( fileName = "NewShip", menuName = "", order = 2 ) ]
public class Ship : ScriptableObject
{
    public GameObject ShipPrefab;
    public float heightOffset = 0;

    public static Ship[] All
    {
        get
        {
            m_Ships = m_Ships ?? Resources.LoadAll< Ship >( "Ships" );
            return m_Ships;
        }
    }

    public static Ship GetShip( int a_Index )
    {
        return a_Index < 0 || a_Index >= All.Length ? null : All[ a_Index ];
    }

    private static Ship[] m_Ships;
}