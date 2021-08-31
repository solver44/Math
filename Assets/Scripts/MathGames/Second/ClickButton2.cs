using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ClickButton2 : MonoBehaviour
{
    [HideInInspector] public bool StartWithoutPlayer = false;
    [HideInInspector] public GameObject[] Lvls = null;
    [HideInInspector] public GameObject[] Questions = null;
    [HideInInspector]public GameObject[] AnswerBtns = null;
    [HideInInspector]public Sprite[] Shapes = null;

    [HideInInspector] public bool Finish = false;

    IEnumerator MoveSmooth(ScrollRect transF, Vector2 target)
    {
        float scaleDuration = 5f;
        for (float t = 0; t < 1; t += Time.deltaTime / scaleDuration)
        {
            transF.normalizedPosition = Vector2.Lerp(transF.normalizedPosition, target, t);
            if (transF.normalizedPosition.Equals(target))
            {
                yield break;
            }
            yield return null;
        }
    }
    private IEnumerator LerpToChild(ScrollRect _scrollRectComponent, RectTransform target, bool isMain)
    {
        //StartCoroutine(LerpToChild(lineScrollRect, Lines[_currentIndex].GetComponent<RectTransform>(), false));
        RectTransform child;
        if (isMain)
            child = _scrollRectComponent.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        else
            child = _scrollRectComponent.transform.GetChild(1).GetComponent<RectTransform>();

        Vector2 _lerpTo = child.anchoredPosition - (Vector2)_scrollRectComponent.transform.InverseTransformPoint(target.position) - new Vector2(0, 690);
        bool _lerp = true;
        Canvas.ForceUpdateCanvases();

        float decelerate = 3f;
        for (float i = 0; i < 1; i += Time.deltaTime * decelerate)
        {
            child.anchoredPosition = Vector2.Lerp(child.anchoredPosition, _lerpTo, i);
            if (Vector2.Distance(child.anchoredPosition, _lerpTo) < 0.25f)
            {
                child.anchoredPosition = _lerpTo;
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    int makeRandomlyNumWithoutEquals(int targetNum, int min, int max)
    {
        while (true)
        {
            int num = Random.Range(min, max);
            if (targetNum != num)
                return num;
        }
    }

    ScaleEffect scl = new ScaleEffect();
    bool wait = false;

    [HideInInspector] public PhotonView PunView = null;

    private IEnumerator waitSeconds()
    {
        wait = true;
        yield return new WaitForSeconds(1f);
        wait = false;
    }
    public void CheckEqual(Image sprite)
    {
        if (Finish || wait)
            return;



        StartCoroutine(waitSeconds());
    }

    void SetPunView()
    {
        if (!StartWithoutPlayer)
            PunView = GameObject.Find("enemy").transform.GetComponent<PhotonView>();
    }
}
