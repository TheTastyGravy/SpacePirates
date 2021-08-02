using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EngineStation : Interactable
{
	public enum EngineRegion
	{
		CENTER,
		LEFT,
		RIGHT
	}
	public EngineRegion region;

    // The current power level of this engine. 0-2
    private int powerLevel = 0;
    public int PowerLevel { get => powerLevel; }
	public TextMeshProUGUI powerLabel;

	private DamageStation damage;



	void Awake()
	{
		damage = GetComponentInChildren<DamageStation>();
	}

	void Start()
	{
		UpdateText();
		SoundManager.Instance.Play("Engine", true);
	}

	protected override void OnActivate(Interactor user)
    {
        // Increase power level, looping
        powerLevel++;

        bool isDamaged = damage == null;
        if (!isDamaged)
            isDamaged = damage.DamageLevel > 0;
        // If we are damaged, there are 2 power levels
        if (powerLevel > (isDamaged ? 1 : 2))
            powerLevel = 0;

		UpdateText();
	}

	private void UpdateText()
	{
		if (powerLabel == null)
			return;

		Color color = Color.white;
		switch (powerLevel)
		{
			case 0:
				color = new Color(0, 0.5f, 0); //dark green
				break;
			case 1:
				color = new Color(0.8f, 0.8f, 0); //dark yellow
				break;
			case 2:
				color = Color.red;
				break;
		}

		// Set text and color
		powerLabel.text = (powerLevel + 1).ToString();
		powerLabel.color = color;
	}
}
