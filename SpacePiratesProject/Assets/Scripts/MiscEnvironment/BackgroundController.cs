using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public MeshRenderer[] backgroundRenderers;
    public ParticleSystem effect;
    [HideInInspector]
    public float speedMultiplier = 1;
    private float customTime = 0;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.TrailModule trailModule;
    private float baseSpeed;
    private float baseEmission;
    private ParticleSystem.MinMaxGradient trailGradient;



    void Start()
    {
        mainModule = effect.main;
        emissionModule = effect.emission;
        trailModule = effect.trails;
        baseSpeed = mainModule.startSpeed.constant;
        baseEmission = emissionModule.rateOverTime.constant;
        trailGradient = trailModule.colorOverLifetime;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            new Vector3(-0.5f, 0.5f, 0.5f));

        customTime += Time.deltaTime * speedMultiplier;
        if (float.IsNaN(customTime))
            customTime = 0;
        foreach (var obj in backgroundRenderers)
		{
            obj.material.SetFloat("_ScaledTime", customTime);
        }

        mainModule.startSpeedMultiplier = baseSpeed * speedMultiplier;
        emissionModule.rateOverTimeMultiplier = baseEmission * speedMultiplier;
        trailGradient.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, (Mathf.Clamp(speedMultiplier, 0.15f, 0.35f) - 0.15f) * 5);
        trailModule.colorOverLifetime = trailGradient;
    }
}
