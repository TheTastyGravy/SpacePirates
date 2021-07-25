using System.Collections;
using System.Collections.Generic;
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
    }
    public InputDevice Device
    {
        get
        {
            return devices.Count > 0 ? devices[ 0 ] : null;
        }
    }
    public Character Character
    {
        get
        {
            return m_Character;
        }
    }
    public GameObject CharacterObject
    {
        get
        {
            return m_CharacterObject;
        }
    }

    private void Awake()
    {
        m_Character = new Character( -1, -1 );
        m_Inputs = actions.actionMaps[ 0 ].actions;
        ChangeCharacter( new Character( 0, 0 ) );
    }

    protected void GetInput( Control a_Control, out Vector2 a_Value )
    {
        a_Value = m_Inputs[ ( int )a_Control ].ReadValue< Vector2 >();
    }

    protected bool GetInput( Control a_Control )
    {
        return m_Inputs[ ( int )a_Control ].ReadValue< bool >();
    }

    protected void GetInput( Control a_Control, out float a_Value )
    {
        a_Value = m_Inputs[ ( int )a_Control ].ReadValue< float >();
    }

    public void ChangeCharacter( Character a_Character )
    {
        if ( a_Character.Index == m_Character.Index )
        {
            if ( a_Character.Variant == m_Character.Variant )
            {
                return;
            }
            else if ( m_CharacterObject == null )
            {
                CharacterManager.CreateCharacter( a_Character, transform ).name = "Character";
                OnCharacterChange( m_Character, a_Character );
            }
            else
            {
                m_CharacterObject.GetComponent< MeshRenderer >().material = CharacterManager.GetCharacterMaterial( a_Character );
                OnCharacterChange( m_Character, a_Character );
            }
        }
        else
        {
            if ( m_CharacterObject != null )
            {
                Destroy( m_CharacterObject );
            }
            
            m_CharacterObject = CharacterManager.CreateCharacter( a_Character, transform );
            m_CharacterObject.name = "Character";
            OnCharacterChange( m_Character, a_Character );
        }

        m_Character = a_Character;
    }

    protected virtual void OnCharacterChange( Character a_OldCharacter, Character a_NewCharacter ) { }

    public void SetControlStage( ControlStage a_ControlStage )
    {
        m_ControlStage = a_ControlStage;

        switch ( a_ControlStage )
        {
            case ControlStage.MENU:
                {
                    notificationBehavior = PlayerNotifications.SendMessages;
                }
                break;
            case ControlStage.GAME:
                {
                    enabled = true;
                    notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
                }
                break;
        }
    }

    private void OnDPAD_PRESSED( InputValue a_Value )
    {

    }

    private void OnSTART_PRESSED()
    {

    }

    private void OnA_PRESSED()
    {

    }

    private void OnB_PRESSED()
    {

    }

    private ControlStage m_ControlStage;
    private Character m_Character;
    private ReadOnlyArray< InputAction > m_Inputs;
    private GameObject m_CharacterObject;

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