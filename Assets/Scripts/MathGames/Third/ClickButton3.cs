using Photon.Pun;
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
        IDictionary<Color, int> colorVals = new Dictionary<Color, int>();
        IDictionary<Sprite, int> allValues = new Dictionary<Sprite, int>();
        int columnIndex = 0;
        for (int l = 0; l < Questions[currentIndex].transform.childCount; l++)
        {
            for (int i = 0; i < 3; i++)
            {
                temp = Questions[currentIndex].transform.GetChild(l).transform.GetChild(i).GetComponent<Image>();
                if (temp.overrideSprite != null)
                {
                    if (allValues.ContainsKey(temp.overrideSprite))
                    {
                        int tempCnt = allValues[temp.overrideSprite] + 1;
                        allValues.Remove(temp.overrideSprite);
                        allValues.Add(temp.overrideSprite, tempCnt);
                    }
                    else
                        allValues.Add(temp.overrideSprite, 1);

                    if (!colorVals.ContainsKey(temp.color)) {
                        colorVals.Add(temp.color, 1);
                    }
                    else
                    {
                        int tempCnt = colorVals[temp.color] + 1;
                        colorVals.Remove(temp.color);
                        colorVals.Add(temp.color, tempCnt);
                    }
                }
                else
                {
                    columnIndex = i;
                }
            }
        }

        int[] randAnsIndexes = getRandomNumber(0, 2, 2, false);

        Image child = AnswerBtns[randAnsIndexes[0]].transform.GetChild(0).GetComponent<Image>();
        Sprite tempSpr = allValues.Where(c => c.Value < 3).First().Key;
        child.overrideSprite = tempSpr;
        if (Questions[currentIndex].transform.childCount > 1)
        {
            child.color = colorVals.Where(c => c.Value < colorVals.Values.Max()).First().Key;
        }
        else
            child.color = Colors[makeRandomlyNumWithoutEquals(new int[] { Array.FindIndex(Colors, c => c == colorVals.Where(f => f.Value < 3).First().Key), Array.FindIndex(Colors, c => c == colorVals.Where(f => f.Value < 3).Last().Key) }, 0, Colors.Length)];
        //if (Questions[currentIndex].transform.childCount < 2)
        //    child.color = Colors[makeRandomlyNumWithoutEquals(new int[] { Array.FindIndex(Colors, c => c == colorVals[indexes[0]]), Array.FindIndex(Colors, c => c == colorVals[indexes[1]]) }, 0, Colors.Length)];
        //else
        //    child.color = Questions[currentIndex].transform.GetChild(Mathf.Abs(columnIndex - 1)).transform.GetChild(3 - (indexes[0] + indexes[1])).GetComponent<Image>().color;

        child.SetNativeSize();
        int randShapeIndex = makeRandomlyNumWithoutEquals(new int[] { Array.FindIndex(Shapes, c => c == tempSpr) }, 0, Shapes.Length);
        child = AnswerBtns[randAnsIndexes[1]].transform.GetChild(0).GetComponent<Image>();
        child.overrideSprite = Shapes[randShapeIndex];
        child.color = Colors[UnityEngine.Random.Range(0, Colors.Length)];
        //if (Questions[currentIndex].transform.childCount < 3)
        //    child.overrideSprite = Shapes[makeRandomlyNumWithoutEquals(new int[] { Array.FindIndex(Shapes, c => c == values[indexes[0]]), Array.FindIndex(Shapes, c => c == values[indexes[1]]) }, 0, Shapes.Length)];
        //else
        //    child.overrideSprite = allValues.Where(c => c.Value == 20).First().Key;

        //child.color = Colors[UnityEngine.Random.Range(0, Colors.Length)];
        child.SetNativeSize();

        Lvls[currentIndex].GetComponent<Image>().color = new Color32(255, 212, 47, 255);
    }
    public void CheckEqual(bool pressedEqual)
    {
        if (Finish || wait)
            return;

        bool isEqual = false;

        Image temp;

        IDictionary<Image, int> firstVals = new Dictionary<Image, int>(); 
        IDictionary<Image, int> secondVals = new Dictionary<Image, int>();
        for (int i = 0; i < Questions[currentIndex].transform.childCount; i++)
        {
            for (int l = 0; l < Questions[currentIndex].transform.GetChild(i).transform.childCount; l++)
            {
                temp = Questions[currentIndex].transform.GetChild(i).transform.GetChild(l).GetComponent<Image>();
                if (i == 0)
                {
                    if(!firstVals.ContainsKey(temp))
                        firstVals.Add(temp, 1);
                    else
                    {
                        int cnt = firstVals[temp] + 1;
                        firstVals.Remove(temp);
                        firstVals.Add(temp, cnt);
                    }
                }
                else
                {
                    if (!secondVals.ContainsKey(temp))
                        secondVals.Add(temp, 1);
                    else
                    {
                        int cnt = secondVals[temp] + 1;
                        secondVals.Remove(temp);
                        secondVals.Add(temp, cnt);
                    }
                }
            }
        }

        

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
