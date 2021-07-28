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

    public RectTransform ParentCharacterDocks;
    public RectTransform DockBoundLeft;
    public RectTransform DockBoundRight;
    public RectTransform ParentCharacterTiles;
    public RectTransform ParentSelectorTiles;

    private void Start()
    {
        Canvas.worldCamera = Camera.main;
        m_SelectorTiles = new SelectorTile[ ( int )IPlayer.PlayerSlot.COUNT ];
        m_CharacterTiles = new CharacterTile[ CharacterTiles.Length ];
        m_CharacterDocks = new CharacterDock[ GameManager.MaxPlayers ];
        PlayerInputManager.instance.EnableJoining();
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        PlayerInputManager.instance.onPlayerLeft += OnPlayerLeft;
        PopulateCharacterTiles();
        PopulateCharacterDocks();

        for ( int i = 0; i < Mathf.Min( PlayerInput.all.Count, m_CharacterDocks.Length ); ++i )
        {
            m_CharacterDocks[ i ].SetPlayer( PlayerInput.all[ i ] as IPlayer );
            ControllerManager.RetrievePlayer( ( IPlayer.PlayerSlot )i ).ChangeCharacter( 0, 0 );
        }

        for ( int i = PlayerInput.all.Count; i < Mathf.Min( Gamepad.all.Count, m_CharacterDocks.Length ); ++i )
        {
            m_CharacterDocks[ i ].CurrentStage = CharacterDock.Stage.WAIT_ON_JOIN;
        }

        for ( int i = Gamepad.all.Count; i < GameManager.MaxPlayers; ++i )
        {
            m_CharacterDocks[ i ].CurrentStage = CharacterDock.Stage.WAIT_ON_DEVICE;
        }

        InputSystem.onDeviceChange += OnDeviceChange;
        ControllerManager.RetrievePlayer(IPlayer.PlayerSlot.P1).AddInputListener( IPlayer.Control.A_PRESSED, OnAPressedByP1 );
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
        ControllerManager.RetrievePlayer( IPlayer.PlayerSlot.P1 ).RemoveInputListener( IPlayer.Control.A_PRESSED, OnAPressedByP1 );
        PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
        PlayerInputManager.instance.onPlayerLeft -= OnPlayerLeft;
        PlayerInputManager.instance.DisableJoining();
    }

    public static CharacterDock GetCharacterDock( IPlayer.PlayerSlot a_PlayerSlot )
    {
        int index = ( int )a_PlayerSlot;
        
        if ( index >= Instance.m_CharacterDocks.Length )
        {
            return null;
        }

        return Instance.m_CharacterDocks[ index ];
    }

    public static bool DoesSelectorExist( IPlayer.PlayerSlot a_PlayerSlot )
    {
        return Instance.m_SelectorTiles[ ( int )a_PlayerSlot ] != null;
    }

    public static SelectorTile InstantiateSelector( IPlayer.PlayerSlot a_PlayerSlot, Vector2Int a_GridPosition )
    {
        if ( Instance.m_SelectorTiles[ ( int )a_PlayerSlot ] == null )
        {
            SelectorTile prefabSelectorTile = null;

            switch ( a_PlayerSlot )
            {
                case IPlayer.PlayerSlot.P1:
                    {
                        prefabSelectorTile = Instance.SelectorP1;
                    }
                    break;
                case IPlayer.PlayerSlot.P2:
                    {
                        prefabSelectorTile = Instance.SelectorP2;
                    }
                    break;
                case IPlayer.PlayerSlot.P3:
                    {
                        prefabSelectorTile = Instance.SelectorP3;
                    }
                    break;
                case IPlayer.PlayerSlot.P4:
                    {
                        prefabSelectorTile = Instance.SelectorP4;
                    }
                    break;
            }

            SelectorTile newSelectorTile = Instantiate( prefabSelectorTile, Instance.ParentSelectorTiles );
            Instance.m_SelectorTiles[ ( int )a_PlayerSlot ] = newSelectorTile;
            newSelectorTile.SetPosition( a_GridPosition, Instance.m_GridCellSize, Instance.m_GridCellSpacing );

            return newSelectorTile;
        }

        return null;
    }

    public static bool DestroySelector( IPlayer.PlayerSlot a_PlayerSlot )
    {
        if ( Instance.m_SelectorTiles[ ( int ) a_PlayerSlot ] != null )
        {
            Destroy( Instance.m_SelectorTiles[ ( int )a_PlayerSlot ].gameObject );
            return true;
        }

        return false;
    }

    public static bool ShiftSelector( IPlayer.PlayerSlot a_PlayerSlot, Direction a_Direction )
    {
        SelectorTile selector = Instance.m_SelectorTiles[ ( int )a_PlayerSlot ];
        
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

        if ( !Instance.m_Grid.Contains( newPosition ) )
        {
            return false;
        }

        selector.SetPosition( newPosition, Instance.m_GridCellSize, Instance.m_GridCellSpacing );
        return true;
    }

    private void PopulateCharacterDocks()
    {
        for ( int i = 0; i < m_CharacterDocks.Length; ++i )
        {
            m_CharacterDocks[ i ] = Instantiate( CharacterDock, ParentCharacterDocks );
        }

        RepositionDocks();
    }

    private void PopulateCharacterTiles()
    {
        IEnumerator gridEnumerator = m_Grid.allPositionsWithin;
        int i = 0;

        while ( gridEnumerator.MoveNext() && i < CharacterTiles.Length )
        {
            m_CharacterTiles[ i ] = Instantiate( CharacterTiles[ i ], ParentCharacterTiles );
            m_CharacterTiles[ i++ ].SetPosition( ( Vector2Int )gridEnumerator.Current, m_GridCellSize, m_GridCellSpacing );
        }
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

    private void OnPlayerJoined( PlayerInput a_PlayerInput )
    {
        for ( int i = 0; i < m_CharacterDocks.Length; ++i )
        {
            if ( m_CharacterDocks[ i ].AssignedPlayer == null )
            {
                IPlayer newPlayer = a_PlayerInput as IPlayer;
                m_CharacterDocks[ i ].SetPlayer( newPlayer );
                newPlayer.ChangeCharacter( 0, 0 );
                break;
            }
        }

        if ( PlayerInput.all.Count >= GameManager.MaxPlayers )
        {
            PlayerInputManager.instance.DisableJoining();
        }
    }

    private void OnPlayerLeft( PlayerInput _ )
    {
        PlayerInputManager.instance.EnableJoining();
    }

    private void OnDeviceChange( InputDevice a_InputDevice, InputDeviceChange a_InputDeviceChange )
    {
        if ( a_InputDevice is Gamepad  )
        {
            switch ( a_InputDeviceChange )
            {
                case InputDeviceChange.Added:
                    {
                        bool isDeviceAssigned = false;

                        foreach ( PlayerInput playerInput in PlayerInput.all )
                        {
                            if ( ( playerInput as IPlayer ).Device.deviceId == a_InputDevice.deviceId )
                            {
                                isDeviceAssigned = true;
                                break;
                            }
                        }

                        for ( int i = 0; i < m_CharacterDocks.Length && !isDeviceAssigned; ++i )
                        {
                            if ( m_CharacterDocks[ i ].CurrentStage == CharacterDock.Stage.WAIT_ON_DEVICE )
                            {
                                m_CharacterDocks[ i ].CurrentStage = CharacterDock.Stage.WAIT_ON_JOIN;
                                break;
                            }
                        }
                    }
                    break;
                case InputDeviceChange.Removed:
                    {
                        bool isDeviceAssigned = false;

                        foreach ( PlayerInput playerInput in PlayerInput.all )
                        {
                            if ( ( playerInput as IPlayer ).Device.deviceId == a_InputDevice.deviceId )
                            {
                                isDeviceAssigned = true;
                                break;
                            }
                        }

                        if ( !isDeviceAssigned )
                        {
                            for ( int i = m_CharacterDocks.Length - 1; i >= 0; --i )
                            {
                                if ( m_CharacterDocks[ i ].CurrentStage == CharacterDock.Stage.WAIT_ON_JOIN )
                                {
                                    m_CharacterDocks[ i ].CurrentStage = CharacterDock.Stage.WAIT_ON_DEVICE;
                                    break;
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }

    private void OnAPressedByP1( InputAction.CallbackContext _ )
    {
        GameManager.CurrentState = GameManager.GameState.TRACK;
    }

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