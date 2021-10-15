using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipTile : Tile
{
    [ Range( 1, 4 ) ] public int MaxPlayers = 1;
	[Header("Ship")]
	public SpriteRenderer image;
	[Header("Text")]
	public Image textImage;
	public Sprite baseTextImage;
	public Sprite selectTextImage;

	private MaterialPropertyBlock propertyBlock;

	
	public void Init()
	{
		propertyBlock = new MaterialPropertyBlock();
		image.GetPropertyBlock(propertyBlock);
		propertyBlock.SetFloat("_EffectAmount", 1);
		image.SetPropertyBlock(propertyBlock);
	}

	public void SetSelected(bool selected)
	{
		propertyBlock.SetFloat("_EffectAmount", selected ? 0 : 1);
		image.SetPropertyBlock(propertyBlock);
		textImage.sprite = selected ? selectTextImage : baseTextImage;
	}
}