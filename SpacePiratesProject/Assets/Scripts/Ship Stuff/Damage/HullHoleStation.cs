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

    internal float oxygenLossRate;
    private float repairTime;

    private int size = 0;
    private Interactor currentInteractor;
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
        if (currentInteractor != null)
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
    }

    private void Repair()
	{
        // Unlock the player and update the interactor
        interactionPrompt.Pop();
        enabled = false;
        currentInteractor.Player.Character.enabled = true;
        currentInteractor.UpdateRegistry();
        currentInteractor = null;

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


    protected override void OnInteractStart(Interactor interactor)
	{
        currentInteractor = interactor;
        timePassed = 0;
        // Lock the player
        currentInteractor.Player.Character.enabled = false;
	}
	protected override void OnInteractStop(Interactor interactor)
	{
        if (currentInteractor == null)
            return;

        // Unlock player
        currentInteractor.Player.Character.enabled = true;
        currentInteractor = null;
    }

    protected override bool ShouldRegister(Interactor interactor, out Player.Control button)
    {
        button = Player.Control.A_PRESSED;
        return true;
    }
}
