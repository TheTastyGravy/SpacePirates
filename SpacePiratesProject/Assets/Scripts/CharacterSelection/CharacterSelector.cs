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

    public static Vector2Int GridSize
    {
        get
        {
            return Instance.m_Grid.size;
        }
    }

    private void Start()
    {
        Canvas.worldCamera = Camera.main;
        m_SelectorTiles = new SelectorTile[ ( int )Player.PlayerSlot.COUNT ];
        m_CharacterTiles = new CharacterTile[ CharacterTiles.Length ];
        m_CharacterDocks = new CharacterDock[ GameManager.MaxPlayers ];

        if ( PlayerInput.all.Count < GameManager.MaxPlayers )
        {
            PlayerInputManager.instance.EnableJoining();
        }
        else
        {
            PlayerInputManager.instance.DisableJoining();
        }

        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        PlayerInputManager.instance.onPlayerLeft += OnPlayerLeft;
        PopulateCharacterTiles();
        PopulateCharacterDocks();

        for ( int i = 0; i < Mathf.Min( PlayerInput.all.Count, m_CharacterDocks.Length ); ++i )
        {
            Player player = PlayerInput.all[ i ] as Player;

            if ( player.Character == null )
            {
                player.ChangeCharacter( 0, 0 );
            }

            m_CharacterDocks[ i ].SetPlayer( player );
            player.Character.gameObject.SetActive( true );
        }

        m_DefaultActionMap = GameManager.DefaultActionMap;
        int countCompatible = 0;

        foreach( InputDevice inputDevice in InputSystem.devices )
        {
            if ( m_DefaultActionMap.IsUsableWithDevice( inputDevice ) )
            {
                ++countCompatible;
            }
        }

        for ( int i = PlayerInput.all.Count; i < Mathf.Min( countCompatible, m_CharacterDocks.Length ); ++i )
        {
            m_CharacterDocks[ i ].ConnectPhase = CharacterDock.Phase.WAIT_ON_JOIN;
        }

        for ( int i = countCompatible; i < GameManager.MaxPlayers; ++i )
        {
            m_CharacterDocks[ i ].ConnectPhase = CharacterDock.Phase.WAIT_ON_DEVICE;
        }

        InputSystem.onDeviceChange += OnDeviceChange;
        Player primaryPlayer = Player.GetPlayerBySlot( Player.PlayerSlot.P1 );
        primaryPlayer.AddInputListener( Player.Control.A_PRESSED, OnAPressedByP1 );
        primaryPlayer.AddInputListener( Player.Control.B_PRESSED, OnBPressedByP1 );
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
        Player primaryPlayer = Player.GetPlayerBySlot( Player.PlayerSlot.P1 );
        primaryPlayer.RemoveInputListener( Player.Control.A_PRESSED, OnAPressedByP1 );
        primaryPlayer.RemoveInputListener( Player.Control.B_PRESSED, OnBPressedByP1 );
        PlayerInputManager.instance.onPlayerJoined -= OnPlayerJoined;
        PlayerInputManager.instance.onPlayerLeft -= OnPlayerLeft;
    }

    public static CharacterDock GetCharacterDock( Player.PlayerSlot a_PlayerSlot )
    {
        int index = ( int )a_PlayerSlot;
        
        if ( index >= Instance.m_CharacterDocks.Length )
        {
            return null;
        }

        return Instance.m_CharacterDocks[ index ];
    }

    public static bool DoesSelectorExist( Player.PlayerSlot a_PlayerSlot )
    {
        return Instance.m_SelectorTiles[ ( int )a_PlayerSlot ] != null;
    }

    public static SelectorTile InstantiateSelector( Player.PlayerSlot a_PlayerSlot, Vector2Int a_GridPosition )
    {
        if ( Instance.m_SelectorTiles[ ( int )a_PlayerSlot ] == null )
        {
            SelectorTile prefabSelectorTile = null;

            switch ( a_PlayerSlot )
            {
                case Player.PlayerSlot.P1:
                    {
                        prefabSelectorTile = Instance.SelectorP1;
                    }
                    break;
                case Player.PlayerSlot.P2:
                    {
                        prefabSelectorTile = Instance.SelectorP2;
                    }
                    break;
                case Player.PlayerSlot.P3:
                    {
                        prefabSelectorTile = Instance.SelectorP3;
                    }
                    break;
                case Player.PlayerSlot.P4:
                    {
                        prefabSelectorTile = Instance.SelectorP4;
                    }
                    break;
            }

            SelectorTile newSelectorTile = Instantiate( prefabSelectorTile, Instance.m_ParentSelectorTiles );
            Instance.m_SelectorTiles[ ( int )a_PlayerSlot ] = newSelectorTile;
            newSelectorTile.SetPosition( a_GridPosition, Instance.m_GridCellSize, Instance.m_GridCellSpacing );

            return newSelectorTile;
        }

        return null;
    }

    public static SelectorTile InstantiateSelector( Player.PlayerSlot a_PlayerSlot, int a_GridIndex )
    {
        if ( Instance.m_SelectorTiles[ ( int )a_PlayerSlot ] == null )
        {
            SelectorTile prefabSelectorTile = null;

            switch ( a_PlayerSlot )
            {
                case Player.PlayerSlot.P1:
                    {
                        prefabSelectorTile = Instance.SelectorP1;
                    }
                    break;
                case Player.PlayerSlot.P2:
                    {
                        prefabSelectorTile = Instance.SelectorP2;
                    }
                    break;
                case Player.PlayerSlot.P3:
                    {
                        prefabSelectorTile = Instance.SelectorP3;
                    }
                    break;
                case Player.PlayerSlot.P4:
                    {
                        prefabSelectorTile = Instance.SelectorP4;
                    }
                    break;
            }

            SelectorTile newSelectorTile = Instantiate( prefabSelectorTile, Instance.m_ParentSelectorTiles );
            Instance.m_SelectorTiles[ ( int )a_PlayerSlot ] = newSelectorTile;

            Vector2Int gridPosition = Vector2Int.zero;
            gridPosition.y = a_GridIndex / Instance.m_Grid.width;
            gridPosition.x = a_GridIndex - gridPosition.y * Instance.m_Grid.width;

            newSelectorTile.SetPosition( gridPosition, Instance.m_GridCellSize, Instance.m_GridCellSpacing );

            return newSelectorTile;
        }

        return null;
    }

    public static bool DestroySelector( Player.PlayerSlot a_PlayerSlot )
    {
        if ( Instance.m_SelectorTiles[ ( int ) a_PlayerSlot ] != null )
        {
            Destroy( Instance.m_SelectorTiles[ ( int )a_PlayerSlot ].gameObject );
            return true;
        }

        return false;
    }

    public static bool ShiftSelector( Player.PlayerSlot a_PlayerSlot, Direction a_Direction )
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

    public static Vector2Int GetSelectorGridPosition( Player.PlayerSlot a_PlayerSlot )
    {
        SelectorTile tile = Instance.m_SelectorTiles[ ( int )a_PlayerSlot ];

        return tile != null ? tile.GridPosition : -Vector2Int.one;
    }

    private void PopulateCharacterDocks()
    {
        for ( int i = 0; i < m_CharacterDocks.Length; ++i )
        {
            m_CharacterDocks[ i ] = Instantiate( CharacterDock, m_ParentCharacterDocks );
        }

        RepositionDocks();
    }

    private void PopulateCharacterTiles()
    {
        IEnumerator gridEnumerator = m_Grid.allPositionsWithin;
        int i = 0;

        while ( gridEnumerator.MoveNext() && i < CharacterTiles.Length )
        {
            m_CharacterTiles[ i ] = Instantiate( CharacterTiles[ i ], m_ParentCharacterTiles );
            m_CharacterTiles[ i++ ].SetPosition( ( Vector2Int )gridEnumerator.Current, m_GridCellSize, m_GridCellSpacing );
        }
    }

    private void RepositionDocks()
    {
        Vector3 boundLeft = m_DockBoundLeft.position;
        Vector3 boundRight = m_DockBoundRight.position;

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
                Player newPlayer = a_PlayerInput as Player;
                m_CharacterDocks[ i ].SetPlayer( newPlayer );
                newPlayer.ChangeCharacter( 0, 0 );
                ( newPlayer.Character as Character ).IsKinematic = false;
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
        if ( m_DefaultActionMap.IsUsableWithDevice( a_InputDevice ) )
        {
            switch ( a_InputDeviceChange )
            {
                case InputDeviceChange.Added:
                    {
                        bool isDeviceAssigned = false;

                        foreach ( PlayerInput playerInput in PlayerInput.all )
                        {
                            if ( ( playerInput as Player ).Device.deviceId == a_InputDevice.deviceId )
                            {
                                isDeviceAssigned = true;
                                break;
                            }
                        }

                        for ( int i = 0; i < m_CharacterDocks.Length && !isDeviceAssigned; ++i )
                        {
                            if ( m_CharacterDocks[ i ].ConnectPhase == CharacterDock.Phase.WAIT_ON_DEVICE )
                            {
                                m_CharacterDocks[ i ].ConnectPhase = CharacterDock.Phase.WAIT_ON_JOIN;
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
                            if ( ( playerInput as Player ).Device.deviceId == a_InputDevice.deviceId )
                            {
                                isDeviceAssigned = true;
                                break;
                            }
                        }

                        if ( !isDeviceAssigned )
                        {
                            for ( int i = m_CharacterDocks.Length - 1; i >= 0; --i )
                            {
                                if ( m_CharacterDocks[ i ].ConnectPhase == CharacterDock.Phase.WAIT_ON_JOIN )
                                {
                                    m_CharacterDocks[ i ].ConnectPhase = CharacterDock.Phase.WAIT_ON_DEVICE;
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
        PlayerInputManager.instance.DisableJoining();

        foreach ( PlayerInput playerInput in PlayerInput.all )
        {
            playerInput.transform.parent = null;
            ( playerInput as Player ).Character.gameObject.SetActive( false );
            playerInput.transform.SetPositionAndRotation( Vector3.zero, Quaternion.identity );
            DontDestroyOnLoad( playerInput.gameObject );
        }
        
        GameManager.CurrentState = GameManager.GameState.TRACK;
    }

    private void OnBPressedByP1( InputAction.CallbackContext _ )
    {
        for ( int i = 1; i < PlayerInput.all.Count; ++i )
        {
            Destroy( PlayerInput.all[ i ].gameObject );
        }

        Player primaryPlayer = PlayerInput.all[ 0 ] as Player;
        primaryPlayer.DestroyCharacter();
        primaryPlayer.transform.parent = null;
        DontDestroyOnLoad( primaryPlayer.gameObject );
        GameManager.CurrentState = GameManager.GameState.SHIP;
    }

    private SelectorTile[] m_SelectorTiles;
    private CharacterTile[] m_CharacterTiles;
    private CharacterDock[] m_CharacterDocks;
    private InputActionMap m_DefaultActionMap;
    [ SerializeField ] private RectTransform m_DockBoundLeft;
    [ SerializeField ] private RectTransform m_DockBoundRight;
    [ SerializeField ] private RectTransform m_ParentCharacterDocks;
    [ SerializeField ] private RectTransform m_ParentCharacterTiles;
    [ SerializeField ] private RectTransform m_ParentSelectorTiles;
    [ SerializeField ] private Vector2 m_GridCellSpacing;
    [ SerializeField ] private Vector2 m_GridCellSize;
    [ SerializeField ] private RectInt m_Grid;

    public enum Direction
    {
        UP, DOWN, LEFT, RIGHT
    }
}