using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawLines : MonoBehaviour
{
    //Public Variables
    public bool SpecialPositions = true; 
    public bool StartFromFirstObject = true;
    public bool DontReturn = false;
    public GameObject[] Objects;
    public Material material;
    public float Range = .3f;
    public int CurrentUnit = 1;
    public WasUnitComplete Parent = null;
    [Header("Effects")]
    public bool ScaleEffect = false;
    public float ScaleR = 0.2f;

    //Private Variables
    private int count = 0;
    private GameObject[,] childObjects;
    private LineRenderer line;
    private Vector3 mousePos;
    private int currLines = 0;

    int tempCount = 0;

    ScaleEffect effect = new ScaleEffect();
    void Start()
    {
        effect.Scaled += Effect_Scaled;
        count = Objects.Length;
        bool tempBool = false;
        //Найти максимальную числу из родителей
        for (int i = 0; i < Objects.Length; i++)
        {
            tempBool = false;
            tempCount = Objects[i].transform.childCount;
            for (int k = 0; k < Objects.Length; k++)
            {
                if (Objects[k].transform.childCount > tempCount)
                    tempBool = true;
            }
            if (!tempBool)
                break;
        }

        childObjects = new GameObject[Objects.Length, tempCount];
        for (int i = 0; i < Objects.Length; i++)
        {
            for (int k = 0; k < Objects[i].transform.childCount; k++)
            {
                childObjects[i,k] = Objects[i].transform.GetChild(k).gameObject;
            }
        }

        if(CurrentUnit > 1)
            WasUnitComplete.Finishing += WasUnitComplete_Finishing;
        else
            StartCoroutine(setPos());
    }

    private void Effect_Scaled()
    {
        childPos = childObjects[_currentParentIndex, _currentChildIndex].transform.position;
    }

    private void WasUnitComplete_Finishing(int unit)
    {
        if (unit == CurrentUnit)
            StartCoroutine(setPos());
    }
    private IEnumerator setPos()
    {
        yield return new WaitForSeconds(3);
        currentParentIndex = 0;
    }

    private int _currentParentIndex = 0, _currentChildIndex = 0;
    Vector2 childPos;

    Vector2 target;
    Vector2 tempTransformPos;
    private int currentParentIndex
    {
        get { return _currentParentIndex; }
        set { _currentParentIndex = value; _currentChildIndex = 0; if (_currentParentIndex >= count) { _currentParentIndex = count - 1; return; }
            if (ScaleEffect)
            {
                tempTransformPos = Objects[_currentParentIndex].transform.localScale;
                target = new Vector2(tempTransformPos.x + (tempTransformPos.x * ScaleR), tempTransformPos.y + (tempTransformPos.y * ScaleR));
                StartCoroutine(effect.Scale(Objects[_currentParentIndex].transform, target, 2f));
            }
            else {
                childPos = childObjects[_currentParentIndex, _currentChildIndex].transform.position; }
        }
    }
    private int currentChildIndex
    {
        get { return _currentChildIndex; }
        set { _currentChildIndex = value;
            if (_currentChildIndex < 0)
                _currentChildIndex += 2;
            else if (_currentChildIndex >= tempCount || childObjects[_currentParentIndex, _currentChildIndex] == null)
                _currentChildIndex = 0;
            childPos = childObjects[_currentParentIndex, _currentChildIndex].transform.position; }
    }

    static int completedTasksCount = 0;
    private int _completedTasksCount
    {
        get { return completedTasksCount; }
        set { completedTasksCount = value; if (completedTasksCount == count) Parent.CompleteUnit(); }
    }
    Touch touch;
    RaycastHit2D hit;

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            downFunc();
        }
        else if (Input.GetMouseButtonUp(0) && line)
        {
            dragFunc();
        }
        else if (Input.GetMouseButton(0) && line)
        {
            upFunc();
        }
    }
#endif


#if UNITY_EDITOR
    private void downFunc()
    {
        if (Vector2.Distance(mousePos, childPos) > Range)
            return;

        if (line == null)
        {
            createLine();
        }

        line.SetPosition(0, childPos);
        line.SetPosition(1, childPos);

        currentChildIndex++;
    }
    private void dragFunc()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = -1;

        if (Vector2.Distance(mousePos, childPos) > Range)
        {
            currentChildIndex--;
            Debug.Log("Destroy");
            Destroy(line.gameObject);
            line = null;
            return;
        }

        line.SetPosition(1, childPos);
        line = null;
        currLines++;

        if ((currLines > 0 && _currentChildIndex == 0) || (_currentChildIndex + 1 >= tempCount && DontReturn))
        {
            _completedTasksCount++;
            currentParentIndex++;
        }

    }
    private void upFunc()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = -1;

        line.SetPosition(1, mousePos);
    }
#else
    private void OnMouseDown()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Vector2.Distance(mousePos, childPos) > Range)
            return;

        if (line == null)
        {
            createLine();
        }

        line.SetPosition(0, childPos);
        line.SetPosition(1, childPos);

        currentChildIndex++;
    }
    private void OnMouseDrag()
    {
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));
        mousePos.z = 0;

        if (Vector2.Distance(mousePos, childPos) > Range)
        {
            currentChildIndex--;
            Destroy(line.gameObject);
            line = null;
            return;
        }

        line.SetPosition(1, childPos);
        line = null;
        currLines++;

        if ((currLines > 0 && _currentChildIndex == 0) || (_currentChildIndex + 1 >= tempCount && DontReturn))
        {
            _completedTasksCount++;
            currentParentIndex++;
        }

    }
    private void OnMouseUp()
    {
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));
        mousePos.z = -1;

        line.SetPosition(1, mousePos);
    }
#endif


    void createLine()
    {
        line = new GameObject("Line" + currLines).AddComponent<LineRenderer>();
        line.transform.parent = this.transform;
        line.transform.position = new Vector3(line.transform.position.x, line.transform.position.y, -1);
        line.material = material;
        line.positionCount = 2;
        line.startWidth = 0.15f;
        line.endWidth = 0.15f;
        line.useWorldSpace = false;
        line.numCapVertices = 50;
    }
}
