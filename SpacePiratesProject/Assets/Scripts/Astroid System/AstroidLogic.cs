using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroidLogic : MonoBehaviour
{
	private Vector3 direction;
	private float speed = 10;



	internal void Setup(Vector3 startPos, Vector3 direction)
	{
		transform.position = startPos;
		this.direction = direction.normalized;
	}


	void Update()
	{
		transform.position += speed * Time.deltaTime * direction;
	}

	void OnCollisionEnter(Collision collision)
	{
		ShipManager.Instance.DamageShipAtPosition(transform.position);
		Destroy(gameObject);
	}
}
