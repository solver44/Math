using System.Collections;
using UnityEngine;

public class MoveToLocation : MonoBehaviour
{
    [SerializeField] private float speedMove = 10; //Default 10
    [SerializeField] private float locationX = 0;
    [SerializeField] private float locationY = 1;
    public bool freezeY = false;
    [SerializeField] private bool isMoveToward = false;
    [SerializeField] private float waitForSecond = 0f;
    [SerializeField] private bool isTextShowing = false;


    private GameObject textPanel = null;

    private bool _gottaFinish = false;
    private bool gottaFinish
    {
        get { return _gottaFinish; }
        set { _gottaFinish = value; if (gottaFinish && _stop) FinishMove = true;}
    }
    public void SetLocationX(float x, bool finish) { locationX = x; stop = false; gottaFinish = finish; try { if (textPanel != null && !isTextShowing) { textPanel.SetActive(false); } else { textPanel.SetActive(true); } }catch{} }
    public void SetLocationY(float y, bool finish) { if (freezeY) locationY = this.transform.localPosition.y; else locationY = y; stop = false; gottaFinish = finish; try { if (textPanel != null && !isTextShowing) { textPanel.SetActive(false); } else { textPanel.SetActive(true); } } catch { } }

    private bool _stop = true;
    public bool stop
    {
        get { return _stop; }
        set { _stop = value; if (gottaFinish && _stop) FinishMove = true; }
    }

    public bool FinishMove = false;

    [Header("Additional")]
    public bool isWaitFinishAnother = false;
    public MoveToLocation[] OtherObjects = null;

    private void Start()
    {
        if (isTextShowing)
        {
            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
            {
                if (go.transform.name == "Panel")
                    textPanel = go.transform.gameObject;
            }
        }

        try { if (textPanel != null && !isTextShowing) { textPanel.SetActive(false); } else { textPanel.SetActive(true); } } catch { }
        StartCoroutine(waitForSec());

        if (freezeY)
            locationY = this.transform.localPosition.y;
    }

    Vector2 target = new Vector2();
    void Update()
    {
        if (stop)
            return;

        target = new Vector2(locationX, locationY);
        if (!stop && Vector2.Distance(this.transform.localPosition, target) > 0.05f) //((this.transform.localPosition.x > locationX + .01f || this.transform.localPosition.x < locationX - .01f) || (this.transform.localPosition.y > locationY + .01f || this.transform.localPosition.y < locationY - .01f))
        {
            if (!isMoveToward)
                this.transform.localPosition = Vector2.Lerp(this.transform.localPosition, target, speedMove * Time.deltaTime);
            else
                this.transform.localPosition = Vector2.MoveTowards(this.transform.localPosition, target, speedMove * Time.deltaTime);
        }
        else if (!stop)
        {
            this.transform.localPosition = target;
            stop = true;
        }

    }

    private IEnumerator waitForSec()
    {
        stop = true;
        yield return new WaitForSeconds(waitForSecond);
        stop = false;
    }
}
