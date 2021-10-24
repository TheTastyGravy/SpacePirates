using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusManager : Singleton<StatusManager>
{
    public TextMeshProUGUI text;
    public string general;
    [Space]
    public string reactorDamaged;
    public string reactorDisabled;
    [Space]
    public string generalRepair;
    public string generalRefuel;
    [Space]
    public string useEngines;
    public string useTurrets;
    [Space]
    public string asteroidEvent;
    public string stormEvent;
    public string shipEvent;

    private bool eventActive = false;
    private EngineStation[] engines;
    private TurretStation[] turrets;



    void Start()
    {
        text.text = general;
        EventManager.Instance.OnEventChange += OnEventChange;
        Invoke(nameof(Init), 0.1f);
    }

	private void Init()
	{
        engines = ShipManager.Instance.gameObject.GetComponentsInChildren<EngineStation>();
        turrets = ShipManager.Instance.gameObject.GetComponentsInChildren<TurretStation>();
        InvokeRepeating(nameof(UpdateText), 5, 2);
    }

	void OnDestroy()
	{
        if (EventManager.Instance != null)
            EventManager.Instance.OnEventChange -= OnEventChange;
    }

	void UpdateText()
	{
        // Events override everything else
        if (eventActive)
            return;

        if (ShipManager.Instance.Reactor.Damage.DamageLevel > 0)
		{
            text.text = reactorDamaged;
            return;
        }
        if (!ShipManager.Instance.Reactor.IsTurnedOn)
		{
            text.text = reactorDisabled;
            return;
        }

        if ((ShipManager.Instance.OxygenLevel < 90 && ShipManager.Instance.oxygenDrain > 2) || (ShipManager.Instance.OxygenLevel < 50 && ShipManager.Instance.oxygenDrain > 0))  //maybe also check num damaged stations
		{
            text.text = generalRepair;
            return;
        }

        int count = 0;
        foreach (var obj in engines)
		{
            if (obj.CurrentFuel == 0)
			{
                count++;
            }
		}
        foreach (var obj in turrets)
		{
            if (obj.ShotsRemaining == 0)
			{
                count++;
            }
		}
        if (count > 1)
		{
            text.text = generalRefuel;
            return;
		}

        if (ShipManager.Instance.GetShipSpeed() == 0)
		{
            text.text = useEngines;
            return;
        }

        text.text = general;
	}

    public void OnEventChange(Level.Event.Type eventType)
	{
        eventActive = eventType != Level.Event.Type.None;

        if (eventActive)
		{
            switch (eventType)
			{
                case Level.Event.Type.AstroidField:
                    text.text = asteroidEvent;
                    break;
                case Level.Event.Type.PlasmaStorm:
                    text.text = stormEvent;
                    break;
                case Level.Event.Type.ShipAttack:
                    text.text = shipEvent;
                    break;
			}
		}
		else
		{
            UpdateText();
		}
	}
}
