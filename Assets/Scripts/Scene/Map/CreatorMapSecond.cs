using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using System;
using UnityEngine.UI;

public class CreatorMapSecond : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject QuestionTemplate = null;
    public GameObject[] AnswerButtons = null;
    public GameObject WaitingPanel = null;
    public GameObject WinPanel = null;
    public GameObject ImageWinPanel = null;
    public Text TextWinPanel = null;

    GameObject currentTemp;
    SpriteRenderer[] currentTempChildren = new SpriteRenderer[2];
    int currentCount = 1;
    private void Start()
    {
        currentTemp = Instantiate(QuestionTemplate, this.transform, false);
        currentTemp.name = "Template" + currentCount; currentCount++;
        currentTemp.transform.localPosition = new Vector3(-1250, 0);

        for (int i = 0; i < currentTemp.transform.childCount; i++)
        {
            currentTempChildren[i] = currentTemp.transform.GetChild(i).GetComponent<SpriteRenderer>();
        }
        StartCoroutine(IeStart());
    }

    private void SetQuestions()
    {
        int[] randNums = new int[2];
        for (int i = 0; i < 3; i++)
        {

        }
    }
    private int getRandomNumber(int[] target, int min, int max)
    {
        int rand = UnityEngine.Random.Range(min, max);
        while (!target.Contains(rand))
        {
            rand = UnityEngine.Random.Range(min, max);
        }

        return rand;
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
        Time.timeScale = 0;
        StartCoroutine(Scale(WinPanel.transform, new Vector3(1, 1, 1), 8f));
    }

    private IEnumerator Scale(Transform transF, Vector3 targetScale, float scaleDuration)
    {
        for (float t = 0; t < 1; t += Time.deltaTime / scaleDuration)
        {
            transF.localScale = Vector3.Lerp(transF.localScale, targetScale, t);
            if (transF.localScale.Equals(targetScale))
            {
                IEnumerator co = Scale(null, Vector3.zero, 0);
                StopCoroutine(co);
                yield break;
            }
            yield return null;
        }
    }
}
