using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : IPlayer
{
    private void FixedUpdate()
    {
        GetInput( Control.LEFT_STICK, out Vector2 value1 );
        // use value for movement.
        GetInput( Control.DPAD_PRESSED, out Vector2 value2 );
        if ( value2 != Vector2.zero )
        {
            // interact
        }
    }
}
