using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullHoleStation : Interactable
{
    [System.Serializable]
    public struct DamageLevel
	{
        public float oxygenLossRate;
        public float repairTime;
	}
    [Space]
    public DamageLevel[] damageLevels;
    public ParticleSystem damageEffect;

    internal float oxygenLossRate;
    private float repairTime;

    private int size = 0;
    private float timePassed = 0;

    internal RoomManager room;
    internal int holeIndex;



    void Start()
    {
        oxygenLossRate = damageLevels[0].oxygenLossRate;
        repairTime = damageLevels[0].repairTime;
	}

    void Update()
    {
        // If we are being interacted with
        if (IsBeingUsed)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= repairTime)
            {
                Repair();
            }

            // Update interaction prompt progress
            interactionPrompt.interactionProgress = timePassed / repairTime;
        }
    }

    internal void IncreaseHoleSize()
    {
        size++;
        if (size >= damageLevels.Length)
            size = damageLevels.Length - 1;

        oxygenLossRate = damageLevels[size].oxygenLossRate;
        repairTime = damageLevels[size].repairTime;
        timePassed = 0;
        room.RecalculateOxygenDrain();

        damageEffect.Play();
    }

    private void Repair()
	{
        // Unlock the player and update the interactor
        interactionPrompt.Pop();
        enabled = false;
        currentInteractor.EndInteraction();

        // Repair hole
        size = 0;
        room.OnHoleDestroied(holeIndex);
        // Hide visuals, and destroy after delay to keep prompt pop effect
        foreach (var obj in GetComponentsInChildren<Renderer>())
		{
            obj.enabled = false;
		}
        GetComponentInChildren<ParticleSystem>().Stop();
        Destroy(gameObject, 1);
    }

    protected override void OnInteractionStart()
	{
        timePassed = 0;
	}

	protected override void OnButtonUp()
	{
        currentInteractor.EndInteraction();
    }

    protected override bool CanBeUsed(Interactor interactor, out Player.Control button)
    {
        button = Player.Control.A_PRESSED;
        return true;
    }
}
