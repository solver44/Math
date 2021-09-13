﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ClickButton3 : MonoBehaviour
{
    [HideInInspector] public bool StartWithoutPlayer = true;
    [HideInInspector] public GameObject[] Lvls = null;
    [HideInInspector] public GameObject[] Questions = null;
    [HideInInspector] public GameObject[] AnswerBtns = null;
    [HideInInspector] public Sprite[] Shapes = null;
    [HideInInspector] public Color[] Colors;
    [HideInInspector] public Text Stats;

    [HideInInspector] public bool Finish = false;

    private void lose()
    {
        Debug.Log("Finish");
        if (!StartWithoutPlayer)
        {
            try
            {
                PunView.RPC("CheckWinOrLose", RpcTarget.All, results);
            }
            catch {
                CreatorMapSecond map = GameObject.Find("Creator").GetComponent<CreatorMapSecond>();
                map.WinOrLose(false);
            }
        }
        else
        {
            CreatorMapSecond map = GameObject.Find("Creator").GetComponent<CreatorMapSecond>();
            map.WinOrLose(false);
        }
    }
    private void changeStats()
    {

    }
    int[] results;
    ScaleEffect effect = new ScaleEffect();
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

    int makeRandomlyNumWithoutEquals(int[] targetNum, int min, int max)
    {
        int num;
        do
        {
            num = UnityEngine.Random.Range(min, max);
        }
        while (targetNum.Contains(num));

        return num;
    }

    ScaleEffect scl = new ScaleEffect();
    bool wait = false;

    [HideInInspector] public PhotonView PunView = null;

    private IEnumerator waitSeconds()
    {
        wait = true;
        yield return new WaitForSeconds(.8f);
        wait = false;
    }
    int currentIndex = 0;
    private IEnumerator IeStart()
    {
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(effect.MoveAnimTowards(Questions[currentIndex].transform, new Vector2(Questions[currentIndex].transform.localPosition.x, 0), true, 8f));
    }
    bool won()
    {
        if (currentIndex >= Lvls.Length)
        {
            Finish = true;
            Debug.Log("Finish");
            if (!StartWithoutPlayer) {
                try
                {
                    PunView.RPC("CheckWinOrLose", RpcTarget.All, results);
                }
                catch {
                    CreatorMapThird map = GameObject.Find("Creator").GetComponent<CreatorMapThird>();
                    map.WinOrLose(true);
                }
            }
            else
            {
                CreatorMapThird map = GameObject.Find("Creator").GetComponent<CreatorMapThird>();
                map.WinOrLose(true);
            }
            return true;
        }
        return false;
    }
    public void CreateNewAns()
    {
        StartCoroutine(IeStart());

        Lvls[currentIndex].transform.GetChild(0).gameObject.SetActive(true);
        RectTransform num = Lvls[currentIndex].transform.GetChild(1).GetComponent<RectTransform>();
        num.offsetMin = new Vector2(60, num.offsetMin.y);
    }
    public void CheckEqual(bool pressedEqual)
    {
        if (Finish || wait)
            return;

        bool isEqual = false;

        List<Sprite> firstVals = new List<Sprite>(); 
        List<Sprite> secondVals = new List<Sprite>();

        for (int i = 0; i < Questions[currentIndex].transform.childCount; i++)
        {
            for (int l = 0; l < Questions[currentIndex].transform.GetChild(i).transform.childCount; l++)
            {
                foreach (var temp in Questions[currentIndex].transform.GetChild(i).transform.GetChild(l).GetComponentsInChildren<Image>())
                {
                    if (i == 0)
                    {
                        firstVals.Add(temp.overrideSprite);
                    }
                    else
                    {
                        secondVals.Add(temp.overrideSprite);
                    }
                }
            }
        }
        if (firstVals.All(secondVals.Contains))
        {
            isEqual = true;
        }
        setInactivePrevious(isEqual == pressedEqual);

        currentIndex++;

        if (currentIndex >= Questions.Length)
        {
            won();
            return;
        }

        CreateNewAns();

        StartCoroutine(waitSeconds());
    }
    private void setInactivePrevious(bool isAnsTrue)
    {
        StartCoroutine(effect.MoveAnimTowards(Questions[currentIndex].transform, new Vector2(Questions[currentIndex].transform.localPosition.y, -500), true, 8f));

        if (isAnsTrue)
        {
            Lvls[currentIndex].GetComponent<Image>().color = new Color32(15, 225, 10, 200);
        }
        else
        {
            Lvls[currentIndex].GetComponent<Image>().color = new Color32(225, 15, 10, 200);
        }
        Lvls[currentIndex].transform.GetChild(0).gameObject.SetActive(false);
        Lvls[currentIndex].transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2(0, Lvls[currentIndex].transform.GetChild(1).GetComponent<RectTransform>().offsetMin.y);
    }
    private int[] getRandomNumber(int min, int max, int count, bool equalNums)
    {
        int rand = UnityEngine.Random.Range(min, max);
        int[] rands = new int[count];
        for (int i = 0; i < rands.Length; i++)
        {
            rands[i] = -1;
        }
        for (int i = 0; i < rands.Length; i++)
        {
            while (!equalNums && rands.Contains(rand))
            {
                rand = UnityEngine.Random.Range(min, max);
            }
            if (equalNums)
                rand = UnityEngine.Random.Range(min, max);

            rands[i] = rand;
        }

        return rands;
    }
    void SetPunView()
    {
        if (!StartWithoutPlayer)
            PunView = GameObject.Find("enemy").transform.GetComponent<PhotonView>();
    }
    private void Start()
    {
        results = new int[Lvls.Length];
    }
}
