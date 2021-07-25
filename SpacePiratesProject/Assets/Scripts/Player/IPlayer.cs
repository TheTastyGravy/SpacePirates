using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class IPlayer : PlayerInput
{
    public PlayerSlot Slot
    {
        get
        {
            return ( PlayerSlot )playerIndex;
        }
    }
    public ControlStage Stage
    {
        get
        {
            return m_ControlStage;
        }
        set
        {
            m_ControlStage = value;
        }
    }
    public InputDevice Device
    {
        get
        {
            return devices.Count > 0 ? devices[ 0 ] : null;
        }
    }
    public ICharacter Character
    {
        get
        {
            return m_Character;
        }
    }

    private void Start()
    {
        m_InputActions = actions.actionMaps[ 0 ].actions;
    }

    public bool GetInput( Control a_Control )
    {
        return m_InputActions[ ( int )a_Control ].ReadValue< bool >();
    }

    public void GetInput( Control a_Control, out Vector2 a_Value )
    {
        a_Value = m_InputActions[ ( int )a_Control ].ReadValue< Vector2 >();
    }

    public void GetInput( Control a_Control, out float a_Value )
    {
        a_Value = m_InputActions[ ( int )a_Control ].ReadValue< float >();
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
                m_Character = CharacterManager.CreateCharacter( a_CharacterIndex, a_VariantIndex );
                m_Character.transform.parent = transform;
                typeof( ICharacter ).GetProperty( "m_Player" ).SetValue( m_Character, this );
            }
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
                m_Character = CharacterManager.CreateCharacter( a_CharacterName, a_VariantIndex );
                m_Character.transform.parent = transform;
                typeof( ICharacter ).GetProperty( "m_Player" ).SetValue( m_Character, this );
            }
        }
    }

    private ControlStage m_ControlStage;
    private ICharacter m_Character;
    private ReadOnlyArray< InputAction > m_InputActions;

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