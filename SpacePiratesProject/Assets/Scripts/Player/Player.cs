using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ RequireComponent( typeof( PlayerInput ) ) ]
public class Player : MonoBehaviour
{
    public PlayerSlot Slot
    {
        get
        {
            return m_Slot;
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
            return m_Device;
        }
    }
    public PlayerInput PlayerInput
    {
        get
        {
            return m_PlayerInput;
        }
    }
    public Character Character
    {
        get
        {
            return m_Character;
        }
    }

    private void Awake()
    {
        m_PlayerInput = GetComponent< PlayerInput >();
        m_Slot = ( PlayerSlot )m_PlayerInput.playerIndex;
        m_Device = m_PlayerInput.devices[ 0 ];
        m_Character = new Character();
        m_Inputs = PlayerInput.actions.actionMaps[ 0 ].actions.ToArray();
    }

    protected void GetInput( Control a_Control, out Vector2 a_Value )
    {
        a_Value = m_Inputs[ ( int )a_Control ].ReadValue< Vector2 >();
    }

    protected void GetInput( Control a_Control, out bool a_Value )
    {
        a_Value = m_Inputs[ ( int )a_Control ].ReadValue< bool >();
    }

    protected void GetInput( Control a_Control, out float a_Value )
    {
        a_Value = m_Inputs[ ( int )a_Control ].ReadValue< float >();
    }

    public void ChangeCharacter( Character a_Character )
    {
        m_Character = a_Character;
        OnCharacterChange( a_Character );
    }

    protected virtual void OnCharacterChange( Character a_NewCharacter ) { }

    private PlayerSlot m_Slot;
    private ControlStage m_ControlStage;
    private InputDevice m_Device;
    private PlayerInput m_PlayerInput;
    private Character m_Character;
    private InputAction[] m_Inputs;

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