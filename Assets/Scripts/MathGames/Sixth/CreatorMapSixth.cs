using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using System;
using UnityEngine.UI;
using System.Linq;

public class CreatorMapSixth : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public bool StartWithoutPlayer = false;
    [Header("Main")]
    public GameObject[] QuestionTemplate = null;
    public GameObject[] AnswerButtons = null;
    private GameObject[] questions = null;
    public Transform Parent = null;
    public int CountQuestions = 10;
    public ClickButton6 click = null;
    [Header("LeftBar")]
    public GameObject PlayerLvlTemplate = null;
    public GameObject LeftBar = null;
    private GameObject[] lvls = null;
    private GameObject _scrollContent = null;
    [Header("RightBar")]
    public GameObject EnemyLvlTemplate = null;
    public GameObject RightBar = null;
    [Header("End")]
    public GameObject WaitingPanel = null;
    public GameObject WinPanel = null;
    public GameObject ImageWinPanel = null;
    public Text TextWinPanel = null;
    [Header("Info")]
    public GameObject Player = null;
    public GameObject Enemy = null;

    GameObject currentTemp;
    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        _scrollContent = LeftBar.transform.GetChild(0).transform.GetChild(0).gameObject;
        setInfos();

        click.AnswerBtns = AnswerButtons;

        questions = new GameObject[CountQuestions];
        _answers = new int[CountQuestions];
    }
    private void setInfos()
    {
        Player.GetComponentInChildren<Text>().text = PlayerPrefs.GetString("nameUser") + " " + PlayerPrefs.GetString("surnameUser");
    }

    int cntElementsCount;
    void setQuestions()
    {
        questions = new GameObject[CountQuestions];

        cntElementsCount = QuestionTemplate[0].transform.GetChild(0).transform.childCount;
        for (int i = 0; i < CountQuestions; i++)
        {
            questions[i] = Instantiate(QuestionTemplate[0], Parent.transform);
            questions[i].name = "Q" + (i + 1).ToString();

            makeQuestion(i);
            
            questions[i].transform.localPosition = new Vector3(800, 0);
        }
        StartCoroutine(IStart());
    }

    GameObject tempChild;

    int[] randNumsLessThanResult(int target)
    {
        int num1 = UnityEngine.Random.Range(1, target);
        int num2 = UnityEngine.Random.Range(1, target);
        while (num1 + num2 > target)
        {
            num1 = UnityEngine.Random.Range(1, target);
            num2 = UnityEngine.Random.Range(1, target);
        }

        return new int[] { num1, num2 };
    }


    int[] nums;
    private void makeQuestion(int i)
    {
        int randAnswer = UnityEngine.Random.Range(0, 10);

        questions[i].transform.GetChild(0).GetChild(randAnswer).gameObject.SetActive(true);



        _answers[i] = randAnswer + 1;

        if (i == 0)
        {
            int[] randAnswers = makeRandomlyNumWithoutEquals(new int[] { randAnswer }, 1, 10, AnswerButtons.Length);
            int randIndex = UnityEngine.Random.Range(0, AnswerButtons.Length);
            for (int l = 0; l < AnswerButtons.Length; l++)
            {
                if (randIndex != l)
                    AnswerButtons[l].GetComponentInChildren<Text>().text = randAnswers[l].ToString();
                else
                    AnswerButtons[l].GetComponentInChildren<Text>().text = _answers[i].ToString();
            }
        }
    }
    private int[] _answers;
    
    private IEnumerator IStart()
    {
        yield return new WaitForSeconds(0.1f);
        ScrollRect lineScrollRect = LeftBar.GetComponent<ScrollRect>();
        StartCoroutine(click.LerpToChild(lineScrollRect, lvls[0].GetComponent<RectTransform>()));
        
        yield return new WaitForSeconds(1);
        StartCoroutine(effect.MoveAnimTowards(questions[0].transform, new Vector3(0, 0), true, 8f));
    }
    private void Start()
    {
        setLvlPanels();
        setQuestions();

        click.Lvls = lvls;
        click.Questions = questions;
        click.Answers = _answers;
        //click.Stats = Player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
    }

    private void setLvlPanels()
    {
        lvls = new GameObject[CountQuestions];
        for (int i = 0; i < CountQuestions; i++)
        {
            lvls[i] = Instantiate(PlayerLvlTemplate, _scrollContent.transform);
            lvls[i].name = "lvlPanel" + (i + 1);
            lvls[i].transform.SetAsLastSibling();
            lvls[i].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = (i + 1).ToString();
            if (i == 0)
            {
                lvls[i].transform.GetChild(1).GetChild(0).GetComponent<OpacityEffect>().enabled = true;
            }
        }

            GameObject temp = Instantiate(PlayerLvlTemplate, _scrollContent.transform);
            temp.name = "space";
            temp.transform.SetAsLastSibling();

        setLBContentHeight();
    }
    void setLBContentHeight()
    {
        float scrollContentHeightLB = _scrollContent.transform.parent.parent.GetComponent<RectTransform>().rect.height;
        RectTransform rtLB = _scrollContent.GetComponent<RectTransform>();
        rtLB.offsetMax = new Vector2(rtLB.offsetMax.x, Mathf.Abs(scrollContentHeightLB + (CountQuestions * 20)));
    }


    int loopCount = 0;
    int[] makeRandomlyNumWithoutEquals(int[] targetNum, int min, int max, int count)
    {
        int[] num = new int[count];

        int rand;
        for (int i = 0; i < num.Length; i++)
        {
            rand = UnityEngine.Random.Range(min, max);
            while (targetNum.Contains(rand) || num.Contains(rand))
            {
                rand = UnityEngine.Random.Range(min, max);

                loopCount++;
                if (loopCount > 10000)
                {
                    Debug.Log("Stop it");
                    loopCount = 0;
                    break;
                }
            }
            num[i] = (rand);
        }


        return num;
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
            if(equalNums)
                rand = UnityEngine.Random.Range(min, max);

            rands[i] = rand;
        }

        return rands;
    }
    ScaleEffect effect = new ScaleEffect();

    bool isOnEvent = false;
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 52)
        {
            Debug.Log("Get Event");
            WaitingPanel.SetActive(false);
            isOnEvent = true;
            Start();
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (Server.isOwnerRoom)
        {
            Debug.Log("Send event");
            WaitingPanel.SetActive(false);
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            SendOptions sendOption = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(52, "", options, sendOption);
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        WinOrLose(true);
    }

    public void WinOrLose(bool win)
    {
        WinPanel.SetActive(true);
        if (win)
        {
            TextWinPanel.text = "Вы выграли!";
        }
        else
        {
            TextWinPanel.text = "Вы проиграли!";
            ImageWinPanel.transform.localScale = new Vector3(.8f, .8f, 1);
            ImageWinPanel.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("MathGame/FirstBattle/Abort");
        }
        StartCoroutine(effect.Scale(WinPanel.transform, new Vector3(1, 1, 1), 8f));
        Time.timeScale = 0;
    }
    
}
