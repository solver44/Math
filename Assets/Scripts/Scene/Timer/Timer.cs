using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public int Seconds = 45;
    public Text timeText = null;
    public Image top = null;

    public delegate void losingEvent();
    public static event losingEvent losingEventP;

    private static bool isOverTime = false;
    void Start()
    {
        currentSeconds = Seconds;
    }
    public void StartTick()
    {
        StartCoroutine(tick());
        top.fillAmount = 1;
        StartCoroutine(fillAM());
    }

    void Change() { }
    private IEnumerator fillAM()
    {
        yield return new WaitForSeconds(1);
        float duration = 1 / (float)Seconds;
        for (float t = 1; t >= 0; t -= duration)
        {
            top.fillAmount -= duration;
            if (top.fillAmount <= 0)
                yield break;
            yield return new WaitForSeconds(1);
        }
    }
    int currentSeconds = 0;
    private IEnumerator tick()
    {
        while (currentSeconds >= 0)
        {
            timeText.text = currentSeconds.ToString();
            currentSeconds--;
            yield return new WaitForSeconds(1);
        }

        isOverTime = true;
        losingEventP?.Invoke();
    }
}
