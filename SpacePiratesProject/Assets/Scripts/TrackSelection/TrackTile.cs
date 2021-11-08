using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackTile : Tile
{
	public MenuButtonAngles angles;
	public UIAudioEventLogic audioEvent;
	[Header("Arrow")]
	public Image image;
	public Sprite baseImage;
	public Sprite selectImage;
	[Header("Text")]
	public Image textImage;
	public Sprite baseTextImage;
	public Sprite selectTextImage;


	public void SetSelected(bool selected)
	{
		image.sprite = selected ? selectImage : baseImage;
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