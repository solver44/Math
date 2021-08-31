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
    public Color[] colors = null;
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

    GameObject currentTemp;
    Image[] currentTempChildren = new Image[3];
    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Landscape;
        _scrollContent = LeftTopBar.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
        click.AnswerBtns = AnswerButtons;
        click.Shapes = Shapes;
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

        int firstRandomNum = 0;
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
                currentTempChildren[randNums[k]].color = colors[UnityEngine.Random.Range(0, colors.Length)];
            }
            StartCoroutine(IeStart());

            questions[i] = currentTemp;
        }
        Image temp = AnswerButtons[0].GetComponent<Image>();
        //temp.overrideSprite = Shapes.FirstOrDefault(c => c.name == questions[0]);
        //temp.overrideSprite = 
    }

    private int[] getRandomNumber(int min, int max, int count, bool equalNums)
    {
        int rand = UnityEngine.Random.Range(min, max);
        int[] rands = new int[count];
        for (int i = 0; i < rands.Length; i++)
        {
            rands[i] = rand;
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
        StartCoroutine(effect.MoveAnim(currentTemp.transform, new Vector2(-37, 0), true, 12f));
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
