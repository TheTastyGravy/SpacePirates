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
		if (image != null)
			image.sprite = selected ? selectImage : baseImage;
		if (textImage != null)
			textImage.sprite = selected ? selectTextImage : baseTextImage;

		if (selected)
		{
			if (angles != null)
				angles.OnSelect(null);
			audioEvent.OnSelect(null);
		}
		else
		{
			if (angles != null)
				angles.OnDeselect(null);
		}
	}
}