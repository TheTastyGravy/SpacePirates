using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidFieldEffect : MonoBehaviour
{
    public ParticleSystem[] asteroidSystems;
    public ParticleSystem[] dustSystems;
    public float minAsteroidSpeed = 0.25f;

    private Coroutine routine;
    private int maxCount;
    private ParticleSystem.NoiseModule[] asteroidNoise;
    private ParticleSystem.EmissionModule[] asteroidEmission;
    private float[] asteroidBaseEmission;
    private float[] asteroidBaseSpeed;
    private ParticleSystem.EmissionModule[] dustEmission;
    private float[] dustBaseEmission;
    private float[] dustBaseSpeed;
    [HideInInspector]
    public float speedModifier = 1;



    private IEnumerator Start()
    {
        yield return null;
        Init();
    }

    private void Init()
    {
        Transform trans = transform;
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector2(1, 1));    //top right corner
        new Plane(Vector3.up, 0).Raycast(ray, out float enter);
        trans.position = ray.GetPoint(enter);

        // Add offset to make sure they are spawned off screen
        Vector3 dir = cam.transform.TransformPoint(new Vector2(0.5f, 0.45f)) - cam.transform.TransformPoint(new Vector2(0, 0));
        trans.position += dir.normalized * 7.5f;
        // Push down and up to spawn behind the player ship
        trans.position += new Vector3(-1, -1, 0) * 2;

        maxCount = 0;
        //  ----- Asteroid Systems -----
        asteroidNoise = new ParticleSystem.NoiseModule[asteroidSystems.Length];
        asteroidEmission = new ParticleSystem.EmissionModule[asteroidSystems.Length];
        asteroidBaseEmission = new float[asteroidSystems.Length];
        asteroidBaseSpeed = new float[asteroidSystems.Length];
        for (int i = 0; i < asteroidSystems.Length; i++)
        {
            // Adjust the emitter size
            ParticleSystem.ShapeModule shape = asteroidSystems[i].shape;
            shape.radius = cam.orthographicSize * 2.5f;

            asteroidNoise[i] = asteroidSystems[i].noise;
            asteroidEmission[i] = asteroidSystems[i].emission;
            asteroidBaseEmission[i] = asteroidEmission[i].rateOverTime.constant;
            asteroidBaseSpeed[i] = asteroidSystems[i].main.startSpeed.constant;

            asteroidEmission[i].enabled = false;

            if (asteroidSystems[i].main.maxParticles > maxCount)
                maxCount = asteroidSystems[i].main.maxParticles;
        }

        //  ----- Dust Systems -----
        dustEmission = new ParticleSystem.EmissionModule[dustSystems.Length];
        dustBaseEmission = new float[dustSystems.Length];
        dustBaseSpeed = new float[dustSystems.Length];
        for (int i = 0; i < dustSystems.Length; i++)
        {
            // Adjust the emitter size
            ParticleSystem.ShapeModule shape = dustSystems[i].shape;
            shape.radius = cam.orthographicSize * 2.5f;

            dustEmission[i] = dustSystems[i].emission;
            dustBaseEmission[i] = dustEmission[i].rateOverTime.constant;
            dustBaseSpeed[i] = dustSystems[i].main.startSpeed.constant;

            dustEmission[i].enabled = false;

            if (dustSystems[i].main.maxParticles > maxCount)
                maxCount = dustSystems[i].main.maxParticles;
        }
    }

    public void StartEffect()
    {
        routine = StartCoroutine(AsteroidEffect());
    }

    public void StopEffect()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;

            float time = 1;

            // Assign array using the max particle count
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[maxCount];

            //  ----- Asteroid Systems -----
            for (int i = 0; i < asteroidSystems.Length; i++)
            {
                // Disable emission
                asteroidEmission[i].enabled = false;

                // Set the lifetime of all particles
                int pCount = asteroidSystems[i].GetParticles(particles);
                for (int j = 0; j < pCount; j++)
                {
                    particles[j].remainingLifetime = particles[j].remainingLifetime / particles[j].startLifetime * time;
                    particles[j].startLifetime = time;
                }
                asteroidSystems[i].SetParticles(particles);
            }
            //  ----- Dust Systems -----
            for (int i = 0; i < dustSystems.Length; i++)
            {
                // Disable emission
                dustEmission[i].enabled = false;

                // Set the lifetime of all particles
                int pCount = dustSystems[i].GetParticles(particles);
                for (int j = 0; j < pCount; j++)
                {
                    particles[j].remainingLifetime = particles[j].remainingLifetime / particles[j].startLifetime * time;
                    particles[j].startLifetime = time;
                }
                dustSystems[i].SetParticles(particles);
            }
        }
    }

    private IEnumerator AsteroidEffect()
    {
        // Assign array using the max particle count
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[maxCount];

        while (true)
        {
            //  ----- Asteroid Systems -----
            for (int i = 0; i < asteroidSystems.Length; i++)
            {
                // Modify module values by speed
                asteroidNoise[i].scrollSpeed = speedModifier;
                asteroidNoise[i].positionAmount = speedModifier;
                asteroidEmission[i].rateOverTime = asteroidBaseEmission[i] * speedModifier;
                asteroidEmission[i].enabled = true;

                // Get particles in the system
                int pCount = asteroidSystems[i].GetParticles(particles);
                for (int j = 0; j < pCount; j++)
                {
                    particles[j].velocity = particles[j].velocity.normalized * asteroidBaseSpeed[i] * speedModifier;
                    // When a particle goes off screen, destroy it
                    if (particles[j].position.z < -30)
                    {
                        particles[j].remainingLifetime = -1;
                    }
                }
                asteroidSystems[i].SetParticles(particles, pCount);
            }

            //  ----- Dust Systems -----
            for (int i = 0; i < dustSystems.Length; i++)
            {
                // Modify module values by speed
                dustEmission[i].rateOverTime = dustBaseEmission[i] * speedModifier;
                dustEmission[i].enabled = true;

                // Get particles in the system
                int pCount = dustSystems[i].GetParticles(particles);
                for (int j = 0; j < pCount; j++)
                {
                    particles[j].velocity = particles[j].velocity.normalized * dustBaseSpeed[i] * Mathf.Max(speedModifier, minAsteroidSpeed);
                    // When a particle goes off screen, destroy it
                    if (particles[j].position.z < -30)
                    {
                        particles[j].remainingLifetime = -1;
                    }
                }
                dustSystems[i].SetParticles(particles, pCount);
            }

            yield return null;
        }
    }
}
