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

    private FMOD.Studio.EventInstance soundInstance;
    private float timePassed = 999;



    void Awake()
    {
        slider.onValueChanged.AddListener(delegate { OnValueChanged(); });

        soundInstance = RuntimeManager.CreateInstance(sound);
        soundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(Vector3.zero));
    }

    void OnDisable()
    {
        soundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        timePassed = 999;
    }

    private void OnValueChanged()
    {
        soundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        timePassed = 0;
    }

    private void Update()
    {
        if (timePassed < timeDelay)
        {
            timePassed += Time.deltaTime;

            if (timePassed >= timeDelay)
            {
                soundInstance.start();
            }
        }
    }
}
