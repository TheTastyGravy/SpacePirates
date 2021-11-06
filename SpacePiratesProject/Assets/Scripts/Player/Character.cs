using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ RequireComponent( typeof( Rigidbody ) ) ]
public class Character : ICharacter
{
    [ Header( "Movement" ) ]
    public float MoveSpeed = 1;
    public float TurnSpeed = 10;

    public bool IsKinematic
    {
        get
        {
            return !m_Rigidbody.isKinematic;
        }
        set
        {
            m_Rigidbody = m_Rigidbody ?? GetComponent< Rigidbody >();
            m_Rigidbody.isKinematic = value;
            if (!value)
                BeginGame();
        }
    }
    private Rigidbody m_Rigidbody;

    private Player.Control[] cheatCode;
    private int cheatIndex = 0;
    private bool cheatActivated = false;
    public static BasicDelegate OnCheatActivated;



    protected override void Start()
    {
        base.Start();

        m_Rigidbody = GetComponent< Rigidbody >();
        enabled = false;
        // Only P1 can activate the cheat code
        if (Player.Slot == Player.PlayerSlot.P1)
		{
            // Setup callbacks
            Player.AddInputListener(Player.Control.DPAD_PRESSED, OnInputEvent);
            Player.AddInputListener(Player.Control.B_PRESSED, OnInputEvent);
            Player.AddInputListener(Player.Control.A_PRESSED, OnInputEvent);
            Player.AddInputListener(Player.Control.START_PRESSED, OnInputEvent);
            // Illegal info
            cheatCode = new Player.Control[]
            {
                Player.Control.COUNT + 0,
                Player.Control.COUNT + 0,
                Player.Control.COUNT + 2,
                Player.Control.COUNT + 2,
                Player.Control.COUNT + 3,
                Player.Control.COUNT + 1,
                Player.Control.COUNT + 3,
                Player.Control.COUNT + 1,
                Player.Control.B_PRESSED,
                Player.Control.A_PRESSED,
                Player.Control.START_PRESSED
            };
        }
    }

	void OnDestroy()
	{
		if (Player.Slot == Player.PlayerSlot.P1)
		{
            Player.RemoveInputListener(Player.Control.DPAD_PRESSED, OnInputEvent);
            Player.RemoveInputListener(Player.Control.B_PRESSED, OnInputEvent);
            Player.RemoveInputListener(Player.Control.A_PRESSED, OnInputEvent);
            Player.RemoveInputListener(Player.Control.START_PRESSED, OnInputEvent);
        }
	}

	void FixedUpdate()
    {
        Player.GetInput( Player.Control.LEFT_STICK, out Vector3 input );

        Vector3 movement;
        if (Camera.main != null)
		{
            // Get camera directions on the Y plane
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0;
            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0;
            // Calculate movement using the camera
            movement = camForward.normalized * input.z + camRight.normalized * input.x;
        }
        else
		{
            movement = input;
		}

        // Set animator value
        currentCharacter.animator.SetFloat("Speed", movement.magnitude);

        movement *= MoveSpeed;
        // Set velocity directly, keeping the y component when its negitive
        m_Rigidbody.velocity = new Vector3(movement.x, Mathf.Min(m_Rigidbody.velocity.y, 0), movement.z);

		// Rotate over time towards the direction of movement
		Vector3 forward = Vector3.Slerp( transform.forward, movement, Time.fixedDeltaTime * TurnSpeed );
		Quaternion quat = Quaternion.FromToRotation( transform.forward, forward );
		m_Rigidbody.MoveRotation( m_Rigidbody.rotation * quat );
    }

	void OnDisable()
	{
        m_Rigidbody.velocity = Vector3.zero;
        currentCharacter.animator.SetFloat("Speed", 0);
    }

    public void ResetCheat()
    {
        cheatActivated = false;
        cheatIndex = 0;
        // Remove all listeners
        if (OnCheatActivated != null)
        {
            foreach (Delegate obj in OnCheatActivated.GetInvocationList())
            {
                OnCheatActivated -= obj as BasicDelegate;
            }
        }
    }

    // Only used for player 1 to enter cheat code
    private void OnInputEvent(InputAction.CallbackContext context)
    {
        if (cheatActivated)
            return;

        // Get action, correcting for dpad
        InputAction correctAction = Player.GetInputAction(cheatCode[cheatIndex] >= Player.Control.COUNT ? Player.Control.DPAD_PRESSED : cheatCode[cheatIndex]);
        if (context.action == correctAction)
        {
            // If its a direction, check dpad input
            if (cheatCode[cheatIndex] >= Player.Control.COUNT)
            {
                Vector2 val = context.ReadValue<Vector2>();
                if (!((val.y == 1 && cheatCode[cheatIndex] == Player.Control.COUNT + 0) ||//up
                    (val.x == 1 && cheatCode[cheatIndex] == Player.Control.COUNT + 1) ||  //right
                    (val.y == -1 && cheatCode[cheatIndex] == Player.Control.COUNT + 2) || //down
                    (val.x == -1 && cheatCode[cheatIndex] == Player.Control.COUNT + 3)))  //left
                {
                    // Fail
                    cheatIndex = 0;
                    return;
                }
            }

            // Continue with code
            cheatIndex++;
            if (cheatIndex == cheatCode.Length)
            {
                cheatActivated = true;
                if (OnCheatActivated != null)
                    OnCheatActivated.Invoke();
                Debug.Log("<size=15><color=red>CHEAT ACTIVATED</color></size>");
            }
        }
        else
        {
            // Fail
            cheatIndex = 0;
        }
    }
}