using UnityEngine;

public class Rotator : MonoBehaviour
{
    public GameObject _Object;
    public Vector3 _Dir;
    public float _Speed;

    Transform myTransform;
    private void OnEnable() => myTransform = _Object.transform;

    private void FixedUpdate() => myTransform.Rotate(_Dir * _Speed * Time.fixedDeltaTime);
}