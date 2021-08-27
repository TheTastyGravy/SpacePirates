using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    public Transform[] targetObjects;
    public Vector3 targetOffset;
    public LayerMask wallMask;
    public float sphereCastRadius = 1;

    public float cutoutSize = 0.1f;


    private Camera mainCamera;
    private List<Material> lastMats = new List<Material>();



    void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        // Reset the materials from the last frame
        foreach (var mat in lastMats)
        {
            mat.SetVector("_CutoutPos_1", -Vector2.one);
            mat.SetVector("_CutoutPos_2", -Vector2.one);
            mat.SetVector("_CutoutPos_3", -Vector2.one);
            mat.SetVector("_CutoutPos_4", -Vector2.one);
            mat.SetFloat("_CutoutSize", 0);
        }
        lastMats.Clear();


        for (int i = 0; i < targetObjects.Length; i++)
        {
            Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObjects[i].position + targetOffset);
            cutoutPos.y /= (Screen.width / Screen.height);

            Vector3 direction = targetObjects[i].position - transform.position;
            float targetDist = Vector3.SqrMagnitude(direction);

            RaycastHit[] hitObjects = Physics.SphereCastAll(transform.position, sphereCastRadius, direction, Mathf.Sqrt(targetDist), wallMask);
            for (int j = 0; j < hitObjects.Length; j++)
            {
                // If the object is further away than the target, ignore it
                float objDist = Vector3.SqrMagnitude(hitObjects[j].transform.position - transform.position);
                if (objDist > targetDist)
                {
                    continue;
                }

                // Set values for each material, and add them to a list to be reset next frame
                Material[] materials = hitObjects[j].transform.GetComponent<Renderer>().materials;
                for (int k = 0; k < materials.Length; k++)
                {
                    materials[k].SetVector("_CutoutPos_" + (i + 1), cutoutPos);
                    materials[k].SetFloat("_CutoutSize", cutoutSize);
                    lastMats.Add(materials[k]);
                }
            }
        }
    }
}
