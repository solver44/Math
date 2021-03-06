using System.Collections;
using UnityEngine;

public class ShowChildAlpha : MonoBehaviour
{
    [SerializeField] private bool makeShow = true;
    [SerializeField] private bool getChildrenFromParent = false;
    public GameObject[] ChildAnswer = null;
    [SerializeField] private float visibleTime = 2f;

    [Header("Additional")]
    [SerializeField] private float rangeOfTransition = 0.5f;
    [SerializeField] private int sizeOfIndex = 18;
    public GameObject Parent = null;
    bool IsWaitUntilClosing = true;
    public bool WhenClickThenClose = false;

    private bool isItStart = false;
    private bool isFirstTime = true;

    private float[] r, g, b;

    private GameObject[] ChildAnswerF = null;

    private GameObject trail = null;
    private int getCountOfChildren()
    {
        int result = 0;
        for (int i = 0; i < ChildAnswer.Length; i++)
        {
            result += ChildAnswer[i].transform.childCount;
        }
        return result;
    }
    float yUpper, yLower, xStart, xEnd;
    private Object LoadPrefabFromFile(string filename)
    {
        var loadedObject = Resources.Load("Prefabs/" + filename);
        if (loadedObject == null)
        {
            throw new System.Exception("...no file found - please check the configuration");
        }
        return loadedObject;
    }
    private void Awake()
    {

            //try
            //{
                //var prefab = LoadPrefabFromFile("trail");
                //trail = (GameObject)Instantiate(prefab, transform);
            //}
            //catch { }
            //collider1 = GetComponent<BoxCollider2D>();

            //var yHalfExtents = collider1.bounds.extents.y;
            //var yCenter = collider1.bounds.center.y;

            //yUpper = yCenter + yHalfExtents;
            //yLower = yCenter - yHalfExtents;

            //var xHalfExtents = collider1.bounds.extents.x;
            //var xCenter = collider1.bounds.center.x;

            //xStart = xCenter - xHalfExtents;
            //xEnd = xCenter + xHalfExtents;

            //trail.transform.position = new Vector3(xStart, yUpper, trail.transform.position.z);
        
    }
    private void Start()
    {
        if (!getChildrenFromParent)
            ChildAnswerF = ChildAnswer;
        else
        {
            int cnt = 0;
            ChildAnswerF = new GameObject[getCountOfChildren()];
            
            for (int i = 0; i < ChildAnswer.Length; i++)
            {
                for (int j=0; j<ChildAnswer[i].transform.childCount; j++)
                {
                    ChildAnswerF[cnt] = ChildAnswer[i].transform.GetChild(j).transform.gameObject;
                    cnt++;
                }
            }
        }
        sizeOfIndex = ChildAnswerF.Length;
        IsWaitUntilClosing = true;
        r = new float[sizeOfIndex];
        g = new float[sizeOfIndex];
        b = new float[sizeOfIndex];

        for (int i = 0; i < ChildAnswerF.Length; i++)
        {
            try
            {
                if (ChildAnswerF[i].transform.name == "RectangleAnswer")
                { WhenClickThenClose = true; rangeOfTransition = 0.1f; }
            }
            catch { }
            try
            {
                r[i] = ChildAnswerF[i].GetComponent<TextMesh>().color.r;
                g[i] = ChildAnswerF[i].GetComponent<TextMesh>().color.g;
                b[i] = ChildAnswerF[i].GetComponent<TextMesh>().color.b;
            }
            catch
            {
                r[i] = ChildAnswerF[i].GetComponent<SpriteRenderer>().color.r;
                g[i] = ChildAnswerF[i].GetComponent<SpriteRenderer>().color.g;
                b[i] = ChildAnswerF[i].GetComponent<SpriteRenderer>().color.b;
            }
        }
        
    }
    //private void OnMouseDown()
    //{
    //    //if (IsWaitUntilClosing)
    //    //{
    //        //if (!startAnim && currentIndex != 0 && !WhenClickThenClose)
    //        //{
    //            //currentIndex = 0;
    //            //startAnim = true;
    //        //}
    //        //if(WhenClickThenClose && startAnim)
    //        //{

    //        //}
    //    //}
    //    //else
    //    //{
    //        //currentIndex = 0;
    //        //startAnim = true;
    //    //}

    //}
    private bool startAnim = false;
    private Touch touch;

