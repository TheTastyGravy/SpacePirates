using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretStation : MonoBehaviour
{
    public Transform playerActivatedTrans;
    public Transform turretBase;
    public Transform firePos;
    public GameObject projectilePrefab;
    [Space]
    public int shotsPerFuel = 5;
    public int maxShots = 5;
    public float projectileSpeed = 10;
    public float minAngle, maxAngle;

    public GameObject turretHud;


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
    private bool firstFire;



    void Start()
    {
        turretActivate = GetComponentInChildren<TurretActivate>();
        damage = GetComponentInChildren<DamageStation>();
        fuelDepo = GetComponentInChildren<FuelDeposit>();

        turretActivate.enabled = false;
        turretHud.SetActive(false);

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
        // Display HUD
        turretHud.SetActive(true);

        currentInteractor.SetActive(false);
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
        firstFire = true;
    }
    // Used to remove a players controls to use the turret
    private void RemovePlayer()
	{
        // Hide HUD
        turretHud.SetActive(false);

        //remove turret controls
        currentInteractor.Player.RemoveInputListener(Player.Control.A_PRESSED, Fire);
        currentInteractor.Player.RemoveInputListener(Player.Control.B_PRESSED, OnDeactivate);
        
        //reset player info
        currentInteractor.Player.Character.transform.SetPositionAndRotation(playerPos, playerRot);
        //activate controls
        (currentInteractor.Player.Character as Character).IsKinematic = false;
        (currentInteractor.Player.Character as Character).enabled = true;
        currentInteractor.SetActive(true);
    }

    private void Fire(InputAction.CallbackContext _)
	{
        // Fix for firing on activation
        if (firstFire)
		{
            firstFire = false;
            return;
        }

        // Shoot projectile in direction of turretBase
        GameObject projectile = Instantiate(projectilePrefab, firePos.position, firePos.rotation);
        projectile.GetComponent<Rigidbody>().AddForce(turretBase.forward * projectileSpeed, ForceMode.Impulse);
        Destroy(projectile, 5);

        fuelDepo.enabled = true;

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


        // Get the desiered direction
        Vector3 direction;
        if (currentInteractor.Player.Device is Gamepad)
		{
            // Use stick with larger magnitude
            currentInteractor.Player.GetInput(Player.Control.LEFT_STICK, out Vector3 left);
            currentInteractor.Player.GetInput(Player.Control.RIGHT_STICK, out Vector3 right);
            direction = (left.sqrMagnitude > right.sqrMagnitude) ? left : right;
		}
		else // Player is using keyboard
		{
            // Cast ray from cursor to plane
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            new Plane(Vector3.up, 0).Raycast(ray, out float enter);
            // Use the hit point relitive to the player as the direction
            direction = ray.GetPoint(enter) - turretBase.position;
            direction.y = 0;
        }

        if (direction.sqrMagnitude < 0.15f)
            return;

        // Limit the angle of the direction
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        if (angle < minAngle)
            RotateDirection(ref direction, (angle - minAngle) * Mathf.Deg2Rad);
        else if (angle > maxAngle)
            RotateDirection(ref direction, (angle - maxAngle) * Mathf.Deg2Rad);

        turretBase.forward = direction;
	}

    private void RotateDirection(ref Vector3 vec, float angle)
	{
        Vector3 newVec = Vector3.zero;
        newVec.x = vec.x * Mathf.Cos(angle) - vec.z * Mathf.Sin(angle);
        newVec.z = vec.x * Mathf.Sin(angle) + vec.z * Mathf.Cos(angle);
        vec = newVec;
    }
}
