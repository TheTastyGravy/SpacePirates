using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ CreateAssetMenu( fileName = "NewTrack", menuName = "", order = 1 ) ]
public class Track : ScriptableObject
{
    public Segment[] Segments;

    public static Track[] All
    {
        get
        {
            m_Tracks = m_Tracks ?? Resources.LoadAll< Track >( "Tracks" );
            return m_Tracks;
        }
    }
    public int Length => Segments != null ? Segments.Length : 0;

    public Segment this[ int a_Index ] => Segments[ a_Index ];

    public static Track GetTrack( int a_Index )
    {
        return a_Index < 0 || a_Index >= All.Length ? null : All[ a_Index ];
    }

    private static Track[] m_Tracks;

    [ Serializable ]
    public struct Segment
    {
        public Type SegmentType => m_SegmentType;
        public int TimeToComplete => m_TimeToComplete;

        [ SerializeField ] private Type m_SegmentType;
        [ SerializeField ] private int m_TimeToComplete;

        public enum Type
	    {
            Straight,
            Left90,
            Left45,
            Right90,
            Right45
	    }
    }
}