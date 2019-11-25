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
        float duration = 4;
        while (elapsed < duration)
        {
            Stars.lengthScale = Mathf.Lerp(Stars.lengthScale, -500, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1);
        float stopelapsed= 0;
        float stopduration = 3;
        while (stopelapsed < stopduration)
        {
            Stars.lengthScale = Mathf.Lerp(Stars.lengthScale, -3, stopelapsed / stopduration);
            stopelapsed += Time.deltaTime;
            yield return null;
        }
        Camera.main.cullingMask = -1;
        yield return new WaitForSeconds(1);
        Stars.lengthScale = -3f;
        LighSpeedActive = false;
        StartText.text = "Start";
        yield return new WaitForSeconds(1);
        StartText.text = "";


    }
}
