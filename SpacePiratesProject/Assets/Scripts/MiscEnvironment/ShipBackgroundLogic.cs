using UnityEngine;

public class ShipBackgroundLogic : MonoBehaviour
{
    public GameObject[] Visuals;
    [Space]
    public Vector3 StartPos;
    public float DisableDelay;
    public float Speed;

    float elapsed;
    private void OnEnable()
    {
        RandomVisual();
        transform.position = StartPos;
        elapsed = 0f;
    }

    void RandomVisual()
    {
        int x = Random.Range(0, Visuals.Length);

        for (int i = 0; i < Visuals.Length; i++)
        {
            if (i == x)
                Visuals[i].SetActive(true);
            else
                Visuals[i].SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Speed * Time.fixedDeltaTime);

        elapsed += Time.fixedDeltaTime;
        if (elapsed >= DisableDelay)
            gameObject.SetActive(false);
    }
}
