using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterSelector : Singleton< CharacterSelector >
{
    public Canvas Canvas;

    public SelectorTile SelectorP1;
    public SelectorTile SelectorP2;
    public SelectorTile SelectorP3;
    public SelectorTile SelectorP4;

    public CharacterTile[] CharacterTiles;
    public CharacterDock CharacterDock;

    public Transform DockBoundLeft;
    public Transform DockBoundRight;
    public RectTransform ParentCharacterTiles;
    public RectTransform ParentSelectorTiles;

    private void Start()
    {
        Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        Canvas.worldCamera = Camera.main;
        m_SelectorTiles = new SelectorTile[ ( int )IPlayer.PlayerSlot.COUNT ];
        m_CharacterTiles = new CharacterTile[ CharacterTiles.Length ];
        m_CharacterDocks = new CharacterDock[ GameManager.MaxPlayers ];
        PopulateCharacterTiles();
        PopulateCharacterDocks();

        for ( int i = 0; i < PlayerInput.all.Count; ++i )
        {
            m_CharacterDocks[ i ].SetPlayer( PlayerInput.all[ i ] as IPlayer );
            ControllerManager.RetrievePlayer( ( IPlayer.PlayerSlot )i ).ChangeCharacter( 0, 0 );
        }

        for ( int i = PlayerInput.all.Count; i < Gamepad.all.Count; ++i )
        {
            m_CharacterDocks[ i ].CurrentStage = CharacterDock.Stage.WAIT_ON_JOIN;
        }

        for ( int i = Gamepad.all.Count; i < GameManager.MaxPlayers; ++i )
        {
            m_CharacterDocks[ i ].CurrentStage = CharacterDock.Stage.WAIT_ON_DEVICE;
        }

        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    public CharacterDock GetCharacterDock( IPlayer.PlayerSlot a_PlayerSlot )
    {
        int index = ( int )a_PlayerSlot;
        
        if ( index >= m_CharacterDocks.Length )
        {
            return null;
        }

        return m_CharacterDocks[ index ];
    }

    public void PopulateCharacterDocks()
    {
        for ( int i = 0; i < m_CharacterDocks.Length; ++i )
        {
            m_CharacterDocks[ i ] = Instantiate( CharacterDock );
        }

        RepositionDocks();
    }

    public void PopulateCharacterTiles()
    {
        IEnumerator gridEnumerator = m_Grid.allPositionsWithin;
        int i = 0;

        while ( gridEnumerator.MoveNext() && i < CharacterTiles.Length )
        {
            m_CharacterTiles[ i ] = Instantiate( CharacterTiles[ i ], ParentCharacterTiles );
            m_CharacterTiles[ i++ ].SetPosition( ( Vector2Int )gridEnumerator.Current, m_GridCellSize, m_GridCellSpacing );
        }
    }

    public bool DoesSelectorExist( IPlayer.PlayerSlot a_PlayerSlot )
    {
        return m_SelectorTiles[ ( int )a_PlayerSlot ] != null;
    }

    public SelectorTile InstantiateSelector( IPlayer.PlayerSlot a_PlayerSlot, Vector2Int a_GridPosition )
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

            SelectorTile newSelectorTile = Instantiate( prefabSelectorTile, ParentSelectorTiles );
            newSelectorTile.SetPosition( a_GridPosition, m_GridCellSize, m_GridCellSpacing );

            return newSelectorTile;
        }

        return null;
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

    private void RepositionDocks()
    {
        Vector3 boundLeft = DockBoundLeft.position;
        Vector3 boundRight = DockBoundRight.position;

        float increment = 1.0f / ( 1 + m_CharacterDocks.Length );

        for ( int i = 0; i < m_CharacterDocks.Length; ++i )
        {
            m_CharacterDocks[ i ].transform.position = Vector3.Lerp( boundLeft, boundRight, ( i + 1 ) * increment );
        }
    }

    private void OnDeviceChange( InputDevice a_InputDevice, InputDeviceChange a_InputDeviceChange )
    {
        if ( a_InputDevice is Gamepad  )
        {
            switch ( a_InputDeviceChange )
            {
                case InputDeviceChange.Added:
                    {

                    }
                    break;
                case InputDeviceChange.Removed:
                    {

                    }
                    break;
            }
        }
    }

    //private int Find

    private SelectorTile[] m_SelectorTiles;
    private CharacterTile[] m_CharacterTiles;
    private CharacterDock[] m_CharacterDocks;
    [ SerializeField ] private Vector2 m_GridCellSpacing;
    [ SerializeField ] private Vector2 m_GridCellSize;
    [ SerializeField ] private RectInt m_Grid;

    public enum Direction
    {
        UP, DOWN, LEFT, RIGHT
    }
}