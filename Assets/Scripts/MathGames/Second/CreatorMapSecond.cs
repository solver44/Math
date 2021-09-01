using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using System;
using UnityEngine.UI;
using System.Linq;

public class CreatorMapSecond : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [Header("Main")]
    public GameObject QuestionTemplate = null;
    public GameObject[] AnswerButtons = null;
    private GameObject[] questions = null;
    public Transform Parent = null;
    public int CountQuestions = 10;
    public ClickButton2 click = null;
    [Header("Shape and colors")]
    public Sprite[] Shapes = null;
    public Color[] Colors = null;
    [Header("LeftTopBar")]
    public GameObject LvlPrefab = null;
    public GameObject LeftTopBar = null;
    private GameObject[] lvls = null;
    private GameObject _scrollContent = null;
    [Header("End")]
    public GameObject WaitingPanel = null;
    public GameObject WinPanel = null;
    public GameObject ImageWinPanel = null;
    public Text TextWinPanel = null;
    [Header("Info")]
    public GameObject Me = null;
    public int Health = 5;

    GameObject currentTemp;
    Image[] currentTempChildren = new Image[3];
    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        _scrollContent = LeftTopBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
        setInfos();

        click.AnswerBtns = AnswerButtons;
        click.Shapes = Shapes;
        click.Colors = Colors;
        click.StatsHealth = statsHealth;
        click.Health = Health;

        questions = new GameObject[CountQuestions];
    }
    Text statsHealth = null;
    private void setInfos()
    {
        Me.transform.GetChild(1).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("nameUser") + " " + PlayerPrefs.GetString("surnameUser");
        if(PlayerPrefs.HasKey("CurrentLevel"))
            Me.transform.GetChild(2).GetComponent<Text>().text = Me.transform.GetChild(2).GetComponent<Text>().text.Replace("!lvl", PlayerPrefs.GetInt("CurrentLevel").ToString());
        else
            Me.transform.GetChild(2).GetComponent<Text>().text = Me.transform.GetChild(2).GetComponent<Text>().text.Replace("!lvl", "0");

        statsHealth = Me.transform.GetChild(3).GetComponentInChildren<Text>();
        statsHealth.text = Health.ToString();
    }
    private void setLvls()
    {
        lvls = new GameObject[CountQuestions];
        GameObject temp;
        float scrollContentWidthLB = LeftTopBar.GetComponent<RectTransform>().rect.width;
        RectTransform content = _scrollContent.GetComponent<RectTransform>();
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CountQuestions * 102);
        for (int i = 0; i < CountQuestions; i++)
        {
            temp = Instantiate(LvlPrefab, _scrollContent.transform, false);
            temp.name = "lvl" + (i + 1).ToString();
            temp.GetComponentInChildren<Text>().text = (i + 1).ToString();
            temp.transform.localPosition = new Vector3((temp.GetComponent<RectTransform>().rect.width / 2) * (i+1) + i*35, -temp.GetComponent<RectTransform>().rect.height / 2 - 10);
            temp.transform.SetAsFirstSibling();
            lvls[i] = temp;
        }
        lvls[0].GetComponent<Image>().color = new Color32(255, 212, 47, 255);
    }
    private void Start()
    {
        setLvls();
        SetQuestions();

        click.Lvls = lvls;
        click.Questions = questions;
    }
    public void SetQuestions()
    {
        int[] randNums;

        for (int i = 0; i < CountQuestions; i++)
        {
            randNums = getRandomNumber(0, 3, 2, false);
            currentTemp = Instantiate(QuestionTemplate, Parent, false);
            currentTemp.name = "Template" + (i+1).ToString();
            currentTemp.transform.localPosition = new Vector3(-1250, 0);

            for (int k = 0; k < randNums.Length; k++)
            {
                currentTempChildren[randNums[k]] = currentTemp.transform.GetChild(randNums[k]).GetComponent<Image>();
                currentTempChildren[randNums[k]].overrideSprite = Shapes[UnityEngine.Random.Range(0, Shapes.Length)];
                currentTempChildren[randNums[k]].color = Colors[UnityEngine.Random.Range(0, Colors.Length)];
                currentTempChildren[randNums[k]].GetComponent<Image>().SetNativeSize();
            }

            questions[i] = currentTemp;
        }
        currentTemp = questions[0];
        StartCoroutine(IeStart());
        Image temp;
        IDictionary<int, Sprite> values = new Dictionary<int, Sprite>();
        for (int i = 0; i < 3; i++)
        {
            temp = questions[0].transform.GetChild(i).GetComponent<Image>();
            if (temp.overrideSprite != null)
            {
                values.Add(i, temp.overrideSprite);
            }
        }
        int randIndex = values.Keys.ElementAt(UnityEngine.Random.Range(0, values.Count()));
        int[] randAnsIndexes = getRandomNumber(0, 2, 2, false);

        Image child = AnswerButtons[randAnsIndexes[0]].transform.GetChild(0).GetComponent<Image>();
        child.overrideSprite = values[randIndex];
        child.color = Colors[UnityEngine.Random.Range(0, Colors.Length)];
        child.SetNativeSize();

        child = AnswerButtons[randAnsIndexes[1]].transform.GetChild(0).GetComponent<Image>();
        child.overrideSprite = Shapes[makeRandomlyNumWithoutEquals(Array.FindIndex(Shapes, c => c == values[randIndex]), 0, Shapes.Length)];
        child.color = Colors[UnityEngine.Random.Range(0, Colors.Length)];
        child.SetNativeSize();
    }
    int makeRandomlyNumWithoutEquals(int targetNum, int min, int max)
    {
        while (true)
        {
            int num = UnityEngine.Random.Range(min, max);
            if (targetNum != num)
                return num;
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
            if(equalNums)
                rand = UnityEngine.Random.Range(min, max);

            rands[i] = rand;
        }

        return rands;
    }
    ScaleEffect effect = new ScaleEffect();
    private IEnumerator IeStart()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(effect.MoveAnimTowards(currentTemp.transform, new Vector2(-37, 0), true, 12f));
    }

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
