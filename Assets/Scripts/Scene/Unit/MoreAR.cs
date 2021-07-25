using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MoreAR : MonoBehaviour
{
    public bool More = false;
    [HideInInspector] public float Speed;
    public GameObject[] ObjectsToExit = null;
    public GameObject[] ObjectsToEnter = null;

    public bool WaitAndComplete = false;

    public bool Finish = false;

    //Range Move
    public float RangeX = 0f;
    public float RangeY = 10f;

    //Time
    public float WaitSecondsEnter = 1f;
    public float WaitSecondsExit = 1f;
    private IEnumerator waitEnter()
    {
        yield return new WaitForSeconds(WaitSecondsEnter + 2f);
        move = true;
        enter = true;
    }
    private IEnumerator waitExit()
    {
        yield return new WaitForSeconds(WaitSecondsExit);
        move = true;
        enter = false;
    }

    private Vector3[] firstLocation = null;

    private void Start()
    {
        RectTransform rect = null;
        firstLocation = new Vector3[ObjectsToEnter.Length];
        for (int i = 0; i < ObjectsToEnter.Length; i++)
        {
            firstLocation[i] = ObjectsToEnter[i].transform.localPosition;
            Vector2 target = new Vector2(firstLocation[i].x + RangeX, firstLocation[i].y + RangeY);

            if(ObjectsToEnter[i].transform.TryGetComponent<RectTransform>(out rect))
            {
                target = new Vector2(firstLocation[i].x + RangeX * 100, firstLocation[i].y + RangeY * 100);
            }

            ObjectsToEnter[i].transform.localPosition = target;
        }
    }
    //Exit
    public void Exit()
    {
        if (ObjectsToExit.Length > 0)
            StartCoroutine(waitExit());
        else
            WaitSecondsEnter--;
    }
    private void exitMove()
    {
        if (stop)
            return;

        RectTransform temp = null;
        for (int i = 0; i < ObjectsToExit.Length; i++)
        {
            Vector2 target = Vector2.zero;
            if(!ObjectsToExit[i].transform.TryGetComponent<RectTransform>(out temp))
                target = new Vector2(firstLocation[i].x + RangeX, firstLocation[i].y + RangeY);
            else
                target = new Vector2(firstLocation[i].x + RangeX * 150, firstLocation[i].y + RangeY * 150);

            ObjectsToExit[i].transform.localPosition = Vector2.Lerp(ObjectsToExit[i].transform.localPosition, target, Speed * Time.deltaTime);
        }
    }
    //End Exit


    bool move = false, enter = true, stop = false;


    //Enter
    public void Enter()
    {
        StartCoroutine(waitEnter());
    }
    private void enterMove()
    {
        for (int i = 0; i < ObjectsToEnter.Length; i++)
        {
            Vector2 target = new Vector2(firstLocation[i].x, firstLocation[i].y);
            if (Vector2.Distance(ObjectsToEnter[i].transform.localPosition, target) >= .01f && !stop)
            {
                ObjectsToEnter[i].transform.localPosition = Vector2.Lerp(ObjectsToEnter[i].transform.localPosition, target, Speed * Time.deltaTime);
            }
            else
            {
                ObjectsToEnter[i].transform.localPosition = target;
                move = false;
                stop = true;
                Finish = true;
                if (WaitAndComplete)
                    StartCoroutine(WaitAndCompleteF());
            }
        }
    }
    //End Enter

    private IEnumerator WaitAndCompleteF()
    {
        yield return new WaitForSeconds(WaitSecondsExit);
        Finish = true;
        this.GetComponent<WasUnitComplete>().CompleteUnit();
    }
    private void Update()
    {
        if (!move)
            return;

        if (enter)
            enterMove();
        else
            exitMove();
    }
}
