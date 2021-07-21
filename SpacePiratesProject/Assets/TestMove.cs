using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestMove : MonoBehaviour
{
    public float MovementSpeed = 10.0f;
    private void OnMove( InputValue a_Value )
    {
        Vector2 move = a_Value.Get< Vector2 >();
        transform.Translate( new Vector3( move.x, 0, move.y ) * MovementSpeed * Time.deltaTime );
    }
    private void OnDeviceLost()
    {

    }
    private void OnDeviceRegained()
    {

    }
}
