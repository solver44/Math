using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ClickButton : MonoBehaviour
{
    [HideInInspector] public bool StartWithoutPlayer = false;
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
            if (!StartWithoutPlayer)
                PunView.RPC("CheckWinOrLose", RpcTarget.All, results);
            else
            {
                CreatorMapFirst map = GameObject.Find("Creator").GetComponent<CreatorMapFirst>();
                map.WinOrLose(true, results.Where(c => c == 1).Count());
            }
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
    void increaseSizeUI(GameObject obj)
    {
        //StartCoroutine(MoveSmooth(rect, new Vector2(rect.localPosition.x, rect.localPosition.y + interval)));
        ScrollRect scrollRect = MainScroll.GetComponent<ScrollRect>();
        ScrollRect lineScrollRect = LeftBarScrollContent.GetComponent<ScrollRect>();
        //StartCoroutine(MoveSmooth(scrollRect, new Vector2(scrollRect.preferredHeight)));
        StartCoroutine(LerpToChild(scrollRect, QBoxes[_currentIndex].GetComponent<RectTransform>(), true));
        StartCoroutine(LerpToChild(lineScrollRect, Lines[_currentIndex].GetComponent<RectTransform>(), false));
    }
    private IEnumerator LerpToChild(ScrollRect _scrollRectComponent, RectTransform target, bool isMain)
    {
        RectTransform child;
        if(isMain)
            child = _scrollRectComponent.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        else
            child = _scrollRectComponent.transform.GetChild(1).GetComponent<RectTransform>();

        Vector2 _lerpTo = child.anchoredPosition - (Vector2)_scrollRectComponent.transform.InverseTransformPoint(target.position) - new Vector2(0, 690);
        bool _lerp = true;
        Canvas.ForceUpdateCanvases();

        float decelerate = 3f;
        for (float i = 0; i < 1; i += Time.deltaTime * decelerate)
        {
            child.anchoredPosition = Vector2.Lerp(child.anchoredPosition, _lerpTo, i);
            if (Vector2.Distance(child.anchoredPosition, _lerpTo) < 0.25f)
            {
                child.anchoredPosition = _lerpTo;
                yield break;
            }
            yield return new WaitForEndOfFrame();
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
    void changedIndex()
    {
        if (checkWin())
            return;

        //if(CurrentIndex == 12)
        increaseSizeUI(MainScrollContent);

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
    bool wait = false;

    private IEnumerator dontWait()
    {
        wait = true;
        yield return new WaitForSeconds(0.3f);
        wait = false;
    }
    public void CheckEqual(Text number)
    {
        if (Finish || wait)
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
            if(!StartWithoutPlayer)
                PunView.RPC("GoUp", RpcTarget.All, CurrentIndex, PunView.IsMine);
        }
        else
        {
            results[CurrentIndex] = 0;
            changingResults?.Invoke();
            setInactivePrevious();
            if (coins > -1)
            {
                coins -= CntCoin;
                CoinText.text = coins.ToString();
            }
            CurrentIndex++;
            if (!StartWithoutPlayer)
                PunView.RPC("GoUp", RpcTarget.All, CurrentIndex, PunView.IsMine);
        }
        StartCoroutine(dontWait());
    }

    void SetPunView()
    {
        if(!StartWithoutPlayer)
            PunView = GameObject.Find("enemy").transform.GetComponent<PhotonView>();
    }
}
