using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScannerStation : MonoBehaviour
{
    public Image baseImage;
    public Image fillImage;

    private BasicSwitch activateSwitch;
    private DamageStation damage;
    private float timePassed = 0;



    void Start()
    {
        activateSwitch = GetComponentInChildren<BasicSwitch>();
        damage = GetComponentInChildren<DamageStation>();
        activateSwitch.OnActivated += OnSwitchUsed;
        // The switch can not be used while damaged
		damage.OnDamageTaken += () => { activateSwitch.enabled = false; activateSwitch.forceDisabled = true; activateSwitch.interactionPrompt.Pop(false); };
		damage.OnDamageRepaired += () => { activateSwitch.enabled = true; activateSwitch.forceDisabled = false; };

        // Set cooldown using difficulty settings
        activateSwitch.interactionCooldown = GameManager.GetDifficultySettings().interactionCooldown.Value;
    }

    void Update()
    {
        if (timePassed > activateSwitch.interactionCooldown)
            return;

        // Update timer
        float value = timePassed / activateSwitch.interactionCooldown;
        baseImage.fillAmount = 1 - value;
        fillImage.fillAmount = value;
        timePassed += Time.deltaTime;
    }

    private void OnSwitchUsed()
	{
        TimelineController.Instance.Ping();
        timePassed = 0;
    }
}
