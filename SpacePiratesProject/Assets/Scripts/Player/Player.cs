using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : IPlayer
{
    private void FixedUpdate()
    {
        GetInput( Control.LEFT_STICK, out Vector2 value );
        // use value for movement.

        if ( GetInput( Control.A_PRESSED ))
        {
            // interact
        }
    }
}
