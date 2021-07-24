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
    }

    public void ChangeCharacter( Character a_Character )
    {
        m_Character = a_Character;
    }

    private PlayerSlot m_Slot;
    private InputDevice m_Device;
    private PlayerInput m_PlayerInput;
    private Character m_Character;

    public enum PlayerSlot
    {
        P1, P2, P3, P4,
        COUNT
    }
}