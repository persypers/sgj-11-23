using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnvironmentController : MonoBehaviour
{
    [SerializeField] Gradient colorSky;
    [SerializeField] Gradient colorFilter;
    [SerializeField] Material Skybox;

    [SerializeField] Volume volume;
    [SerializeField] float minBrightness;
    [SerializeField] float maxBrightness;
    private ColorAdjustments brightness;
    private ColorAdjustments hue;

    [Range(0.0f, 1.0f)]
    public float timeOfDay;

    public float timeOfDaySpeed;

    //[SerializeField] bool timeOfDayAuto;

    private float _timer;

    // Start is called before the first frame update
    void Start()
    {
        _timer = 0.3f;

        Skybox = RenderSettings.skybox;
        VolumeProfile proflile = volume.sharedProfile;
        volume.profile.TryGet(out brightness);
        volume.profile.TryGet(out hue);
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime * timeOfDaySpeed;

        timeOfDay = _timer;

        Skybox.SetColor("_Tint", colorSky.Evaluate(timeOfDay));

        if (timeOfDay > 0.5f)
        {
            brightness.postExposure.value = Mathf.Lerp(minBrightness, maxBrightness, (timeOfDay - 1f) * (-1f));
            hue.colorFilter.value = colorFilter.Evaluate((timeOfDay - 1f) * (-1f));
        }
        else
        {
            brightness.postExposure.value = Mathf.Lerp(minBrightness, maxBrightness, timeOfDay);
            hue.colorFilter.value = colorFilter.Evaluate(timeOfDay);
        }


        if (timeOfDay > 1f)
        {
            timeOfDay = 0;
            _timer = 0;
        }

    }
}
