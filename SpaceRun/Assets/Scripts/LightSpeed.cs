using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightSpeed : MonoBehaviour
{
    public ParticleSystemRenderer Stars;
    public GameObject Ship;
    public Text StartText;
    public bool LighSpeedActive;
    public int duration;
    public int StopDuration;
    public int WaitBeforeStart;

    public void Start()
    {
        StartText.text = "";
        LighSpeedActive = true;
        Camera.main.cullingMask = ~(1 << LayerMask.NameToLayer("Tube"));

        Stars.lengthScale = 3;
        StartCoroutine(LightSpeedSimilate());

    }

    IEnumerator LightSpeedSimilate()
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            Stars.lengthScale = Mathf.Lerp(Stars.lengthScale, -500, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1);
        float stopelapsed= 0;
        while (stopelapsed < StopDuration)
        {
            Stars.lengthScale = Mathf.Lerp(Stars.lengthScale, -3, stopelapsed / StopDuration);
            stopelapsed += Time.deltaTime;
            yield return null;
        }
        Camera.main.cullingMask = -1;
        yield return new WaitForSeconds(WaitBeforeStart);
        Stars.lengthScale = -3f;
        LighSpeedActive = false;
        StartText.text = "Start";
        yield return new WaitForSeconds(1);
        StartText.text = "";


    }
}
