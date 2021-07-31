using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleEffect : MonoBehaviour
{
    #region Events
    public delegate void Effects();
    public event Effects Scaled;
    #endregion
    public float scaleR = 0.2f;

    private bool _lock = false;
    public bool Lock{ get { return Lock; } set { Lock = value; } }
    Vector3 targetScale = new Vector3();

    private bool back = false;

    void Start()
    {
        targetScale = new Vector3((this.transform.localScale.x * scaleR) + this.transform.localScale.x, (this.transform.localScale.y * scaleR) + this.transform.localScale.y, this.transform.localScale.z);

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
        StartCoroutine(Scale(this.transform, targetScale, 10f));
    }

    public IEnumerator Scale(Transform transF, Vector2 target, float duration)
    {
        float scaleDuration = duration;
        for (float t = 0; t < 1; t += Time.deltaTime / scaleDuration)
        {
            transF.localScale = Vector3.MoveTowards(transF.localScale, target, t);
            if (transF.localScale.Equals(target))
            {
                //transform.localScale = toThisScale;
                wait = false;
                back = !back;
                Scaled?.Invoke();
                yield break;
            }
            yield return null;
        }
    }

    public IEnumerator Scale2(Transform transF, Vector2 target, float duration)
    {
        float scaleDuration = duration;
        for (float t = 0; t < 1; t += Time.deltaTime / scaleDuration)
        {
            transF.localScale = Vector3.Lerp(transF.localScale, target, t);
            if (transF.localScale.Equals(target))
            {
                //transform.localScale = toThisScale;
                wait = false;
                back = !back;
                yield break;
            }
            yield return null;
        }
    }

    public IEnumerator MoveAnim(Transform parent, Vector2 target, bool isLocal, float duration)
    {
        float scaleDuration = duration;
        for (float t = 0; t < 1; t += Time.deltaTime / scaleDuration)
        {
            if (isLocal)
                parent.localPosition = Vector3.Lerp(parent.localPosition, target, t);
            else
                parent.position = Vector3.Lerp(parent.position, target, t);

            if ((isLocal && parent.localPosition.Equals(target)) || (!isLocal && parent.position.Equals(target)))
            {
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
