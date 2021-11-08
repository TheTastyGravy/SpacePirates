using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;

public class CharacterSelector : Singleton< CharacterSelector >
{
    public Canvas Canvas;
    public SelectorTile SelectorP1;
    public SelectorTile SelectorP2;
    public SelectorTile SelectorP3;
    public SelectorTile SelectorP4;
    public CharacterTile[] CharacterTiles;
    public CharacterDock CharacterDock;
    [Space]
    public EventReference selectEvent;
    public EventReference returnEvent;

    public static Vector2Int GridSize
    {
        get
        {
            return Instance.m_Grid.size;
        }
    }

    private void Start()
    {
        // Edge case
        if (PlayerInputManager.instance.playerCount == 0)
        {
            GameManager.ChangeState(GameManager.GameState.START);
            return;
        }

        //Canvas.worldCamera = Camera.main;
        m_SelectorTiles = new SelectorTile[ ( int )Player.PlayerSlot.COUNT ];
        m_CharacterTiles = new CharacterTile[ CharacterTiles.Length ];
        m_CharacterDocks = new CharacterDock[ GameManager.MaxPlayers ];

        // Only allow new players if not over max players
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

        // Setup characters for each player
        for ( int i = 0; i < Mathf.Min( PlayerInput.all.Count, m_CharacterDocks.Length ); ++i )
        {
            Player player = PlayerInput.all[ i ] as Player;

            if ( player.Character == null )
            {
                player.ChangeCharacter( 0, 0 );
            }

            m_CharacterDocks[ i ].SetPlayer( player );
            player.Character.gameObject.SetActive( true );
            player.Character.SetUseCharacterSelectAnimations(true);
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

        // Setup listeners
        InputSystem.onDeviceChange += OnDeviceChange;
        Player primaryPlayer = Player.GetPlayerBySlot( Player.PlayerSlot.P1 );
        primaryPlayer.AddInputListener( Player.Control.B_PRESSED, OnBPressedByP1 );
    }

    private void OnDestroy()
    {
        // Remove listeners
        InputSystem.onDeviceChange -= OnDeviceChange;
        Player primaryPlayer = Player.GetPlayerBySlot( Player.PlayerSlot.P1 );
        if (primaryPlayer != null)
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

    public static bool ShiftSelector( Player.PlayerSlot a_PlayerSlot, Direction a_Direction, bool a_WrapAround = false )
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

        if ( !a_WrapAround && !Instance.m_Grid.Contains( newPosition ) )
        {
            return false;
        }

        if ( newPosition.x < 0 )
        {
            newPosition.x = Instance.m_Grid.width - 1;
        }

        if ( newPosition.x >= Instance.m_Grid.width )
        {
            newPosition.x = 0;
        }

        if ( newPosition.y < 0 )
        {
            newPosition.y = Instance.m_Grid.height - 1;
        }

        if ( newPosition.y >= Instance.m_Grid.height )
        {
            newPosition.y = 0;
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

        float increment = 1.0f / ( m_CharacterDocks.Length );

        if (m_CharacterDocks.Length == 2)
        {
            m_CharacterDocks[0].transform.position = Vector3.Lerp(boundLeft, boundRight, 0.6f * increment);
            m_CharacterDocks[1].transform.position = Vector3.Lerp(boundLeft, boundRight, 1.4f * increment);
        }
        else
        {
            for (int i = 0; i < m_CharacterDocks.Length; ++i)
            {
                m_CharacterDocks[i].transform.position = Vector3.Lerp(boundLeft, boundRight, (i + 0.5f + (i / m_CharacterDocks.Length)) * increment);
            }
        }
    }

    private void OnPlayerJoined( PlayerInput a_PlayerInput )
    {
        for ( int i = 0; i < m_CharacterDocks.Length; ++i )
        {
            if ( m_CharacterDocks[ i ].AssignedPlayer == null )
            {
                RuntimeManager.PlayOneShot(selectEvent);

                Player newPlayer = a_PlayerInput as Player;
                newPlayer.ChangeCharacter( 0, 0 );
                newPlayer.Character.SetUseCharacterSelectAnimations(true);
                m_CharacterDocks[i].SetPlayer(newPlayer);
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
        RuntimeManager.PlayOneShot(returnEvent);
        PlayerInputManager.instance.EnableJoining();
        CheckReadyState();
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

    private void OnBPressedByP1( InputAction.CallbackContext _ )
    {
        // Destroy all other players (players can only join in character select)
        for ( int i = 1; i < PlayerInput.all.Count; ++i )
        {
            Destroy( PlayerInput.all[ i ].gameObject );
        }

        // Reset player 1 and return to ship select
        Player primaryPlayer = PlayerInput.all[ 0 ] as Player;
        primaryPlayer.DestroyCharacter();
        primaryPlayer.transform.parent = null;
        DontDestroyOnLoad( primaryPlayer.gameObject );
        GameManager.ChangeState(GameManager.GameState.SHIP);

        RuntimeManager.PlayOneShot(returnEvent);
    }

    public void CheckReadyState()
	{
        foreach (var obj in m_CharacterDocks)
		{
            // A player is choosing their character, so all players are not ready
            if (obj.AssignedPlayer != null && obj.ConnectPhase == CharacterDock.Phase.CHOOSE_CHARACTER)
			{
                return;
			}
		}

        // All players are ready, so we can continue to track select
        PlayerInputManager.instance.DisableJoining();
        // Setup players to change scene
        foreach (var dock in m_CharacterDocks)
        {
            Player player = dock.AssignedPlayer;
            if (player == null)
                continue;

            player.transform.parent = null;
            // Fix scale to prevent weirdness
            player.transform.localScale = Vector3.one;

            if (player.Character)
			{
                player.Character.SetUseCharacterSelectAnimations(false);
                player.Character.gameObject.SetActive(false);
            }
            player.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            DontDestroyOnLoad(player.gameObject);
        }

        GameManager.ChangeState(GameManager.GameState.TRACK);
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