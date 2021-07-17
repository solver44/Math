using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SameObjectsFinder : MonoBehaviour
{
    public WasUnitComplete Parent = null;
    public int Cnt = 0;
    public bool KillWithEffect = false;

    private int CountOfSameObjects = 1;
    private void Start()
    {
        List<string> names = new List<string>();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            names.Add(this.transform.GetChild(i).name);
        }
        for (int i = 1; i < names.Count; i++)
        {
            if (names[0] == names[i])
                CountOfSameObjects++;
        }
    }

    string previousName = null;
    int currentCount = 0;
    List<GameObject> previousChild = new List<GameObject>();

    private int _currentSameObject = 0;
    private int currentSameObject
    {
        get { return _currentSameObject; }
        set { _currentSameObject = value; if (_currentSameObject == Cnt) {
                if (Parent != null)
                    Parent.CompleteUnit();
            } }
    }
    void SetRayCast(RaycastHit2D hitTouch)
    {
        if (hitTouch && hitTouch.collider.transform.parent.gameObject.Equals(this.gameObject))
        {
            Debug.Log(currentCount);
            GameObject currentObj = hitTouch.collider.gameObject;
            if (currentObj.transform.GetChild(0).gameObject.activeSelf)
                return;

            if (previousName == null)
                previousName = currentObj.name;
            if(currentObj.name != previousName)
            {
                currentCount = 0;
                if(previousChild.Count > 0)
                {
                    for (int i = 0; i < previousChild.Count; i++)
                    {
                        previousChild[i].SetActive(false);
                    }
                }
                previousChild.RemoveRange(0, previousChild.Count);
                currentObj.transform.GetChild(0).gameObject.SetActive(true);
                currentCount++;
            }
            else
            {
                currentObj.transform.GetChild(0).gameObject.SetActive(true);
                currentCount++;
            }

            previousName = currentObj.name;
            previousChild.Add(currentObj.transform.GetChild(0).gameObject);
        }
    }

    Touch touch;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hitTouch = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0);

            SetRayCast(hitTouch);
        }

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            RaycastHit2D hitTouch = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(touch.deltaPosition).x, Camera.main.ScreenToWorldPoint(touch.deltaPosition).y), Vector2.zero, 0);

            SetRayCast(hitTouch);
        }

        if (CountOfSameObjects == currentCount)
        {
            currentCount = 0;
            if(KillWithEffect)
            {
                for (int i = 0; i < previousChild.Count; i++)
                {
                    //Destroy(previousChild[i].transform.parent.gameObject, 0.5f);
                    for (float x = 0; x < 1; x--)
                    {
                        previousChild[i].transform.localScale = new Vector3(previousChild[i].transform.localScale.x, x);
                    }
                }
            }
            previousChild.RemoveRange(0, previousChild.Count);
            previousName = null;
            currentSameObject++;
            Debug.Log(currentSameObject);
        }
    }
}
