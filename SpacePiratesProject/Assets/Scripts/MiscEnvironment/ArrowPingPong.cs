using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPingPong : MonoBehaviour
{
    public GameObject Arrow;
    public float Distance;
    public float Speed;
    [Space]
    public bool Left;

    private Vector3 startPos;
    private RectTransform rectTransform;
    private Coroutine routine;



    void OnEnable()
    {
        if (startPos == Vector3.zero)
        {
            rectTransform = Arrow.GetComponent<RectTransform>();
            startPos = rectTransform.localPosition;
        }

        if (routine != null)
            StopCoroutine(routine);
        routine = StartCoroutine(PingPong());
    }

    void OnDisable()
    {
        if (routine != null)
            StopCoroutine(routine);
    }

    private IEnumerator PingPong()
    {
        while (true)
        {
            if (Left)
                rectTransform.localPosition = startPos - new Vector3(Mathf.Sin(Time.unscaledTime * Speed) * Distance, 0, 0);
            else
                rectTransform.localPosition = startPos + new Vector3(Mathf.Sin(Time.unscaledTime * Speed) * Distance, 0, 0);
            yield return null;
        }
    }
}