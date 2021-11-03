using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public MeshRenderer[] backgroundRenderers;
    [HideInInspector]
    public float speedMultiplier = 1;
    private float customTime = 0;
    
    void LateUpdate()
    {
        customTime += Time.deltaTime * speedMultiplier;
        foreach (var obj in backgroundRenderers)
		{
            obj.material.SetFloat("_ScaledTime", customTime);
        }
    }
}
