using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ClickButton7 : MonoBehaviour
{
    [HideInInspector] public bool StartWithoutPlayer = true;
    [HideInInspector] public GameObject[] Lvls = null;
    [HideInInspector] public GameObject[] Questions = null;
    [HideInInspector] public GameObject[] AnswerBtns = null;
    [HideInInspector] public Text PlayerScore = null;
    [HideInInspector] public int[] Answers;

    [HideInInspector] public bool Finish = false;

    private void lose(PhotonView punView)
    {
        Debug.Log("Finish");
        if (!StartWithoutPlayer)
        {
            try
            {
                punView.RPC("CheckWinOrLose", RpcTarget.All, results);
            }
            catch {
                CreatorMapSeventh map = GameObject.Find("Creator").GetComponent<CreatorMapSeventh>();
                map.WinOrLose(false);
            }
        }
        else
        {
            CreatorMapSeventh map = GameObject.Find("Creator").GetComponent<CreatorMapSeventh>();
            map.WinOrLose(false);
        }
    }
    private void changeStats(bool isTrue)
    {
        int currNum = Int32.Parse(PlayerScore.text);
        if (isTrue)
            PlayerScore.text = (currNum + 2).ToString();
        else if (currNum > 0)
            PlayerScore.text = (currNum - 1).ToString();
    }
    int[] results;
    ScaleEffect effect = new ScaleEffect();

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
        StartCoroutine(effect.MoveAnimTowards(Lvls[currentIndex].transform, new Vector2(0, 0), true, 5f));
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
                    CreatorMapSeventh map = GameObject.Find("Creator").GetComponent<CreatorMapSeventh>();
                    map.WinOrLose(true);
                }
            }
            else
            {
                CreatorMapSeventh map = GameObject.Find("Creator").GetComponent<CreatorMapSeventh>();
                map.WinOrLose(true);
            }
            return true;
        }
        return false;
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
    public void CreateNewAns()
    {
        int[] randWrongNums = makeRandomlyNumWithoutEquals(new int[] { Answers[currentIndex] }, 1, 10, AnswerBtns.Length);
        int randIndex = UnityEngine.Random.Range(0, AnswerBtns.Length);

        int randEqualPlace = makeRandomlyNumWithoutEquals(new int[] { randIndex }, 1, AnswerBtns.Length);
        Color curColor = Questions[currentIndex].transform.GetChild(0).GetComponent<Image>().color;

        for (int l = 0; l < AnswerBtns.Length; l++)
        {
            if (currentIndex < Questions.Length / 2)
            {
                if (randIndex != l)
                    AnswerBtns[l].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = randWrongNums[l].ToString();
                else
                    AnswerBtns[l].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Answers[currentIndex].ToString();
            }
            else
            {
                if (randIndex != l)
                {
                    if (randEqualPlace != l)
                    {
                        AnswerBtns[l].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = randWrongNums[l].ToString();
                        AnswerBtns[l].transform.GetChild(0).localRotation = new Quaternion(0, 0, 0, 0);
                        AnswerBtns[l].transform.GetChild(0).GetChild(0).transform.localRotation = new Quaternion(0, 0, 0, 0);
                    }
                    else
                    {
                        AnswerBtns[l].transform.GetChild(0).localRotation = new Quaternion(0, 180, 0, 0);
                        AnswerBtns[l].transform.GetChild(0).GetChild(0).transform.localRotation = new Quaternion(0, 180, 0, 0);
                        AnswerBtns[l].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Answers[currentIndex].ToString();
                    }
                }
                else
                {
                    AnswerBtns[l].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Answers[currentIndex].ToString();
                    AnswerBtns[l].transform.GetChild(0).localRotation = new Quaternion(0, 0, 0, 0);
                    AnswerBtns[l].transform.GetChild(0).GetChild(0).transform.localRotation = new Quaternion(0, 0, 0, 0);
                }
            }
            AnswerBtns[l].transform.GetChild(0).GetComponent<Image>().color = curColor;
        }

        StartCoroutine(IeStart());
    }
    public void CheckEqual(Text value)
    {
        if (Finish || wait)
            return;

        bool isTrue = false;

        if (Answers[currentIndex] == Int32.Parse(value.text) && value.transform.parent.transform.localRotation.y == 1)
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
    public Image StatsPanel;
    private void setInactivePrevious(bool isAnsTrue)
    {
        changeStats(isAnsTrue);

        StartCoroutine(effect.MoveAnimTowards(Questions[currentIndex].transform, new Vector2(0, -800), true, 8f));
        StartCoroutine(effect.MoveAnimTowards(Lvls[currentIndex].transform, new Vector2(0, -200), true, 5f));

        if (isAnsTrue)
        {
            StatsPanel.color = new Color32(49, 255, 67, 0);
        }
        else
        {
            StatsPanel.color = new Color32(255, 49, 59, 0);
        }
        StatsPanel.GetComponent<Animator>().SetTrigger("start");
    }

    void SetPunView()
    {
        if (!StartWithoutPlayer)
            PunView = GameObject.Find("enemy").transform.GetComponent<PhotonView>();
    }
    private void Start()
    {
        results = new int[Lvls.Length];   
        //symbolAnimController.SetLayerWeight(0, 0f);
    }
}
