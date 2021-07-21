﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ClickButton : MonoBehaviour
{
    [HideInInspector] public GameObject[] QBoxes;
    [HideInInspector] public GameObject[] Lines;
    [HideInInspector] public Sprite[] Images;
    [HideInInspector] public Text[] AnswerTexts;
    [HideInInspector] public Text CoinText;
    [HideInInspector] public int CntCoin;
    [HideInInspector] public float ScaleR = 0.2f;

    [HideInInspector] public PhotonView PunView;

    [HideInInspector] public GameObject MainScroll, MainScrollContent, LeftBarScrollContent;


    private int _currentIndex = 0;
    public int CurrentIndex {
        get { return _currentIndex; }
        set { _currentIndex = value; changedIndex(); }
    }
    void Start()
    {
        Timer.losingEventP += Timer_losingEventP;
    }

    private void Timer_losingEventP()
    {
        Debug.Log("lose");
        PunView.RPC("CheckWinOrLose", RpcTarget.All, results);
    }

    public int[] results;
    static int coins = 0;
    int calculateNums(string text)
    {
        string[] numbers = text.Trim(' ').Split('+');
        int res = 0;
        for (int i = 0; i < numbers.Length; i++)
        {
            res += int.Parse(numbers[i]);
        }

        return res;
    }

    public bool Finish = false;
    bool checkWin()
    {
        if(CurrentIndex >= QBoxes.Length)
        {
            Finish = true;
            Debug.Log("Finish");
            PunView.RPC("CheckWinOrLose", RpcTarget.All, results);
            return true;
        }
        return false;
    }
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
    void increaseSizeUI(GameObject obj, float interval)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        //StartCoroutine(MoveSmooth(rect, new Vector2(rect.localPosition.x, rect.localPosition.y + interval)));
        ScrollRect scrollRect = MainScroll.GetComponent<ScrollRect>();
        StartCoroutine(MoveSmooth(scrollRect, new Vector2(0, 1)));
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
    void changedIndex()
    {
        if (checkWin())
            return;

        //if(CurrentIndex == 12)
           // increaseSizeUI(MainScrollContent, -160);

        int randS = Random.Range(0, 2);
        Values vals = QBoxes[CurrentIndex].GetComponent<Values>();
        vals.UserImage.SetActive(false);
        vals.Effect.SetActive(true);
        int result = calculateNums(vals.Text.text);
        AnswerTexts[randS].text = result.ToString();
        if (randS == 1)
        {
            int num = Random.Range(0, 11);
            AnswerTexts[0].text = (num == result ? makeRandomlyNumWithoutEquals(num, 0, 11) : num).ToString();
        }
        else
        {
            int num = Random.Range(0, 11);
            AnswerTexts[1].text = (num == result ? makeRandomlyNumWithoutEquals(num, 0, 11) : num).ToString();
        }

        Transform obj = QBoxes[CurrentIndex].transform.GetChild(0).transform.GetChild(0).transform;
        Vector2 toThisScale = new Vector2(obj.localScale.x + (obj.localScale.x * ScaleR), obj.localScale.y + (obj.localScale.x * ScaleR));
        StartCoroutine(scl.Scale(obj, toThisScale, 3f));

        Values valsLine = Lines[CurrentIndex].GetComponent<Values>();
        valsLine.UserImage.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        valsLine.UserImage.GetComponent<Image>().overrideSprite = Images[0];
        valsLine.Effect.SetActive(true);
    }

    ScaleEffect scl = new ScaleEffect();
    void setInactivePrevious()
    {
        QBoxes[CurrentIndex].GetComponent<Values>().Effect.SetActive(false);
        QBoxes[CurrentIndex].GetComponent<Values>().UserImage.SetActive(true);
        Transform obj = QBoxes[CurrentIndex].transform.GetChild(0).transform.GetChild(0).transform;
        Vector2 toThisScale = new Vector2(obj.localScale.x - (obj.localScale.x * ScaleR), obj.localScale.y - (obj.localScale.x * ScaleR));
        StartCoroutine(scl.Scale(obj, toThisScale, 3f));

        Values valsLine = Lines[CurrentIndex].GetComponent<Values>();
        if(results[CurrentIndex] == 1)
            valsLine.UserImage.GetComponent<Image>().overrideSprite = Images[1];
        else
            valsLine.UserImage.GetComponent<Image>().overrideSprite = Images[2];
    }

    public delegate void Change();
    public event Change changingResults;
    public void CheckEqual(Text number)
    {
        if (Finish)
            return;

        if (PunView == null)
            SetPunView();

        int num = int.Parse(number.text);
        string[] nums = QBoxes[CurrentIndex].GetComponent<Values>().Text.text.Trim(' ').Split('+');
        int result = int.Parse(nums[0]) + int.Parse(nums[1]);
        if (num.Equals(result))
        {
            results[CurrentIndex] = 1;
            changingResults?.Invoke();
            setInactivePrevious();
            coins += CntCoin;
            CoinText.text = coins.ToString();
            CurrentIndex++;
            PunView.RPC("GoUp", RpcTarget.All, CurrentIndex, PunView.IsMine);
        }
        else
        {
            results[CurrentIndex] = 0;
            changingResults?.Invoke();
            setInactivePrevious();
            CurrentIndex++;
            PunView.RPC("GoUp", RpcTarget.All, CurrentIndex, PunView.IsMine);
        }
    }

    void SetPunView()
    {
        PunView = GameObject.Find("enemy").transform.GetComponent<PhotonView>();
    }
}