using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ClickButton4 : MonoBehaviour
{
    [HideInInspector] public bool StartWithoutPlayer = true;
    [HideInInspector] public GameObject[] Lvls = null;
    [HideInInspector] public GameObject[] Questions = null;
    [HideInInspector] public GameObject[] AnswerBtns = null;
    [HideInInspector] public Sprite[] Elements = null;
    [HideInInspector] public int[] Answers;

    [HideInInspector] public bool Finish = false;

    private Animator symbolAnimController; // => GameObject.FindGameObjectWithTag("Anim").GetComponent<Animator>();

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
                CreatorMapFourth map = GameObject.Find("Creator").GetComponent<CreatorMapFourth>();
                map.WinOrLose(false);
            }
        }
        else
        {
            CreatorMapFourth map = GameObject.Find("Creator").GetComponent<CreatorMapFourth>();
            map.WinOrLose(false);
        }
    }
    private void changeStats()
    {

    }
    int[] results;
    ScaleEffect effect = new ScaleEffect();
    void increaseSizeUI()
    {
        //StartCoroutine(MoveSmooth(rect, new Vector2(rect.localPosition.x, rect.localPosition.y + interval)));
        ScrollRect lineScrollRect = LBScrollContent.GetComponent<ScrollRect>();
        //StartCoroutine(MoveSmooth(scrollRect, new Vector2(scrollRect.preferredHeight)));
        StartCoroutine(LerpToChild(lineScrollRect, Lvls[currentIndex].GetComponent<RectTransform>()));
    }
    private IEnumerator LerpToChild(ScrollRect _scrollRectComponent, RectTransform target)
    {
        //StartCoroutine(LerpToChild(lineScrollRect, Lines[_currentIndex].GetComponent<RectTransform>(), false));
        RectTransform child;

        child = _scrollRectComponent.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();

        Vector2 _lerpTo = child.anchoredPosition - (Vector2)_scrollRectComponent.transform.InverseTransformPoint(target.position) - new Vector2(19, 60);

        Canvas.ForceUpdateCanvases();

        float decelerate = 1f;
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


    GameObject LBScrollContent;
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
        StartCoroutine(effect.MoveAnimTowards(Questions[currentIndex].transform, new Vector2(0, 0), true, 8f));
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
                    CreatorMapFourth map = GameObject.Find("Creator").GetComponent<CreatorMapFourth>();
                    map.WinOrLose(true);
                }
            }
            else
            {
                CreatorMapFourth map = GameObject.Find("Creator").GetComponent<CreatorMapFourth>();
                map.WinOrLose(true);
            }
            return true;
        }
        return false;
    }
    public void CreateNewAns()
    {
        int randAnsBtn = UnityEngine.Random.Range(0, AnswerBtns.Length);
        int randAnswer = -1;
        for (int o = 0; o < AnswerBtns.Length; o++)
        {
            if (o == randAnsBtn)
                AnswerBtns[o].GetComponentInChildren<Text>().text = (Answers[currentIndex]).ToString();
            else
            {
                randAnswer = makeRandomlyNumWithoutEquals(new int[] { Answers[currentIndex], randAnswer }, 1, 5);
                AnswerBtns[o].GetComponentInChildren<Text>().text = randAnswer.ToString();
            }
        }

        StartCoroutine(IeStart());

        increaseSizeUI();

        Lvls[currentIndex].transform.GetChild(0).gameObject.SetActive(true);
        Lvls[currentIndex].transform.GetChild(0).GetComponent<Image>().overrideSprite = icons[2];
    }
    public void CheckEqual(Text value)
    {
        if (Finish || wait)
            return;

        bool isTrue = false;

        if (Answers[currentIndex] == Int32.Parse(value.text))
            isTrue = true;

        StartCoroutine(waitAnim(isTrue));
    }

    private IEnumerator waitAnim(bool isTrue)
    {
        yield return new WaitForSeconds(0f);

        setInactivePrevious(isTrue);
        currentIndex++;

        if (currentIndex >= Questions.Length)
        {
            won();
            yield break;
        }

        CreateNewAns();

        StartCoroutine(waitSeconds());
    }
    public Sprite[] icons;
    private void setInactivePrevious(bool isAnsTrue)
    {
        StartCoroutine(effect.MoveAnimTowards(Questions[currentIndex].transform, new Vector2(0, -800), true, 8f));

        if (isAnsTrue)
        {
            Lvls[currentIndex].transform.GetChild(0).GetComponent<Image>().overrideSprite = icons[0];
        }
        else
        {
            Lvls[currentIndex].transform.GetChild(0).GetComponent<Image>().GetComponent<Image>().overrideSprite = icons[1];
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

        LBScrollContent = GameObject.FindGameObjectWithTag("Levels");

        //symbolAnimController.SetLayerWeight(0, 0f);
    }
}
