using System.Collections;
using UnityEngine;

public class ShowChildAlpha : MonoBehaviour
{
    [SerializeField] private bool makeShow = true;
    public GameObject[] ChildAnswer = null;
    [SerializeField] private float visibleTime = 2f;

    [Header("Additional")]
    [SerializeField] private float rangeOfTransition = 0.5f;
    [SerializeField] private int sizeOfIndex = 18;
    public GameObject Parent = null;
    public bool IsWaitUntilClosing = true;

    private bool isItStart = true;
    private bool isFirstTime = true;

    private float[] r, g, b;
    private void Start()
    {
        r = new float[sizeOfIndex];
        g = new float[sizeOfIndex];
        b = new float[sizeOfIndex];

        for (int i = 0; i < ChildAnswer.Length; i++)
        {
            try
            {
        
                r[i] = ChildAnswer[i].GetComponent<TextMesh>().color.r;
                g[i] = ChildAnswer[i].GetComponent<TextMesh>().color.g;
                b[i] = ChildAnswer[i].GetComponent<TextMesh>().color.b;
            }
            catch
            {
                r[i] = ChildAnswer[i].GetComponent<SpriteRenderer>().color.r;
                g[i] = ChildAnswer[i].GetComponent<SpriteRenderer>().color.g;
                b[i] = ChildAnswer[i].GetComponent<SpriteRenderer>().color.b;
            }
        }
        
    }
    private void OnMouseDown()
    {
        if (IsWaitUntilClosing)
        {
            if (!startAnim && currentIndex != 0)
            {
                currentIndex = 0;
                startAnim = true;
            }
        }
        else
        {
            currentIndex = 0;
            startAnim = true;
        }

    }
    private bool startAnim = false;
    private Touch touch;
    private void Update()
    {
        if(Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            RaycastHit2D hitTouch = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(touch.deltaPosition).x, Camera.main.ScreenToWorldPoint(touch.deltaPosition).y), Vector2.zero, 0);
            if (hitTouch && hitTouch.collider.transform.name == transform.name)
            {
                if (IsWaitUntilClosing)
                {
                    if (!startAnim && currentIndex != 0)
                    {
                        currentIndex = 0;
                        startAnim = true;
                    }
                }
                else
                {
                    currentIndex = 0;
                    startAnim = true;
                }
            }
        }

        if(currentIndex > -1 && startAnim)
        {
            countIndex();

            if (currentIndex >= ChildAnswer.Length)
            {
                currentIndex = -1;
                startAnim = false;
                isFirstTime = true;
            }
        }
    }

    private void countIndex()
    {
        //if (!isFirstTime && rangeOfTransition != 0)
        //    yield return new WaitForSeconds(rangeOfTransition);
        //else
        //{
        //    yield return new WaitForSeconds(0);
        //}

        if (currentIndex > -1 && startAnim && isItStart)
        {
            makeShowData();
            currentIndex++;
        }
    }

    private int currentIndex = -1;
    private void makeShowData()
    {
        if (makeShow && isItStart)
        {
            isItStart = false;
            StartCoroutine(makeAlphaShow(currentIndex));
        }
    }
    float alpha = 0.0f;
    private IEnumerator makeAlphaShow(int index)
    {
        if (!isFirstTime && rangeOfTransition != 0)
            yield return new WaitForSeconds(rangeOfTransition);
        else if(rangeOfTransition != 0)
        {
            yield return new WaitForSeconds(0);
            isFirstTime = false;
        }

        try
        {
            alpha = ChildAnswer[index].GetComponent<TextMesh>().color.a;
        }
        catch
        {
            alpha = ChildAnswer[index].GetComponent<SpriteRenderer>().color.a;
        }
        if (alpha == 1.0f)
        {
            currentIndex = -1;
            startAnim = false;
            isFirstTime = true;
            yield return 0;
            StopCoroutine(makeAlphaShow(currentIndex));
        }

        StartCoroutine(makeAlphaHideAndShow(1.0f, 1.0f, index));
        isItStart = true;
        if (Parent != null)
        {
            Parent.GetComponent<WasUnitComplete>().SetCountOfDifference++;
        }
        if (visibleTime != 0)
        {
            yield return new WaitForSeconds(visibleTime);
            yield return StartCoroutine(makeAlphaHideAndShow(0.0f, 1.0f, index));
        }
    }

    private IEnumerator makeAlphaHideAndShow(float aValue, float aTime, int index)
    {
        float alpha = 0.0f;
        try
        {
            alpha = ChildAnswer[index].GetComponent<TextMesh>().color.a;
        }
        catch
        {
            alpha = ChildAnswer[index].GetComponent<SpriteRenderer>().color.a;
        }
        
        for (float i = 0.0f; i < 1.0f; i += Time.deltaTime / aTime)
        {
            Color newColor = new Color(r[index], g[index], b[index], Mathf.Lerp(alpha, aValue, i));
            try
            {
                ChildAnswer[index].GetComponent<TextMesh>().color = newColor;
            }
            catch
            {
                ChildAnswer[index].GetComponent<SpriteRenderer>().color = newColor;
            }
            yield return null;
        }
    }

}
