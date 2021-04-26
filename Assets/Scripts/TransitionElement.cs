//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TransitionElement : MonoBehaviour
//{

//    [HideInInspector] public float startAtSeconds = 0f;
//    [HideInInspector] public bool isStartFromThisObj = false;
//    /*[HideInInspector]*/ public TransitionElement lastObject = null;
//    [HideInInspector] public float timeOfRange = 0.5f;
//    [HideInInspector] public string[] TypeOfTransition = new string[] { "Lerp", "Move Toward", "Scale" };
//    [HideInInspector] public int IndexOfTypes = 0;
//    [HideInInspector] public Vector3 ToThatLocation;
//    [HideInInspector] public float ScaleRadius = 0.1f;

//    public delegate void StartTransitionBoolChanged();
//    public event StartTransitionBoolChanged ToNextObject;

//    [HideInInspector] public bool isGoNextObject;
//    public bool IsGoNextObject
//    {
//        get
//        {
//            return isGoNextObject;
//        }
//        set
//        {
//            isGoNextObject = value;
//            if (ToNextObject != null)
//            {
//                ToNextObject();
//            }
//        }
//    }

//    private bool isStartTransition;
//    public bool IsStartTransition
//    {
//        get { return isStartTransition; }
//        set
//        {
//            if (value == isStartTransition)
//                return;

//            isStartTransition = value;
//            if (isStartTransition)
//            {
//                StartAnim();
//            }
//        }
//    }

//    private void StartAnim()
//    {
//        StartCoroutine(startNextObjAnim());
//    }

//    private IEnumerator startNextObjAnim()
//    {
//        yield return new WaitForSeconds(timeOfRange);
//        isGoNextObject = true;
//    }

//    private void Start()
//    {
//        IsStartTransition = false;

//        if (isStartFromThisObj)
//            StartCoroutine(startAt());
//        else
//            lastObject.ToNextObject += LastObject_ToNextObject;
//    }

//    private void LastObject_ToNextObject()
//    {
//        IsStartTransition = true;
//    }

//    private void FixedUpdate()
//    {

//    }

//    private IEnumerator startAt()
//    {
//        yield return new WaitForSeconds(startAtSeconds);
//        IsStartTransition = true;
//    }
//}
