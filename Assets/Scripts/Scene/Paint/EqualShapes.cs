using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqualShapes : MonoBehaviour
{
    public GameObject TargetShape = null;
    public bool OnTop = false;
    [HideInInspector] public string currentNameColor {
        set { _currentNameColor = value; nameOfShape = _currentNameColor;
            if (TargetShape.GetComponent<EqualShapes>().currentNameColor != value)
            {
                TargetShape.GetComponent<EqualShapes>().currentNameColor = value;
            }
            TargetShape.GetComponent<EqualShapes>().haveParent = true;  }
        get { return _currentNameColor; }
    }
    private string _currentNameColor;

    private string currN = "";
    private string nameOfShape = "";
    private PaintInfo info => this.GetComponentInParent<PaintInfo>();

    bool isOther = false;
    void Start()
    {
        if (TargetShape != null)
        {
            if (TargetShape.name.Contains("color"))
                nameOfShape = TargetShape.transform.name;
            else
                isOther = true;
        }
    }

    [HideInInspector] public bool haveParent { set { _haveParent = value; if(!string.IsNullOrEmpty(currN) && currN == _currentNameColor) CheckIt(currN, false, false); } get { return _haveParent; } }
    private bool _haveParent = false;
    public bool CheckIt(string name, bool sumCount, bool dontCheck)
    {
        if(dontCheck)
        {
            info.Count++;
            currN = name;
            return true;
        }

        if (TargetShape == null && !isOther)
            nameOfShape = name;

        if (isOther && !haveParent)
            currentNameColor = name;

        if (nameOfShape.Equals(name))
        {
            if (sumCount || (haveParent && currN != name))
                info.Count++;

            Debug.Log(info.Count);
            currN = name;
            return true;
        }
        else
        {
            if (currN != name && currN != "" && isOther) {
                if (haveParent && info.Count > 0)
                    info.Count--;

                _haveParent = false;

                currentNameColor = name;
                currN = name;
                return true;
            }
            if(isOther)
                currentNameColor = name;
            currN = name;
            return false;
        }
    }
}
