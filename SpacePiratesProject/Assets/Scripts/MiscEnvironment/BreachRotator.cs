using UnityEngine;

public class BreachRotator : MonoBehaviour
{
    public GameObject _ob;
    [Space]
    public Vector3 _RotationAmount;

    const float MIN = 0f;

    private void OnEnable() => ApplyRotation(_RotationAmount);

    void ApplyRotation(Vector3 v3) => _ob.transform.localEulerAngles = new Vector3(GetRandomValue(v3.x), GetRandomValue(v3.y), GetRandomValue(v3.z));

    float GetRandomValue(float max)
    {
        return Random.Range(MIN, max);
    }
}