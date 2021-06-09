using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleEffect : MonoBehaviour
{
    public float scaleR = 0.2f;

    private bool _lock = false;
    public bool Lock{ get { return Lock; } set { Lock = value; } }
    Vector3 targetScale = new Vector3();

    private bool back = false;

    void Start()
    {
        targetScale = new Vector3((this.transform.localScale.x * scaleR) + this.transform.localScale.x, (this.transform.localScale.y * scaleR) + this.transform.localScale.y, this.transform.localScale.z);
        Debug.Log((this.transform.localScale.x * scaleR) + this.transform.localScale.x);
    }
    int interpolationFramesCount = 150;
    int elapsedFrames = 1;
    Vector3 temp;

    bool wait = true;
    void Update()
    {
        if (_lock)
            return;    

        //this.transform.localScale = Vector3.MoveTowards(this.transform.localScale, targetScale, Time.deltaTime * 5f);

        if (!back && !wait)
        {
            wait = true;
            targetScale = new Vector3((this.transform.localScale.x * scaleR) + this.transform.localScale.x, (this.transform.localScale.y * scaleR) + this.transform.localScale.y, this.transform.localScale.z);
        }
        else if (!wait)
        {
            wait = true;
            targetScale = new Vector3(this.transform.localScale.x - (this.transform.localScale.x * scaleR), this.transform.localScale.y - (this.transform.localScale.y * scaleR), this.transform.localScale.z);
        }
        StartCoroutine(Scale());
    }

    IEnumerator Scale()
    {
        float scaleDuration = 10f;
        for (float t = 0; t < 1; t += Time.deltaTime / scaleDuration)
        {
            this.transform.localScale = Vector3.MoveTowards(this.transform.localScale, targetScale, t);
            if (this.transform.localScale.Equals(targetScale))
            {
                //transform.localScale = toThisScale;
                IEnumerator co = Scale();
                wait = false;
                back = !back;
                StopCoroutine(co);
                yield break;
            }
            yield return null;
        }
    }
}
