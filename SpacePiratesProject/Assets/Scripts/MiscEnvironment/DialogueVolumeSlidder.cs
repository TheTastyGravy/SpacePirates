using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class DialogueVolumeSlidder : MonoBehaviour
{
    public Slider slider;
    public EventReference sound;
    public float timeDelay = 0.25f;

    private FMOD.Studio.EventInstance musicInstance;
    private float timePassed = 0;



    void Awake()
    {
        slider.onValueChanged.AddListener(delegate { OnValueChanged(); });

        musicInstance = RuntimeManager.CreateInstance(sound);
        musicInstance.set3DAttributes(RuntimeUtils.To3DAttributes(Vector3.zero));
    }

    private void OnValueChanged()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        timePassed = 0;
    }

    private void Update()
    {
        if (timePassed < timeDelay)
        {
            timePassed += Time.deltaTime;

            if (timePassed >= timeDelay)
            {
                musicInstance.start();
            }
        }
    }
}
