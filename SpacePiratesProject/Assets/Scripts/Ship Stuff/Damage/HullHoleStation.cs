using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class HullHoleStation : Interactable
{
    [System.Serializable]
    public struct DamageLevel
	{
        public float oxygenLossRate;
        public float repairTime;
        public float scale;
	}
    [Space]
    public DamageLevel[] damageLevels;
    public ParticleSystem damageEffect;
    public Transform[] scalableTransforms;
    [Space]
    public EventReference repairEvent;
    public EventReference repairEndEvent;

    internal float oxygenLossRate;
    private float repairTime;

    private int size = 0;
    private float timePassed = 0;
    private FMOD.Studio.EventInstance repairEventInstance;

    internal RoomManager room;
    internal int holeIndex;



    void Start()
    {
        oxygenLossRate = damageLevels[0].oxygenLossRate;
        repairTime = damageLevels[0].repairTime;
        repairEventInstance = RuntimeManager.CreateInstance(repairEvent);
        repairEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(Vector3.zero));

        foreach (Transform obj in scalableTransforms)
        {
            obj.localScale = new Vector3(damageLevels[0].scale, damageLevels[0].scale, damageLevels[0].scale);
        }
    }

    protected override void OnDestroy()
	{
        base.OnDestroy();
        repairEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        repairEventInstance.release();
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

        foreach (Transform obj in scalableTransforms)
        {
            obj.localScale = new Vector3(damageLevels[size].scale, damageLevels[size].scale, damageLevels[size].scale);
        }
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
        // Stop sound effect, and play end sound
        repairEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        RuntimeManager.PlayOneShot(repairEndEvent);
    }

    protected override void OnInteractionStart()
	{
        timePassed = 0;
        // Start sound effect
        repairEventInstance.start();
    }

	protected override void OnButtonUp()
	{
        currentInteractor.EndInteraction();
        // Stop sound effect
        repairEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        RuntimeManager.PlayOneShot(repairEndEvent);
    }

    protected override bool CanBeUsed(Interactor interactor, out Player.Control button)
    {
        button = Player.Control.A_PRESSED;
        return true;
    }
}
