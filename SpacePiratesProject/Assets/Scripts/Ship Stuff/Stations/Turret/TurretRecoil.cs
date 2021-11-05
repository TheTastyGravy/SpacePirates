using UnityEngine;

public class TurretRecoil : MonoBehaviour
{
    public Transform barrel;
    public Vector3 RecoilAmount;
    public float Speed;
    public float notInUseAngle = -40;
    public float angleTimeDown = 1;
    public float angleTimeUp = 0.3f;

    private Vector3 startPos, endPos, targetPos;
    private float currentBarrelAngle = 0;
    private bool shouldDoRecoil = false;
    private bool shouldRotate;
    private bool isRotatingDown;
    private float rotateTimePassed = 0;



    private void OnEnable()
    {
        startPos = barrel.localPosition;
        endPos = RecoilAmount;
        shouldRotate = true;
        isRotatingDown = true;
    }

    private void FixedUpdate()
    {
        if (shouldDoRecoil)
        {
            if (barrel.localPosition != targetPos)
                barrel.localPosition = Vector3.MoveTowards(barrel.localPosition, targetPos, Speed * Time.fixedDeltaTime);
            else
                Stop();
        }

        if (shouldRotate)
        {
            rotateTimePassed += Time.deltaTime;

            float lastAngle = currentBarrelAngle;
            currentBarrelAngle = Mathf.Lerp(0, notInUseAngle, (isRotatingDown ? rotateTimePassed / angleTimeDown : 1 - rotateTimePassed / angleTimeUp));
            barrel.RotateAround(barrel.parent.position, barrel.right, currentBarrelAngle - lastAngle);

            if ((isRotatingDown && currentBarrelAngle == notInUseAngle) ||
                (!isRotatingDown && currentBarrelAngle == 0))
            {
                shouldRotate = false;
            }
        }
    }

    public void Run()
    {
        shouldDoRecoil = true;
        barrel.localPosition = startPos;
        targetPos = endPos;
    }

    void Stop()
    {
        if (barrel.localPosition == startPos)
            shouldDoRecoil = false;
        else
            targetPos = startPos;
    }

    public void DoRotation(bool rotateDown)
    {
        shouldRotate = true;
        isRotatingDown = rotateDown;
        rotateTimePassed = 0;
    }
}