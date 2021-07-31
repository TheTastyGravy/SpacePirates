using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OxygenLevelDisplay : MonoBehaviour
{
	[System.Serializable]
	public struct DisplaySettings
	{
		[Tooltip("Below this percentage, this will be used for display"), Range(0, 100)]
		public float oxygenLevel;
		public Color color;
		public string message;
	}

	public RoomManager room;
	[Tooltip("How data should be displaied depending on the current oxygen level")]
	public DisplaySettings[] displaySettings;
	[Space]
	public TextMeshProUGUI messageLabel;
	public TextMeshProUGUI levelLabel;

	

    void LateUpdate()
    {
		if (room == null)
			return;

		float oxygenPercent = room.OxygenLevel / room.totalRoomOxygen * 100;

		// Find the display setting to use
		DisplaySettings display = displaySettings[0];
		for (int i = 1; i < displaySettings.Length; i++)
		{
			if (displaySettings[i].oxygenLevel > oxygenPercent)
			{
				display = displaySettings[i];
			}
			else
			{
				break;
			}
		}

		// Display message and percentage level
		messageLabel.text = "Level: " + display.message;
		levelLabel.text = oxygenPercent.ToString("0") + "%";
		// Set color
		messageLabel.color = display.color;
		levelLabel.color = display.color;
    }
}
