using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSpeedLogic : MonoBehaviour
{
    public BackgroundController background;
    [Space]
    public float minSpeedModifier = 0.1f;
    public float maxSpeedModifier = 1;



    void Start()
    {

    }

    void LateUpdate()
    {
        float value = Mathf.Lerp(minSpeedModifier, maxSpeedModifier, ShipManager.Instance.GetShipSpeed() / ShipManager.Instance.GetMaxSpeed());

        background.speedMultiplier = value;
    }
}