    void SetRayCast(RaycastHit2D hitTouch)
    {
        if (hitTouch)
        {
            if (hitTouch.collider.transform == transform) {

                if (IsWaitUntilClosing)
                { if (!startAnim && currentIndex != 0)
                    {
                        currentIndex = 0;
                        startAnim = true;
                }}else{
                        currentIndex = 0;
                        startAnim = true;
                }

                if (WhenClickThenClose && isItStart)
                {
                    for (int i = 0; i < ChildAnswerF.Length; i++)
                    {
                        StartCoroutine(makeAlphaHideAndShow(0.0f, 1.0f, i, false));
                    }
                    currentIndex = -1;
                    startAnim = false;
                    isFirstTime = true;
                    isItStart = false;
                }
            }
            
        }
    }
    BoxCollider2D collider1;
    float interpolate;
    int curDot = 1;
    private void Update()
    {
 
            //interpolate = Time.fixedDeltaTime * 1f;
            //switch (curDot)
            //{
            //    case 1:
            //        trail.transform.position = Vector3.MoveTowards(trail.transform.position, new Vector3(xEnd, yUpper), interpolate);
            //        if (Vector3.Distance(trail.transform.position, new Vector3(xEnd, yUpper)) <= 0)
            //            curDot = 2;
            //        break;
            //    case 2:
            //        trail.transform.position = Vector3.MoveTowards(trail.transform.position, new Vector3(xEnd, yLower), interpolate);
            //        if (Vector3.Distance(trail.transform.position, new Vector3(xEnd, yLower)) <= 0)
            //            curDot = 3;
            //        break;
            //    case 3:
            //        trail.transform.position = Vector3.MoveTowards(trail.transform.position, new Vector3(xStart, yLower), interpolate);
            //        if (Vector3.Distance(trail.transform.position, new Vector3(xStart, yLower)) <= 0)
            //            curDot =4;
            //        break;
            //    case 4:
            //        trail.transform.position = Vector3.MoveTowards(trail.transform.position, new Vector3(xStart, yUpper), interpolate);
            //        if (Vector3.Distance(trail.transform.position, new Vector3(xStart, yUpper)) <= 0)
            //            curDot = 1;
            //        break;
            //}
        

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            RaycastHit2D hitTouch = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(touch.deltaPosition).x, Camera.main.ScreenToWorldPoint(touch.deltaPosition).y), Vector2.zero, 0);
            SetRayCast(hitTouch);
        }
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hitTouch = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0);
            SetRayCast(hitTouch);
        }

        if (startAnim && currentIndex > -1)
        {
            countIndex();

            if (currentIndex >= ChildAnswerF.Length)
            {
                currentIndex = -1;
                startAnim = false;
                isItStart = false;
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

        if (currentIndex > -1 && startAnim && isStartIt)
        {
            makeShowData();
            currentIndex++;
        }
    }

    private bool isStartIt = true;
    private int currentIndex = -1;
    private void makeShowData()
    {
        if (makeShow && isStartIt)
        {
            isStartIt = false;
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
            alpha = ChildAnswerF[index].GetComponent<TextMesh>().color.a;
        }
        catch
        {
            alpha = ChildAnswerF[index].GetComponent<SpriteRenderer>().color.a;
        }

        ChildAnswerF[index].gameObject.GetComponent<Collider2D>().enabled = false;

        StartCoroutine(makeAlphaHideAndShow(1.0f, 1.0f, index, false));
        isItStart = true;
        isStartIt = true;
        if (Parent != null)
        {
            Parent.GetComponent<WasUnitComplete>().SetCountOfDifference++;
        }
        if (visibleTime != 0 && !WhenClickThenClose)
        {
            yield return new WaitForSeconds(visibleTime);
            yield return StartCoroutine(makeAlphaHideAndShow(0.0f, 1.0f, index, false));
            isItStart = false;
        }
    }

    private IEnumerator makeAlphaHideAndShow(float aValue, float aTime, int index, bool isMakeFalse)
    {
        float alpha = 0.0f;
        try
        {
            alpha = ChildAnswerF[index].GetComponent<TextMesh>().color.a;
        }
        catch
        {
            alpha = ChildAnswerF[index].GetComponent<SpriteRenderer>().color.a;
        }
        
        for (float i = 0.0f; i < 1.0f; i += Time.deltaTime / aTime)
        {
            Color newColor = new Color(r[index], g[index], b[index], Mathf.Lerp(alpha, aValue, i));
            try
            {
                ChildAnswerF[index].GetComponent<TextMesh>().color = newColor;
            }
            catch
            {
                ChildAnswerF[index].GetComponent<SpriteRenderer>().color = newColor;
            }
            yield return null;
        }
        if(isMakeFalse)
            isItStart = false;
    }

}
