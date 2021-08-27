using UnityEngine;

public class WallRandomiser : MonoBehaviour
{
    public GameObject[] Visuals;
    private void OnEnable()
    {
        DisableVisuals();
        EnableVisual();
    }

    void DisableVisuals()
    {
        foreach (GameObject ob in Visuals)
        {
            if (ob.activeInHierarchy)
                ob.SetActive(false);
        }
    }

    void EnableVisual() => Visuals[Random.Range(0, Visuals.Length)].SetActive(true);
}
