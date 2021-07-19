using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SameObjectsFinder : MonoBehaviour
{
    public WasUnitComplete Parent = null;
    public bool ScaleEffect = false;
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

    ScaleEffect effects = new ScaleEffect();
    void SetRayCast(RaycastHit2D hitTouch)
    {
        if (hitTouch && hitTouch.collider.transform.parent.gameObject.Equals(this.gameObject))
        {
            GameObject currentObj = hitTouch.collider.gameObject;
            if ((!ScaleEffect && currentObj.transform.GetChild(0).gameObject.activeSelf) || (ScaleEffect && currentObj.GetComponent<SpriteRenderer>().sortingLayerName == "Selected"))
                return;

            if (previousName == null)
                previousName = currentObj.name;
            if(currentObj.name != previousName)
            {
                currentCount = 0;

                if (previousChild.Count > 0)
                {
                    for (int i = 0; i < previousChild.Count; i++)
                    {
                        if (!ScaleEffect)
                            previousChild[i].SetActive(false);
                        else {
                            Vector2 tempTarget = new Vector2(previousChild[i].transform.localScale.x - (previousChild[i].transform.localScale.x * .1f), previousChild[i].transform.localScale.y - (previousChild[i].transform.localScale.y * .1f));
                            StartCoroutine(effects.Scale(previousChild[i].transform, tempTarget, 3f));
                            previousChild[i].GetComponent<SpriteRenderer>().sortingLayerName = "Top";
                        }
                    }
                }
                previousChild.RemoveRange(0, previousChild.Count);

            }
            if (!ScaleEffect)
                currentObj.transform.GetChild(0).gameObject.SetActive(true);
            else
            {
                Vector2 tempTarget = new Vector2(currentObj.transform.localScale.x + (currentObj.transform.localScale.x * .1f), currentObj.transform.localScale.y + (currentObj.transform.localScale.y * .1f));
                StartCoroutine(effects.Scale(currentObj.transform, tempTarget, 3f));
                currentObj.GetComponent<SpriteRenderer>().sortingLayerName = "Selected";
            }
            currentCount++;

            previousName = currentObj.name;
            if(!ScaleEffect)
                previousChild.Add(currentObj.transform.GetChild(0).gameObject);
            else
                previousChild.Add(currentObj.gameObject);
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
                    if(!ScaleEffect)
                        StartCoroutine(methodScale(previousChild[i].transform.parent, new Vector2(0, 0), true));
                    else
                        StartCoroutine(methodScale(previousChild[i].transform, new Vector2(0, 0), true));

                }
            }
            previousChild.RemoveRange(0, previousChild.Count);
            previousName = null;
            currentSameObject++;
        }
    }

    private IEnumerator methodScale(Transform parent, Vector2 toScale, bool moreEffect)
    {
        if (moreEffect) {
            Vector2 target = new Vector2(parent.localScale.x + (parent.localScale.x * .2f), parent.localScale.y + (parent.localScale.y * .2f));
            for (float i = 0; i <= 1; i += Time.deltaTime / 3f)
            {
                parent.localScale = Vector2.MoveTowards(parent.localScale, target, i);
                if (parent.localScale.Equals(target))
                    break;
                yield return new WaitForEndOfFrame();
            }
        }

        for (float i = 0; i <= 1; i += Time.deltaTime / 3f)
        {
            parent.localScale = Vector2.LerpUnclamped(parent.localScale, toScale, i);
            if (parent.localScale.Equals(toScale))
                yield break;
            yield return new WaitForEndOfFrame();
        }
    }
}
