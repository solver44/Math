using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using System;
using UnityEngine.UI;
using System.Linq;

public class CreatorMapSeventh : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public bool StartWithoutPlayer = false;
    [Header("Main")]
    public GameObject[] QuestionTemplate = null;
    public GameObject[] AnswerButtons = null;
    private GameObject[] questions = null;
    public Transform Parent = null;
    public int CountQuestions = 10;
    public ClickButton7 click = null;
    private int score = 0;
    [Header("Colors")]
    public Color[] Colors = null;
    [Header("LeftBar")]
    public GameObject LvlTemplate = null;
    public GameObject LeftBar = null;
    public GameObject PlayerLvl = null;
    public Text PlayerScore = null;
    private GameObject[] lvls = null;
    [Header("RightBar")]
    public GameObject RightBar = null;
    public Text EnemyScore = null;
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
        setInfos();

        click.AnswerBtns = AnswerButtons;

        questions = new GameObject[CountQuestions];
        _answers = new int[CountQuestions];
        lvls = new GameObject[CountQuestions];
    }
    private void setInfos()
    {
        Player.transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetString("nameUser") + " " + PlayerPrefs.GetString("surnameUser");
        Player.transform.GetChild(1).GetComponent<Text>().text = PlayerPrefs.GetInt("CurrentLevel").ToString();
    }

    void setQuestions()
    {
        questions = new GameObject[CountQuestions];
        
        for (int i = 0; i < CountQuestions; i++)
        {
            questions[i] = Instantiate(QuestionTemplate[0], Parent.transform);
            questions[i].name = "Q" + (i + 1).ToString();

            makeQuestion(i);
            
            questions[i].transform.localPosition = new Vector3(0, 600);
        }
        StartCoroutine(IStart());
    }

    GameObject tempChild;
    int[] randNumsLessThanResult(int target)
    {
        int num1 = UnityEngine.Random.Range(1, target);
        int num2 = UnityEngine.Random.Range(1, target);
        while (num1 >= target || num2 >= num1)
        {
            num1 = UnityEngine.Random.Range(1, target);
            num2 = UnityEngine.Random.Range(1, target);

            loopCount++;
            if(loopCount > 10000)
            {
                Debug.Log("Stop it");
                loopCount = 0;
                break;
            }
        }

        return new int[] { num1, num2 };
    }

    int[] nums;
    private void makeQuestion(int i)
    {
        int[] randNums = randNumsLessThanResult(10);

        questions[i].GetComponentInChildren<Text>().text = randNums[0].ToString() + "-" + randNums[1].ToString();

        _answers[i] = randNums[0] - randNums[1];

        int randColor = UnityEngine.Random.Range(0, Colors.Length);
        questions[i].transform.GetChild(0).GetComponent<Image>().color = Colors[randColor];

        if (i == 0)
        {
            int[] randWrongNums = makeRandomlyNumWithoutEquals(new int[] { _answers[i] }, 1, 10, AnswerButtons.Length);
            int randIndex = UnityEngine.Random.Range(0, AnswerButtons.Length);
            for (int l = 0; l < AnswerButtons.Length; l++)
            {
                if (randIndex != l)
                    AnswerButtons[l].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = randWrongNums[l].ToString();
                else
                    AnswerButtons[l].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = _answers[i].ToString();

                AnswerButtons[l].transform.GetChild(0).GetComponent<Image>().color = Colors[randColor];
            }
        }
    }
    private int[] _answers;

    private IEnumerator IStart()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(effect.MoveAnimTowards(questions[0].transform, new Vector3(0, 0), true, 8f));
        StartCoroutine(effect.MoveAnimTowards(lvls[0].transform, new Vector3(0, 0), true, 5f));
        //questions[0].transform.localPosition = new Vector3(0, 0);
    }
    private void Start()
    {
        setLvlLabels();
        setQuestions();

        click.Questions = questions;
        click.Lvls = lvls;
        click.Answers = _answers;
        click.PlayerScore = PlayerScore;
        PlayerScore.text = "0";
        EnemyScore.text = "0";
    }

    private void setLvlLabels()
    {
        for (int i = 0; i < CountQuestions; i++)
        {
            lvls[i] = Instantiate(LvlTemplate, PlayerLvl.transform);
            lvls[i].name = "lvl" + (i + 1);
            lvls[i].GetComponent<Text>().text = (i + 1).ToString();
            lvls[i].transform.localPosition = new Vector2(0, 200);
        }
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
    }
    
}
