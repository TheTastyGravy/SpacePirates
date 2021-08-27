using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretStation : MonoBehaviour
{
    public int shotsPerFuel = 5;
    public int maxShots = 5;

    public Transform playerActivatedTrans;
    public Transform turretBase;


    private TurretActivate turretActivate;
    private DamageStation damage;
    private FuelDeposit fuelDepo;

    private bool isTurnedOn = false;
    public bool IsTurnedOn => isTurnedOn;

    private Interactor currentInteractor = null;
    // Info about player before we move them into the turret
    private Vector3 playerPos;
    private Quaternion playerRot;

    private int shotsRemaining = 0;



    void Start()
    {
        turretActivate = GetComponentInChildren<TurretActivate>();
        damage = GetComponentInChildren<DamageStation>();
        fuelDepo = GetComponentInChildren<FuelDeposit>();

        // Setup callbacks
        turretActivate.OnInteract += OnActivate;
        damage.OnDamageTaken += TurnOff;
        damage.OnDamageRepaired += TryTurnOn;
        fuelDepo.OnFuelDeposited += OnFueled;
    }

    private void TurnOff()
	{
        isTurnedOn = false;

        //disable use
        turretActivate.enabled = false;
        //kick player out
        if (currentInteractor != null)
		{
            RemovePlayer();
            currentInteractor = null;
        }
    }
    private void TryTurnOn()
	{
        if (isTurnedOn || damage.DamageLevel > 0 || shotsRemaining == 0)
		{
            return;
		}

        isTurnedOn = true;
        turretActivate.enabled = true;
    }


    // Called when a player activates the turret
    private void OnActivate(Interactor interactor)
	{
        if (currentInteractor != null)
            return;


        currentInteractor = interactor;
        turretActivate.enabled = false;
        AddPlayer();
    }
    // Called when a player in the turret exits
    private void OnDeactivate(InputAction.CallbackContext _)
	{
        RemovePlayer();
        currentInteractor = null;

        if (isTurnedOn)
		{
            turretActivate.enabled = true;
        }
    }

    // Called when the fuel deposit has been used
    private void OnFueled()
	{
        shotsRemaining += shotsPerFuel;
        if (shotsRemaining > maxShots)
		{
            shotsRemaining = maxShots;
            fuelDepo.enabled = false;
        }

        TryTurnOn();
	}



    // Used to setup a players controls to use the turret
    private void AddPlayer()
	{
        //disable controls
        (currentInteractor.Player.Character as Character).IsKinematic = true;
        (currentInteractor.Player.Character as Character).enabled = false;
        //get info
        playerPos = currentInteractor.Player.Character.transform.position;
        playerRot = currentInteractor.Player.Character.transform.rotation;
        //move character
        currentInteractor.Player.Character.transform.SetPositionAndRotation(playerActivatedTrans.position, playerActivatedTrans.rotation);

        //setup turret controls
        currentInteractor.Player.AddInputListener(Player.Control.A_PRESSED, Fire);
        currentInteractor.Player.AddInputListener(Player.Control.B_PRESSED, OnDeactivate);
    }
    // Used to remove a players controls to use the turret
    private void RemovePlayer()
	{
        //remove turret controls
        currentInteractor.Player.RemoveInputListener(Player.Control.A_PRESSED, Fire);
        currentInteractor.Player.RemoveInputListener(Player.Control.B_PRESSED, OnDeactivate);
        
        //reset player info
        currentInteractor.Player.Character.transform.SetPositionAndRotation(playerPos, playerRot);
        //activate controls
        (currentInteractor.Player.Character as Character).IsKinematic = false;
        (currentInteractor.Player.Character as Character).enabled = true;
    }


    private void Fire(InputAction.CallbackContext _)
	{
        Debug.Log("BANG");
        //shoot projectile/raycast in direction of turretBase to hit astroids

        //fix firing on activation

        shotsRemaining--;
        if (shotsRemaining == 0)
		{
            TurnOff();
		}
	}

	void Update()
	{
        if (currentInteractor == null)
            return;

        //aim turret
	}
}
