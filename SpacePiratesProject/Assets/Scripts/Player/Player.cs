using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class Player : PlayerInput
{
    public PlayerSlot Slot => ( PlayerSlot )playerIndex;
    public ControlStage Stage => m_ControlStage;
    public InputDevice Device => m_InputDevice;
    public ICharacter Character => m_Character;
    public bool IsDeviceConnected => m_IsDeviceConnected;

    private void Awake()
    {
        m_InputActions = actions.actionMaps[ 0 ].actions;
        m_IsDeviceConnected = true;
        onDeviceLost += device => m_IsDeviceConnected = false;
        onDeviceRegained += device => m_IsDeviceConnected = true;
    }

    private void Start()
    {
        m_InputDevice = devices.Count > 0 ? devices[ 0 ] : null;
    }

    public static Player GetPlayerBySlot( PlayerSlot a_PlayerSlot ) => GetPlayerByIndex( ( int )a_PlayerSlot ) as Player;

    public bool GetInput( Control a_Control )
    {
        return m_InputActions[ ( int )a_Control ].ReadValue< bool >();
    }

    public void GetInput( Control a_Control, out Vector2 o_Value )
    {
        o_Value = m_InputActions[ ( int )a_Control ].ReadValue< Vector2 >();
    }

    public void GetInput( Control a_Control, out Vector3 o_Value )
    {
        o_Value = m_InputActions[ ( int )a_Control ].ReadValue< Vector2 >();
        o_Value.z = o_Value.y;
        o_Value.y = 0;
    }

    public void GetInput( Control a_Control, out float o_Value )
    {
        o_Value = m_InputActions[ ( int )a_Control ].ReadValue< float >();
    }

    public InputAction GetInputAction( Control a_Control )
    {
        return m_InputActions[ ( int )a_Control ];
    }
    
    public void AddInputListener( Control a_Control, Action< InputAction.CallbackContext > a_OnPerformed )
    {
        m_InputActions[ ( int )a_Control ].performed += a_OnPerformed;
    }

    public void RemoveInputListener( Control a_Control, Action< InputAction.CallbackContext > a_OnPerformed )
    {
        m_InputActions[ ( int )a_Control ].performed -= a_OnPerformed;
    }

    public void ChangeCharacter( int a_VariantIndex )
    {
        m_Character.VariantIndex = a_VariantIndex;
    }

    public void ChangeCharacter( int a_CharacterIndex, int a_VariantIndex )
    {
        if ( m_Character != null && m_Character.CharacterIndex == a_CharacterIndex )
        {
            if ( m_Character.VariantIndex == a_VariantIndex )
            {
                return;
            }
            else
            {
                m_Character.VariantIndex = a_VariantIndex;
            }
        }
        else
        {
            if ( m_Character != null )
            {
                Destroy( m_Character.gameObject );
            }

            m_Character = CharacterManager.CreateCharacter( this, a_CharacterIndex, a_VariantIndex );
        }
    }

    public void ChangeCharacter( string a_CharacterName, int a_VariantIndex )
    {
        if ( m_Character != null && m_Character.CharacterName == a_CharacterName )
        {
            if ( m_Character.VariantIndex == a_VariantIndex )
            {
                return;
            }
            else
            {
                m_Character.VariantIndex = a_VariantIndex;
            }
        }
        else
        {
            if ( m_Character != null )
            {
                Destroy( m_Character.gameObject );
            }

            m_Character = CharacterManager.CreateCharacter( this, a_CharacterName, a_VariantIndex );
            typeof( ICharacter ).GetProperty( "m_Player" ).SetValue( m_Character, this );
        }
    }

    public void DestroyCharacter()
    {
        if ( m_Character == null )
        {
            return;
        }

        Destroy( m_Character.gameObject );
    }

    private ControlStage m_ControlStage;
    private ICharacter m_Character;
    private ReadOnlyArray< InputAction > m_InputActions;
    private InputDevice m_InputDevice;
    private bool m_IsDeviceConnected;

    public enum PlayerSlot
    {
        P1, P2, P3, P4,
        COUNT
    }

    public enum ControlStage
    {
        MENU, GAME
    }

    public enum Control
    {
        DPAD_PRESSED,
        LEFT_STICK,
        LEFT_STICK_BUTTON_PRESSED,
        RIGHT_STICK,
        RIGHT_STICK_BUTTON_PRESSED,
        START_PRESSED,
        BACK_PRESSED,
        A_PRESSED,
        B_PRESSED,
        X_PRESSED,
        Y_PRESSED,
        LEFT_BUMBER_PRESSED,
        LEFT_TRIGGER,
        RIGHT_BUMPER_PRESSED,
        RIGHT_TRIGGER,
        COUNT
    }
}