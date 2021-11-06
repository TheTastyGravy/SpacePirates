using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantOffset : MonoBehaviour
{
    private Vector3 offset;

    void Awake()
    {
        if (transform.parent == null)
            enabled = false;
        offset = transform.localPosition;
    }

    void LateUpdate()
    {
        transform.position = transform.parent.position + offset;
    }
}
