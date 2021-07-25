using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : Singleton< CharacterSelector >
{
    public SelectorTile SelectorP1;
    public SelectorTile SelectorP2;
    public SelectorTile SelectorP3;
    public SelectorTile SelectorP4;

    public CharacterTile[] CharacterTiles;

    private void Start()
    {
        m_SelectorTiles = new SelectorTile[ ( int )IPlayer.PlayerSlot.COUNT ];
        m_CharacterTiles = new CharacterTile[ CharacterTiles.Length ];
        m_ParentCharacterTiles = m_ParentCharacterTiles ?? transform.Find( "CharacterTiles" )?.GetComponent< RectTransform >();
        m_ParentSelectorTiles = m_ParentSelectorTiles ?? transform.Find( "SelectorTiles" )?.GetComponent< RectTransform >();

        PopulateCharacterTiles();
    }

    public void PopulateCharacterTiles()
    {
        IEnumerator gridEnumerator = m_Grid.allPositionsWithin;
        int i = 0;

        while ( gridEnumerator.MoveNext() && i < CharacterTiles.Length )
        {
            m_CharacterTiles[ i ] = Instantiate( CharacterTiles[ i ], m_ParentCharacterTiles );
            m_CharacterTiles[ i++ ].SetPosition( ( Vector2Int )gridEnumerator.Current, m_GridCellSize, m_GridCellSpacing );
        }
    }

    public bool DoesSelectorExist( IPlayer.PlayerSlot a_PlayerSlot )
    {
        return m_SelectorTiles[ ( int )a_PlayerSlot ] != null;
    }

    public bool InstantiateSelector( IPlayer.PlayerSlot a_PlayerSlot, Vector2Int a_GridPosition )
    {
        if ( m_SelectorTiles[ ( int )a_PlayerSlot ] == null )
        {
            SelectorTile prefabSelectorTile = null;

            switch ( a_PlayerSlot )
            {
                case IPlayer.PlayerSlot.P1:
                    {
                        prefabSelectorTile = SelectorP1;
                    }
                    break;
                case IPlayer.PlayerSlot.P2:
                    {
                        prefabSelectorTile = SelectorP1;
                    }
                    break;
                case IPlayer.PlayerSlot.P3:
                    {
                        prefabSelectorTile = SelectorP1;
                    }
                    break;
                case IPlayer.PlayerSlot.P4:
                    {
                        prefabSelectorTile = SelectorP1;
                    }
                    break;
            }

            SelectorTile newSelectorTile = Instantiate( prefabSelectorTile, m_ParentSelectorTiles );
            newSelectorTile.SetPosition( a_GridPosition, m_GridCellSize, m_GridCellSpacing );

            return true;
        }

        return false;
    }

    public bool DestroySelector( IPlayer.PlayerSlot a_PlayerSlot )
    {
        if ( m_SelectorTiles[ ( int ) a_PlayerSlot ] != null )
        {
            Destroy( m_SelectorTiles[ ( int )a_PlayerSlot ].gameObject );
            return true;
        }

        return false;
    }

    public bool ShiftSelector( IPlayer.PlayerSlot a_PlayerSlot, Direction a_Direction )
    {
        SelectorTile selector = m_SelectorTiles[ ( int )a_PlayerSlot ];
        
        if ( selector == null )
        {
            return false;
        }

        Vector2Int newPosition = selector.GridPosition;

        switch ( a_Direction )
        {
            case Direction.UP:
                {
                    newPosition += Vector2Int.down;
                }
                break;
            case Direction.DOWN:
                {
                    newPosition += Vector2Int.up;
                }
                break;
            case Direction.LEFT:
                {
                    newPosition += Vector2Int.left;
                }
                break;
            case Direction.RIGHT:
                {
                    newPosition += Vector2Int.right;
                }
                break;
        }

        if ( !m_Grid.Contains( newPosition ) )
        {
            return false;
        }

        selector.SetPosition( newPosition, m_GridCellSize, m_GridCellSpacing );
        return true;
    }

    private SelectorTile[] m_SelectorTiles;
    private CharacterTile[] m_CharacterTiles;
    private RectTransform m_ParentCharacterTiles;
    private RectTransform m_ParentSelectorTiles;
    [ SerializeField ] private Vector2 m_GridCellSpacing;
    [ SerializeField ] private Vector2 m_GridCellSize;
    [ SerializeField ] private RectInt m_Grid;

    public enum Direction
    {
        UP, DOWN, LEFT, RIGHT
    }
}