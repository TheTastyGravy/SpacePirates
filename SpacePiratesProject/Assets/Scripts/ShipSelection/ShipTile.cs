using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipTile : Tile
{
	public MenuButtonAngles angles;
    [ Range( 1, 4 ) ] public int MaxPlayers = 1;
	[Header("Ship")]
	public Image image;
	[Header("Text")]
	public Image textImage;
	public Sprite baseTextImage;
	public Sprite selectTextImage;

	private MaterialPropertyBlock propertyBlock;

	
	public void Init()
	{
		propertyBlock = new MaterialPropertyBlock();
		image.materialForRendering.SetFloat("_EffectAmount", 1);
	}

	public void SetSelected(bool selected)
	{
		image.materialForRendering.SetFloat("_EffectAmount", selected ? 0 : 1);
		textImage.sprite = selected ? selectTextImage : baseTextImage;

		if (selected)
			angles.OnSelect(null);
		else
			angles.OnDeselect(null);
	}
}