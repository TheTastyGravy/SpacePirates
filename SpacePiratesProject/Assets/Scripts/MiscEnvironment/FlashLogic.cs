using UnityEngine;
using UnityEngine.UI;
public class FlashLogic : MonoBehaviour
{
    public Image _Image;
    [Space]
    public float Speed;

    bool running, back;



    void Start()
    {
        running = true;
        back = false;
    }

    private void FixedUpdate()
    {
        if(running)
        {
            if (!back)
            {
                _Image.color += new Color(0f, 0f, 0f, Time.fixedDeltaTime * Speed);
                if (_Image.color.a >= 1f)
                    back = true;
            }
            else
            {
                _Image.color += new Color(0f, 0f, 0f, Time.fixedDeltaTime * -Speed);
                if (_Image.color.a <= 0f)
                    Destroy(gameObject);
            }
        }
    }
}