using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using System;
using UnityEngine.UI;
using System.Linq;

public class CreatorMapFourth : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public bool StartWithoutPlayer = false;
    [Header("Main")]
    public GameObject[] QuestionTemplate = null;
    public GameObject[] AnswerButtons = null;
    private GameObject[] questions = null;
    public Transform Parent = null;
    public int CountQuestions = 10;
    public ClickButton4 click = null;
    [Header("Elements")]
    public Sprite[] Elements = null;
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
    Image[] currentTempChildren = new Image[3];
    List<Image[]> columnTempChildren = new List<Image[]>();
    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        _scrollContent = LeftBar.transform.GetChild(0).transform.GetChild(0).gameObject;
        setInfos();

        click.AnswerBtns = AnswerButtons;
        click.Elements = Elements;

        questions = new GameObject[CountQuestions];
    }
    private void setInfos()
    {
        Player.transform.GetChild(1).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("nameUser") + " " + PlayerPrefs.GetString("surnameUser");
        Text tempUser = Player.transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>();
        if(PlayerPrefs.HasKey("CurrentLevel"))
            tempUser.text = tempUser.text = PlayerPrefs.GetInt("CurrentLevel").ToString();
        else
            tempUser.text = tempUser.text = "0";
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

            makeQuestion(i, UnityEngine.Random.Range(0, 2) == 1);
            
            questions[i].transform.localPosition = new Vector2(0, 500);
        }
        StartCoroutine(IStart());
    }

    GameObject tempChild;
    private void makeQuestion(int i, bool isElementsEqual)
    {
       
    }
    private IEnumerator IStart()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(effect.MoveAnimTowards(questions[0].transform, new Vector2(0, 0), true, 8f));
    }
    private void Start()
    {
        setLvlPanels();
        setQuestions();

        click.Lvls = lvls;
        click.Questions = questions;
        click.Stats = Player.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
    }

    private void setLvlPanels()
    {
        lvls = new GameObject[CountQuestions];
        for (int i = 0; i < CountQuestions; i++)
        {
            lvls[i] = Instantiate(PlayerLvlTemplate, _scrollContent.transform);
            lvls[i].name = "lvlPanel" + (i + 1);
            lvls[i].transform.SetAsFirstSibling();
            lvls[i].GetComponentInChildren<Text>().text = (i + 1).ToString();
            if (i == 0)
            {
                lvls[0].transform.GetChild(0).gameObject.SetActive(true);
                RectTransform num = lvls[0].transform.GetChild(1).GetComponent<RectTransform>();
                num.offsetMin = new Vector2(60, num.offsetMin.y);
            }
        }
        setLBContentHeight();
    }
    void setLBContentHeight()
    {
        //LeftTopBar
        float scrollContentHeightLB = _scrollContent.transform.parent.GetComponent<RectTransform>().rect.height;
        RectTransform rtLB = _scrollContent.GetComponent<RectTransform>();
        rtLB.offsetMax = new Vector2(rtLB.offsetMax.x, Mathf.Abs(scrollContentHeightLB - (CountQuestions * 60)));
    }


    int loopCount = 0;
    int makeRandomlyNumWithoutEquals(int[] targetNum, int min, int max)
    {
        int num;
        do
        {
            num = UnityEngine.Random.Range(min, max);
            loopCount++;
            if(loopCount > 5000)
            {
                Debug.Log("Stop it");

                break;
            }
        }
        while (targetNum.Contains(num));

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
