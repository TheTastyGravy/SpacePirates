using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerSlot Slot { get; protected set; }
    public InputDevice Device { get; protected set; }
    public PlayerInput PlayerInput { get; protected set; }
    public int CharacterIndex { get; protected set; }
    public int CharacterVariant { get; protected set; }

    public enum PlayerSlot
    {
        P1, P2, P3, P4,
        COUNT
    }
}