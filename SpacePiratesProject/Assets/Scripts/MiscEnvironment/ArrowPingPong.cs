using UnityEngine;
public class ArrowPingPong : MonoBehaviour
{
    public GameObject Arrow;
    public float Distance;
    public float Speed;
    [Space]
    public bool Left;

    Vector3 startPos;
    RectTransform rectTransform;

    private void OnEnable()
    {
        if (startPos == Vector3.zero)
        {
            rectTransform = Arrow.GetComponent<RectTransform>();
            startPos = rectTransform.localPosition;
        }
        else
            rectTransform.localPosition = startPos;
    }

    private void FixedUpdate()
    {
        if (Left)
            rectTransform.localPosition += new Vector3(-(Mathf.Sin(Time.time * Speed) * Distance), 0f, 0f);
        else
            rectTransform.localPosition += new Vector3((Mathf.Sin(Time.time * Speed) * Distance), 0f, 0f);
    }
}