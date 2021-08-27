using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CreatorMap : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [Header("Timer")]
    public Timer timer = null;

    [Header("Panels")]
    public GameObject WaitingPanel = null;
    public GameObject WinPanel = null;
    public GameObject imageWinPanel = null;
    public Text TextWinPanel = null;

    [Header("Main")]
    [SerializeField] private GameObject parent = null;
    public GameObject QuestionBox = null;
    public GameObject Cup = null;
    [SerializeField] private GameObject Vertical = null;
    public int CountOfBoxes = 1;
    public ClickButton click = null;
    public int coinCount = 1;

    [Header("LeftBar")]
    [SerializeField] private GameObject parentLB = null;
    public GameObject Line = null;
    public Sprite[] ImagesToLine = null;

    [Header("LeftBottomBar")]
    public Text UserName = null;
    public Text Coin = null;

    [Header("AnswerButtons")]
    public Text[] AnswerTexts = null;
    public float ScaleR = 0.2f;


    private string[] _qBoxesExamples;

    public int[] ExamsResults;
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 42)
        {
            Debug.Log("Get Event");
            WaitingPanel.SetActive(false);
            isOnEvent = true;
            awake();
            _qBoxesExamples = (string[])photonEvent.CustomData;
            start();
            InstantiateEnemy();
            if (photonViewE != null)
                click.PunView = photonViewE;

            timer.StartTick();
        }
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
            imageWinPanel.transform.localScale = new Vector3(.8f, .8f, 1);
            imageWinPanel.GetComponent<Image>().overrideSprite = Resources.Load<Sprite>("MathGame/Battle/Abort");
        }
        Time.timeScale = 0;
        StartCoroutine(Scale(WinPanel.transform, new Vector3(1, 1, 1), 8f));
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (Server.isOwnerRoom)
        {
            Debug.Log("Send event");
            WaitingPanel.SetActive(false);
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            SendOptions sendOption = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(42, _qBoxesExamples, options, sendOption);
            timer.StartTick();
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        WinOrLose(true);
    }
    bool isOnEvent = false;
    private void awake()
    {
        _qBoxesExamples = new string[CountOfBoxes];
        UserName.text = PlayerPrefs.GetString("nameUser") + "\n" + PlayerPrefs.GetString("surnameUser");
        if (photonViewE != null)
            click.PunView = photonViewE;
        click.AnswerTexts = this.AnswerTexts;
        Coin.text = "0";
        click.MainScroll = Vertical.transform.parent.transform.parent.transform.parent.gameObject;
        click.MainScrollContent = Vertical;
        click.LeftBarScrollContent = parentLB.transform.parent.gameObject;
        click.Images = ImagesToLine;
        click.ScaleR = this.ScaleR;
        click.results = new int[CountOfBoxes];
        click.CoinText = Coin;
        click.CntCoin = coinCount;
    }
    void Awake()
    {
        WaitingPanel.SetActive(true);
        Screen.orientation = ScreenOrientation.Portrait;
        if (Server.isOwnerRoom)
            awake();

        click.changingResults += Click_changingResults;
    }

    private void Click_changingResults()
    {
        ExamsResults = click.results;
    }

    private Vector2 toThisScale = Vector2.zero;

    private void start()
    {
        var pos = TextAnchor.MiddleLeft;
        bool minus = false;

        click.QBoxes = new GameObject[CountOfBoxes];
        click.Lines = new GameObject[CountOfBoxes];

        for (int i = 0; i < CountOfBoxes; i++)
        {
            SetMain(i, ref pos, ref minus);

            SetLeftBar(i);
        }

        SetHeightMain();
        SetHeightLeftTopBar();
    }
    void Start()
    {
        if (Server.isOwnerRoom)
            start();
    }


    GameObject enemy = null;
    PhotonView photonViewE = null;
    public void InstantiateEnemy()
    {
        enemy = PhotonNetwork.Instantiate(Resources.Load<GameObject>("Enemy").name, new Vector3(0, 0, 0), Quaternion.identity);
        photonViewE = enemy.GetPhotonView();
        photonViewE.RPC("Setting", RpcTarget.All, "enemy", "ParentEnemy");
    }

    private void Server_isOwner(bool isOwn)
    {
        if (isOwn)
        {
            awake();
            start();
        }
    }

    void SetMain(int i, ref TextAnchor pos, ref bool minus)
    {
        click.QBoxes[i] = Instantiate(QuestionBox, Vertical.transform);
        click.QBoxes[i].name = click.QBoxes[i].name.Remove(10, 7); click.QBoxes[i].name = "QF" + (i + 1);
        click.QBoxes[i].transform.SetAsFirstSibling();
        click.QBoxes[i].GetComponent<HorizontalLayoutGroup>().childAlignment = pos;
        if (!minus)
            pos++;
        else
            pos--;

        if (pos == TextAnchor.LowerLeft)
        {
            pos = TextAnchor.MiddleCenter;
            minus = true;
        }
        else if (pos == TextAnchor.UpperRight)
        {
            pos = TextAnchor.MiddleCenter;
            minus = false;
        }
        click.QBoxes[i].GetComponentInChildren<Text>().text = (i + 1).ToString();

        int[] rand = makeRandomlyNumbers(2, 0, 6, false);
        int result = rand[0] + rand[1];
        if (!isOnEvent)
            _qBoxesExamples[i] = rand[0] + " + " + rand[1];
        else
        {
            string[] nums = _qBoxesExamples[i].Trim(' ').Split('+');
            result = int.Parse(nums[0]) + int.Parse(nums[1]);
        }
        click.QBoxes[i].GetComponent<Values>().Text.text = _qBoxesExamples[i];
        if (i == 0)
        {
            int randS = Random.Range(0, 2);
            click.QBoxes[i].GetComponent<Values>().Effect.SetActive(true);
            click.AnswerTexts[randS].text = result.ToString();
            if (randS == 1)
            {
                int num = makeRandomlyNumWithoutEquals(result, 0, 11);
                click.AnswerTexts[0].text = num.ToString();
            }
            else
            {
                int num = makeRandomlyNumWithoutEquals(result, 0, 11);
                click.AnswerTexts[1].text = num.ToString();
            }

            Transform obj = click.QBoxes[i].transform.GetChild(0).transform.GetChild(0).transform;
            toThisScale = new Vector2(obj.localScale.x + (obj.localScale.x * ScaleR), obj.localScale.y + (obj.localScale.x * ScaleR));
            StartCoroutine(Scale(obj, toThisScale, 2f));
        }
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
    int[] makeRandomlyNumbers(int cnt, int min, int max, bool sameNumbers)
    {
        int[] result = new int[cnt];
        for (int i = 0; i < cnt; i++)
        {
            int res = Random.Range(min, max);
            if (!sameNumbers)
            {
                while (true)
                {
                    int num = Random.Range(min, max);
                    if (checkIsNotEqual(result, num))
                    {
                        res = num;
                        break;
                    }
                }
            }
            result[i] = res;
        }

        return result;
    }
    bool checkIsNotEqual(int[] res, int num)
    {
        for (int j = 0; j < res.Length; j++)
        {
            if (num == res[j])
                return false;
        }
        return true;
    }
    void SetLeftBar(int i)
    {
        //LeftBar
        click.Lines[i] = Instantiate(Line, parentLB.transform);
        click.Lines[i].name = "l" + (i + 1);
        click.Lines[i].transform.SetAsFirstSibling();
        var vals = click.Lines[i].GetComponent<Values>();
        vals.Text.text = (i + 1).ToString();
        Image render = vals.UserImage.GetComponent<Image>();
        if (i == 0)
        {
            vals.UserImage.SetActive(true);
            render.overrideSprite = ImagesToLine[0];
            render.color = new Color(1, 1, 1, 1);
            vals.Effect.SetActive(true);
        }
        else
        {
            render.sprite = null;
            vals.Effect.SetActive(false);
        }
    }
    void SetHeightMain()
    {
        GameObject cup = Instantiate(Cup, Vertical.transform);
        cup.transform.SetAsFirstSibling();

        float scrollContentHeight = parent.GetComponent<RectTransform>().rect.height;
        RectTransform rt = Vertical.GetComponent<RectTransform>();
        rt.offsetMax = new Vector2(rt.offsetMax.x, Mathf.Abs(scrollContentHeight - (CountOfBoxes * 150) - 600));
        RectTransform rtParent = parent.GetComponent<RectTransform>();
        rtParent.offsetMax = new Vector2(rtParent.offsetMax.x, Mathf.Abs(scrollContentHeight - (CountOfBoxes * 150) - 600));
    }
    void SetHeightLeftTopBar()
    {
        //LeftTopBar
        float scrollContentHeightLB = parentLB.GetComponent<RectTransform>().rect.height;
        RectTransform rtLB = parentLB.GetComponent<RectTransform>();
        rtLB.offsetMax = new Vector2(rtLB.offsetMax.x, Mathf.Abs(scrollContentHeightLB - (CountOfBoxes * 320)));
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
