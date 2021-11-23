using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSettings : MonoBehaviour
{
    public string characterName;
    public Material[] variants;
	public Renderer[] renderers;
    public Transform grabTransform;


	// Called by ICharacter in Awake. Avoids race events
	internal void Init()
	{
		if (variants.Length == 0)
		{
			variants = new Material[1];
			variants[0] = renderers[0].sharedMaterial;
		}
	}
}
