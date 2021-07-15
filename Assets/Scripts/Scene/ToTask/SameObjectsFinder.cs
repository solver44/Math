using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SameObjectsFinder : MonoBehaviour
{
    public WasUnitComplete Parent = null;
    public int Cnt = 0;
    private int CountOfSameObjects = 0;
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
            GameObject currentObj = hitTouch.collider.gameObject;
            if (currentObj.transform.GetChild(0).gameObject.activeSelf)
                return;

            if (previousName == null)
                previousName = currentObj.name;
            if(currentObj.name != previousName)
            {
                if(previousChild.Count > 0)
                {
                    for (int i = 0; i < previousChild.Count; i++)
                    {
                        previousChild[i].SetActive(false);
                    }
                }
                currentObj.transform.GetChild(0).gameObject.SetActive(true);
                currentCount = 0;
                previousChild.RemoveRange(0, previousChild.Count);
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
            previousChild.RemoveRange(0, previousChild.Count);
            currentCount = 0;
            previousName = null;
            currentSameObject++;
            Debug.Log(currentSameObject);
        }
    }
}
