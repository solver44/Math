﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ClickButton2 : MonoBehaviour
{
    [HideInInspector] public bool StartWithoutPlayer = true;
    [HideInInspector] public GameObject[] Lvls = null;
    [HideInInspector] public GameObject[] Questions = null;
    [HideInInspector] public GameObject[] AnswerBtns = null;
    [HideInInspector] public Sprite[] Shapes = null;
    [HideInInspector] public Color[] Colors;
    [HideInInspector] public Text StatsHealth;
    [HideInInspector] public int Health;

    [HideInInspector] public bool Finish = false;


    private int health { get { return Health; } set
        {
            Health = value;
            changeStats();
            if (value < 1)
            {
                Finish = true;
                lose();
            }
        } }

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
        StatsHealth.text = health.ToString();
        StatsHealth.GetComponent<Animator>().SetTrigger("start");
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
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(effect.MoveAnimTowards(Questions[currentIndex].transform, new Vector2(-37, Questions[currentIndex].transform.localPosition.y), true, 12f));
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
                    CreatorMapSecond map = GameObject.Find("Creator").GetComponent<CreatorMapSecond>();
                    map.WinOrLose(true);
                }
            }
            else
            {
                CreatorMapSecond map = GameObject.Find("Creator").GetComponent<CreatorMapSecond>();
                map.WinOrLose(true);
            }
            return true;
        }
        return false;
    }
    public void CreateNewAns()
    {

        StartCoroutine(IeStart());

        Image temp;
        IDictionary<int, Sprite> values = new Dictionary<int, Sprite>();
        IDictionary<int, Color> colorVals = new Dictionary<int, Color>();
        int[] indexes = new int[3];
        int columnIndex = 0;
        bool stop = false;
        for (int l = 0; l < Questions[currentIndex].transform.childCount; l++)
        {
            values = new Dictionary<int, Sprite>();
            colorVals = new Dictionary<int, Color>();
            for (int i = 0, cnt = 0; i < 3; i++)
            {
                temp = Questions[currentIndex].transform.GetChild(l).transform.GetChild(i).GetComponent<Image>();
                if (temp.overrideSprite != null)
                {
                    indexes[cnt] = i;
                    cnt++;
                    values.Add(i, temp.overrideSprite);
                    colorVals.Add(i, temp.color);
                }
                else
                {
                    columnIndex = l;
                    stop = true;
                }
            }
            if (stop)
                break;
        }

        int randIndex = values.Keys.ElementAt(UnityEngine.Random.Range(0, values.Count()));
        int[] randAnsIndexes = getRandomNumber(0, 2, 2, false);


        Image child = AnswerBtns[randAnsIndexes[0]].transform.GetChild(0).GetComponent<Image>();
        child.overrideSprite = values[randIndex];
        if (Questions[currentIndex].transform.childCount < 2)
            child.color = Colors[makeRandomlyNumWithoutEquals(new int[] { Array.FindIndex(Colors, c => c == colorVals[indexes[0]]), Array.FindIndex(Colors, c => c == colorVals[indexes[1]]) }, 0, Colors.Length)];
        else
            child.color = Questions[currentIndex].transform.GetChild(Mathf.Abs(columnIndex - 1)).transform.GetChild(3 - (indexes[0] + indexes[1])).GetComponent<Image>().color;

        child.SetNativeSize();

        child = AnswerBtns[randAnsIndexes[1]].transform.GetChild(0).GetComponent<Image>();
        child.overrideSprite = Shapes[makeRandomlyNumWithoutEquals(new int[] { Array.FindIndex(Shapes, c => c == values[indexes[0]]), Array.FindIndex(Shapes, c => c == values[indexes[1]]) }, 0, Shapes.Length)];
        child.color = Colors[UnityEngine.Random.Range(0, Colors.Length)];
        child.SetNativeSize();

        Lvls[currentIndex].GetComponent<Image>().color = new Color32(255, 212, 47, 255);
    }
    public void CheckEqual(Image sprite)
    {
        if (Finish || wait)
            return;

        bool equalThreeObjects = false;
        Image temp;
        IDictionary<int, Sprite> values = new Dictionary<int, Sprite>();

        int[] indexes = new int[2];
        for (int l = 0; l < Questions[currentIndex].transform.childCount; l++)
        {
            if (Questions[currentIndex].transform.GetChild(l).transform.GetComponentsInChildren<Image>().Where(c => c.overrideSprite == null).Count() > 0)
            {
                for (int i = 0, cnt = 0; i < 3; i++)
                {
                    temp = Questions[currentIndex].transform.GetChild(l).transform.GetChild(i).GetComponent<Image>();
                    if (temp.overrideSprite != null)
                    {
                        indexes[cnt] = i;
                        cnt++;
                        values.Add(i, temp.overrideSprite);
                    }
                }
            }
        }

        if (values[indexes[0]] == values[indexes[1]])
            equalThreeObjects = true;

        if ((values[indexes[0]].name == sprite.overrideSprite.name && equalThreeObjects) || (values.Where(c => c.Value.name == sprite.overrideSprite.name).Count() < 1 && !equalThreeObjects))
            results[currentIndex] = 1;
        else
            results[currentIndex] = 0;

        setInactivePrevious(results[currentIndex] == 1);

        currentIndex++;

        if (won())
            return;

        CreateNewAns();

        StartCoroutine(waitSeconds());
    }
    private void setInactivePrevious(bool isAnsTrue)
    {
        StartCoroutine(effect.MoveAnimTowards(Questions[currentIndex].transform, new Vector2(1250, Questions[currentIndex].transform.localPosition.y), true, 12f));

        if (isAnsTrue)
        {
            Lvls[currentIndex].GetComponent<Image>().color = new Color32(15, 225, 10, 255);
        }
        else
        {
            Lvls[currentIndex].GetComponent<Image>().color = new Color32(225, 15, 10, 255);
            health--;
        }

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
