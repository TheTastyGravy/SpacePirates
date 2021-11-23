using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipTile : Tile
{
	public MenuButtonAngles angles;
	public UIAudioEventLogic audioEvent;
	[ Range( 1, 4 ) ] public int MaxPlayers = 1;
	[Header("Ship")]
	public Image image;
    public Material mat;
    [Header("Text")]
	public Image textImage;
	public Sprite baseTextImage;
	public Sprite selectTextImage;


	
	public void Init()
	{
        // Assign a new instance of the material so the actual values dont get changed
        image.material = Instantiate(mat);
        image.material.SetFloat("_EffectAmount", 1);
	}

	public void SetSelected(bool selected)
	{
		image.material.SetFloat("_EffectAmount", selected ? 0 : 1);
		textImage.sprite = selected ? selectTextImage : baseTextImage;

		if (selected)
		{
			angles.OnSelect(null);
			audioEvent.OnSelect(null);
		}
		else
		{
			angles.OnDeselect(null);
		}
	}
}