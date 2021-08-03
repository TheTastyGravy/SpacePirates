using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorColorLogic : MonoBehaviour
{
	public Color[] colors;

	private DamageStation damage;
	public Renderer render;



    void Start()
    {
		damage = GetComponent<DamageStation>();
    }

    void LateUpdate()
    {
		render.material.color = colors[damage.DamageLevel];
    }
}
