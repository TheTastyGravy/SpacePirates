using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class TurretStation : MonoBehaviour
{
    public Transform playerActivatedTrans;
    public Transform turretBase;
    public TurretRecoil recoilScript;
    public Transform firePos;
    public GameObject projectilePrefab;
    [Space]
    public float projectileSpeed = 10;
    public float minAngle, maxAngle;
    [Tooltip("The angle that the turret begins at. Used for custom models")]
    public float baseAngle = 0;
    [Tooltip("A value used for tweeking the snap direction")]
    public float snapAngle = 0;
    [Space]
    public GameObject turretHud;
    public FuelIndicator fuelIndicator;

    private TurretActivate turretActivate;
    private DamageStation damage;
    private FuelDeposit fuelDepo;
    private ParticleSystem shootEffect;
    private Renderer laser;
    private bool isTurnedOn = false;
    public bool IsTurnedOn => isTurnedOn;
    private Interactor currentInteractor = null;
    // Info about player before we move them into the turret
    private Vector3 playerPos;
    private Quaternion playerRot;
    private int shotsPerFuel = 5;
    private int maxShots = 5;
    private int startShots = 0;
    private int shotsRemaining = 0;
    public int ShotsRemaining => shotsRemaining;
    private bool firstFire;
    private Vector3 relitiveForward;



    void Start()
    {
        turretActivate = GetComponentInChildren<TurretActivate>();
        damage = GetComponentInChildren<DamageStation>();
        fuelDepo = GetComponentInChildren<FuelDeposit>();
        shootEffect = firePos.GetComponentInChildren<ParticleSystem>();
        laser = firePos.GetComponentInChildren<MeshRenderer>();

        if (laser != null)
            laser.enabled = false;
        if (turretHud != null)
            turretHud.SetActive(false);

        // Get values from difficulty settings
        LevelDificultyData.DiffSetting setting = GameManager.GetDifficultySettings();
        maxShots = setting.maxShots.Value;
        shotsPerFuel = setting.shotsPerFuel.Value;
        startShots = setting.startShots.Value;
        
        shotsRemaining = startShots;
        Invoke(nameof(FixFuelIndicator), 0.1f);
        // Get a direction to use as forward for aiming the turret
        relitiveForward = Vector3.forward;
        RotateDirection(ref relitiveForward, (baseAngle + snapAngle) * Mathf.Deg2Rad);

        TryTurnOn();

        // Setup callbacks
        turretActivate.OnInteract += OnActivate;
        damage.OnDamageTaken += TurnOff;
        damage.OnDamageRepaired += TryTurnOn;
        fuelDepo.OnFuelDeposited += OnFueled;
    }

    private void FixFuelIndicator()
	{
        fuelIndicator.SetFuelLevel((float)shotsRemaining / (float)maxShots * 100f);
        if (shotsRemaining >= maxShots)
            fuelDepo.enabled = false;
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
        if (shotsRemaining >= maxShots)
		{
            shotsRemaining = maxShots;
            fuelDepo.enabled = false;
        }

        fuelIndicator.SetFuelLevel((float)shotsRemaining / (float)maxShots * 100f);

        TryTurnOn();
	}


    // Used to setup a players controls to use the turret
    private void AddPlayer()
	{
        // Display HUD and aiming laser
        if (turretHud != null)
            turretHud.SetActive(true);
        if (laser != null)
            laser.enabled = true;

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
        // Hide HUD and aiming laser
        if (turretHud != null)
            turretHud.SetActive(false);
        if (laser != null)
            laser.enabled = false;

        //remove turret controls
        currentInteractor.Player.RemoveInputListener(Player.Control.A_PRESSED, Fire);
        currentInteractor.Player.RemoveInputListener(Player.Control.B_PRESSED, OnDeactivate);
        //reset player info
        currentInteractor.Player.Character.transform.SetPositionAndRotation(playerPos, playerRot);

        currentInteractor.EndInteraction();
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
        projectile.GetComponent<Rigidbody>().AddForce(firePos.forward * projectileSpeed, ForceMode.Impulse);
        Destroy(projectile, 5);
        // Rotate child containing collider to align with camera. This is to create more consistant collisions visualy
        projectile.transform.GetChild(0).rotation = Camera.main.transform.rotation;
        // Play explosion effect at fire pos
        if (shootEffect != null)
            shootEffect.Play();
        if (recoilScript != null)
            recoilScript.Run();

        fuelDepo.enabled = true;

        shotsRemaining--;
        if (shotsRemaining == 0)
		{
            TurnOff();
		}

        fuelIndicator.SetFuelLevel((float)shotsRemaining / (float)maxShots * 100f);
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

            // Get camera directions on the Y plane
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0;
            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0;
            // Calculate movement using the camera
            direction = camForward.normalized * direction.z + camRight.normalized * direction.x;
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

        direction.Normalize();
        
        // Limit the angle of the direction
        float angle = Vector3.SignedAngle(relitiveForward, direction, Vector3.up) + baseAngle;
        if (angle < minAngle)
            RotateDirection(ref direction, (angle - minAngle) * Mathf.Deg2Rad);
        else if (angle > maxAngle)
            RotateDirection(ref direction, (angle - maxAngle) * Mathf.Deg2Rad);

        RotateDirection(ref direction, baseAngle * Mathf.Deg2Rad);
        turretBase.forward = direction;
	}

    private void RotateDirection(ref Vector3 vec, float angle)
	{
        Vector3 newVec = Vector3.zero;
        newVec.x = vec.x * Mathf.Cos(angle) - vec.z * Mathf.Sin(angle);
        newVec.z = vec.x * Mathf.Sin(angle) + vec.z * Mathf.Cos(angle);
        vec = newVec;
    }

	void OnDestroy()
	{
        if (currentInteractor != null)
		{
            RemovePlayer();
        }
	}

	
#if UNITY_EDITOR
    [DrawGizmo(GizmoType.InSelectionHierarchy, typeof(TurretStation))]
    private static void DrawGizmos(TurretStation turretStation, GizmoType gizmoType)
    {
        if (!turretStation.enabled)
            return;

        Vector3 relForward = Vector3.forward;
        turretStation.RotateDirection(ref relForward, (turretStation.baseAngle + turretStation.snapAngle) * Mathf.Deg2Rad);
        float radius = 1.75f;
        float thickness = 5;

        // Aim arc
        Vector3 minDir = Quaternion.Euler(0, turretStation.minAngle - turretStation.baseAngle, 0) * relForward;
        Handles.DrawWireArc(turretStation.turretBase.position, Vector3.up, minDir, turretStation.maxAngle - turretStation.minAngle, radius, thickness);
        // Min snap arc
        Handles.color = Color.red;
        Handles.DrawWireArc(turretStation.turretBase.position, Vector3.up, -relForward, turretStation.minAngle + (180 - turretStation.baseAngle), radius, thickness);
        // Max snap arc
        Handles.color = Color.green;
        Handles.DrawWireArc(turretStation.turretBase.position, Vector3.up, -relForward, turretStation.maxAngle - (180 + turretStation.baseAngle), radius, thickness);
    }
#endif
}
