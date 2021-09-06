﻿using Photon.Pun;
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
    public GameObject[] QuestionTemplate = null;
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
    List<Image[]> columnTempChildren = new List<Image[]>();
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

        int[] randShapesIndexes = null;
        int[] randColorIndexes = null;

        List<int> prevShape = new List<int>();

        Sprite[] shapes = null;
        for (int i = 0, c = 0, secondPart = 0; i < CountQuestions; i++)
        {
            currentTemp = Instantiate(QuestionTemplate[c], Parent, false);
            currentTemp.name = "Template" + (i+1).ToString();
            currentTemp.transform.localPosition = new Vector3(-1250, currentTemp.transform.localPosition.y);

            if (prevShape.Count > 0 && currentTemp.transform.childCount < 2)
            {
                randShapesIndexes = getRandomNumber(0, Shapes.Length, currentTemp.transform.childCount, false);
                while (prevShape.Contains(randShapesIndexes[0]))
                {
                    randShapesIndexes = getRandomNumber(0, Shapes.Length, currentTemp.transform.childCount, false);
                }
            }
            else
                randShapesIndexes = getRandomNumber(0, Shapes.Length, currentTemp.transform.childCount, false);

            randColorIndexes = getRandomNumber(0, Colors.Length, 3, false);

            int randColumn = UnityEngine.Random.Range(0, currentTemp.transform.childCount);
            if (secondPart == 1)
            {
                shapes = blendArrays(currentTemp.transform.childCount * 3, randColorIndexes, randShapesIndexes);
            }
            for (int l = 0, q = 0; l < currentTemp.transform.childCount; l++)
            {
                currentTempChildren = new Image[3];

                if (randColumn == l)
                    randNums = getRandomNumber(0, 3, 2, false);
                else
                    randNums = getRandomNumber(0, 3, 3, false);

                for (int k = 0; k < randNums.Length; k++)
                {
                    if (secondPart == 0)
                    {
                        currentTempChildren[randNums[k]] = currentTemp.transform.GetChild(l).transform.GetChild(randNums[k]).GetComponent<Image>();
                        currentTempChildren[randNums[k]].overrideSprite = Shapes[randShapesIndexes[l]];
                        currentTempChildren[randNums[k]].color = Colors[randColorIndexes[randNums[k]]];
                        currentTempChildren[randNums[k]].GetComponent<Image>().SetNativeSize();
                    }
                    else
                    {
                        currentTempChildren[k] = currentTemp.transform.GetChild(l).transform.GetChild(k).GetComponent<Image>();
                        currentTempChildren[k].overrideSprite = shapes[q];
                        q++;
                        currentTempChildren[k].color = Colors[randColorIndexes[k]];
                        currentTempChildren[k].GetComponent<Image>().SetNativeSize();
                    }
                }
                if (randNums.Length < 3)
                    q++;

                columnTempChildren.Add(currentTempChildren);
            }

            if (!prevShape.Contains(randShapesIndexes[0]))
                prevShape.Add(randShapesIndexes[0]);

            //OTH
            if ((i + 1) % (CountQuestions / QuestionTemplate.Length) == 0 && c < QuestionTemplate.Length)
            { c++; secondPart = 0; }
            if ((i + 1) >= ((c + 1) * (CountQuestions / QuestionTemplate.Length)) - ((CountQuestions / QuestionTemplate.Length) / 2) && currentTemp.transform.childCount > 2)
                secondPart = 1;

            //Debug.Log(((c + 1) * (CountQuestions / QuestionTemplate.Length)) - ((CountQuestions / QuestionTemplate.Length) / 2));

            questions[i] = currentTemp;
        }
        currentTemp = questions[0];
        StartCoroutine(IeStart());
        Image temp;
        IDictionary<int, Sprite> values = new Dictionary<int, Sprite>();
        IDictionary<int, Color> colorVals = new Dictionary<int, Color>();
        int[] indexes = new int[2];

        int rowIndex = columnTempChildren.IndexOf(columnTempChildren.Where(c => c.Contains(null)).First());
 
        for (int i = 0, cnt = 0; i < 3; i++)
        {
            temp = questions[0].transform.GetChild(rowIndex).transform.GetChild(i).GetComponent<Image>();
            if (temp.overrideSprite != null)
            {
                indexes[cnt] = i;
                cnt++;
                values.Add(i, temp.overrideSprite);
                colorVals.Add(i, temp.color);

            }
        }

        int randIndex = values.Keys.ElementAt(UnityEngine.Random.Range(0, values.Count()));
        int[] randAnsIndexes = getRandomNumber(0, 2, 2, false);

        Image child = AnswerButtons[randAnsIndexes[0]].transform.GetChild(0).GetComponent<Image>();
        child.overrideSprite = values[randIndex];
        child.color = Colors[makeRandomlyNumWithoutEquals(new int[] { Array.FindIndex(Colors, c => c == colorVals[indexes[0]]), Array.FindIndex(Colors, c => c == colorVals[indexes[1]]) }, 0, Colors.Length)];
        child.SetNativeSize();

        child = AnswerButtons[randAnsIndexes[1]].transform.GetChild(0).GetComponent<Image>();
        child.overrideSprite = Shapes[makeRandomlyNumWithoutEquals(new int[] { Array.FindIndex(Shapes, c => c == values[randIndex]) }, 0, Shapes.Length)];
        child.color = Colors[UnityEngine.Random.Range(0, Colors.Length)];
        child.SetNativeSize();
    }

    private Sprite[] blendArrays(int arrayLength, int[] colorIndexes, int[] shapesIndexes)
    {
        Sprite[] result = new Sprite[arrayLength];
        string[] counter = new string[shapesIndexes.Length];

        int rowCnt = arrayLength / 3, currRowIndex, currColumnIndex, tempI = 0;
        int[] prevColumnIndex, prevRowIndex;

        //int[,] locationsAllShapes = new int[rowCnt, 3];
        int tempRandCnt;
        int countOfLoop = 0;

        int[] indexes = new int[arrayLength];
        for (int i = 0; i < arrayLength; i++)
        {
            currRowIndex = i / 3;
            currColumnIndex = i - (currRowIndex * 3);
            indexes[i] = i;
            if (currRowIndex == 1)
            {
                if (currColumnIndex == 1)
                    indexes[i] = i + 1;
                else if (currColumnIndex == 2)
                    indexes[i] = i - 1;
            }
        }

        while (tempI < arrayLength)
        {
            tempRandCnt = UnityEngine.Random.Range(0, shapesIndexes.Length);

            currRowIndex = indexes[tempI] / 3;
            currColumnIndex = indexes[tempI] - (currRowIndex * 3);

            if (!string.IsNullOrEmpty(counter[tempRandCnt]))
            {
                prevRowIndex = counter[tempRandCnt].Split('/').Where(c => !string.IsNullOrEmpty(c)).Select(c => int.Parse(c) / 3).ToArray();
                prevColumnIndex = counter[tempRandCnt].Split('/').Where(c => !string.IsNullOrEmpty(c)).Select(c => int.Parse(c) - (prevRowIndex[Array.FindIndex(counter[tempRandCnt].Split('/'), f => f == c)] * 3)).ToArray();
                
            }
            else
            {
                prevColumnIndex = null;
                prevRowIndex = null;
            }
            if (string.IsNullOrEmpty(counter[tempRandCnt]) || (prevRowIndex != null && !prevColumnIndex.Contains(currColumnIndex) && !prevRowIndex.Contains(currRowIndex)))
            {
                result[indexes[tempI]] = Shapes[shapesIndexes[tempRandCnt]];
                counter[tempRandCnt] += indexes[tempI] + "/";

                tempI++;
            }
            else
            {
                countOfLoop++;
            }

            if (countOfLoop > 1000)
            {
                Debug.Log("Stop it");
                //result[tempI] = result[tempI - 1];
                //counter[Array.FindIndex(shapesIndexes, c => c == Array.FindIndex(Shapes, f => f == result[tempI - 1]))] = tempI;
                //result[tempI - 1] = Shapes[shapesIndexes[tempRandCnt]];
                //counter[tempRandCnt] = tempI - 1;
                //tempI++;  
                break;
            }
            //breakLoop = false;

            //if (counter[tempRandCnt] > 0)
            //{
            //    for (int l = 0; l < currColumnIndex + 1; l++)
            //    {
            //        if (locationsAllShapes[currRowIndex, l] == tempRandCnt + 1)
            //        {
            //            breakLoop = true;
            //            break;
            //        }
            //    }

            //    if (!breakLoop)
            //    {
            //        for (int i = 0; i < rowCnt; i++)
            //        {
            //            if (locationsAllShapes[i, currColumnIndex] == tempRandCnt + 1)
            //            {
            //                breakLoop = true;
            //                break;
            //            }
            //        }
            //    }
            //}

            //if (!breakLoop && counter[tempRandCnt] < 3)
            //{
            //    locationsAllShapes[currRowIndex, currColumnIndex] = tempRandCnt + 1;

            //    array1[tempI] = Shapes[shapesIndexes[tempRandCnt]];
            //    counter[tempRandCnt]++;
            //    tempI++;
            //}

        }
        foreach (var item in result)
        {
            Debug.Log(item.name);
        }
        return result;
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
