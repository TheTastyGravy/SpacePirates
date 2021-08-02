using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class ShrapnelLogic : MonoBehaviour
{
	public float minDamageToPlayer = 10;
	public float maxDamageToPlayer = 25;

	public float chanceToDamageStation = 0.05f;



	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			// Damage player
			collision.gameObject.GetComponent<Character>().ApplyHealthModifier(-Random.Range(minDamageToPlayer, maxDamageToPlayer));
		}
		else
		{
			// Random chance to damage a station
			DamageStation damageStation = collision.gameObject.GetComponentInChildren<DamageStation>();
			if (damageStation != null && Random.Range(0f, 1f) < chanceToDamageStation)
			{
				damageStation.Damage();
			}
		}

		Destroy(gameObject);
		SoundController.Instance.Play("ShrapnelImpact", false);
	}
}
