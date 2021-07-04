using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HelmStation : StationBase
{
	//currently, this only supports a single player. when the controller 
	//system is implemented, this needs to be updated to get input using that

	public float acceleration = 1;
	public float maxSpeed = 20;
	public float turnSpeed = 30;

	private Rigidbody rootRb;



	protected override void OnActivated()
	{
	}
	protected override void OnDeactivated()
	{
	}


	protected override void Start()
    {
		base.Start();

		rootRb = shipExternal.GetComponent<Rigidbody>();
	}

    void FixedUpdate()
    {
		if (!isActive)
			return;

		var keyboard = Keyboard.current;
		if (keyboard == null)
			return;

		//thrust
		Vector3 force = Vector3.zero;
		force.z += keyboard.wKey.ReadValue();
		force.z -= keyboard.sKey.ReadValue();
		rootRb.AddRelativeForce(force * acceleration, ForceMode.Acceleration);

		//max speed
		if (rootRb.velocity.sqrMagnitude > maxSpeed * maxSpeed)
		{
			float diff = (rootRb.velocity.magnitude - maxSpeed) / rootRb.velocity.magnitude;
			rootRb.velocity -= rootRb.velocity * diff;
		}

		//turning
		Vector3 torque = Vector3.zero;
		torque.y -= keyboard.aKey.ReadValue();
		torque.y += keyboard.dKey.ReadValue();
		rootRb.AddRelativeTorque(torque * turnSpeed, ForceMode.Acceleration);
	}
}
