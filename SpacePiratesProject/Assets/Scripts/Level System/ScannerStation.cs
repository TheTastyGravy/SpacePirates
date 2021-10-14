using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerStation : MonoBehaviour
{
    private BasicSwitch activateSwitch;
    private DamageStation damage;



    void Start()
    {
        activateSwitch = GetComponentInChildren<BasicSwitch>();
        damage = GetComponentInChildren<DamageStation>();

        activateSwitch.OnActivated += OnSwitchUsed;
        // The switch can not be used while damaged
		damage.OnDamageTaken += () => { activateSwitch.enabled = false; activateSwitch.forceDisabled = true; };
		damage.OnDamageRepaired += () => { activateSwitch.enabled = true; activateSwitch.forceDisabled = false; };
	}

    private void OnSwitchUsed()
	{
        TimelineController.Instance.Ping();
	}
}
