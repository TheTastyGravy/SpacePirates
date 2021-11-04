using UnityEngine;

public class TurretRecoil : MonoBehaviour
{
    public GameObject Barrel;
    public Vector3 RecoilAmount;
    public float Speed;

    Vector3 startPos, endPos, targetPos;
    bool running;

    private void OnEnable()
    {
        startPos = Barrel.transform.localPosition;
        endPos = startPos + RecoilAmount;
        running = false;
    }

    private void FixedUpdate()
    {
        if (running)
        {
            if (Barrel.transform.localPosition != targetPos)
                Barrel.transform.localPosition = Vector3.MoveTowards(Barrel.transform.localPosition, targetPos, Speed * Time.fixedDeltaTime);
            else
                Stop();
        }
    }

    public void Run()
    {
        running = true;
        Barrel.transform.localPosition = startPos;
        targetPos = endPos;
    }

    void Stop()
    {
        if (Barrel.transform.localPosition == startPos)
            running = false;
        else
            targetPos = startPos;
    }
}