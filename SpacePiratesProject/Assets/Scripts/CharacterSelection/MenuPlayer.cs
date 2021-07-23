using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPlayer : Player
{
    private void OnDPadPressed( InputValue a_Value )
    {
        Vector2 newDPad = a_Value.Get< Vector2 >();
        m_Controls_DPAD.x = ( int )newDPad.x;
        m_Controls_DPAD.y = ( int )newDPad.y;
    }

    private void OnAPressed()
    {

    }

    private void OnStartPressed()
    {

    }

    private Vector2Int m_Controls_DPAD;
    private bool m_Controls_A;
}