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
            if (transform.position != targetPos)
                transform.position = Vector3.MoveTowards(transform.position, targetPos, Speed * Time.fixedDeltaTime);
            else
                Stop();
        }
    }

    public void Run()
    {
        running = true;
        Barrel.transform.position = startPos;
        targetPos = endPos;
    }

    void Stop()
    {
        if (transform.position == startPos)
            running = false;
        else
            targetPos = startPos;
    }
}