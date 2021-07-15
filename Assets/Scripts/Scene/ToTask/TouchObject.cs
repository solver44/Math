using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TouchObject : MonoBehaviour
{
    [SerializeField] private bool hasSound = false;
    [SerializeField] private bool hasName = false;
    [SerializeField] private bool scaleWithoutAnim = false;
    [SerializeField] private float scaleR = .1f;

    private Animator animator = null;
    private AudioSource sound = null;
    private GameObject obj = null;
    private Text text = null;

    Vector3 scaleS;
    float xScale = 0;
    float yScale = 0;

    private Touch touch;

    private void Start()
    {
        scaleS = transform.localScale;
        xScale = scaleS.x;
        yScale = scaleS.y;

    }
    bool scale = false;
    void SetRayCast(RaycastHit2D hitTouch)
    {
        if (hitTouch)
        {
            //if(hitTouch.collider.CompareTag("Difference"))
            //{
            //    obj = hitTouch.collider.transform.gameObject as GameObject;

            //    if (obj.GetComponent<SpriteRenderer>().color.a == 0)
            //        Parent.GetComponent<WasUnitComplete>().SetCountOfDifference +=  1;
            //    obj.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 255);
            //}
            if (hasSound && (transform == hitTouch.collider.transform) && (sound == null || !sound.isPlaying))
            {
                if (hasName && text == null)
                    text = GameObject.FindGameObjectWithTag("ObjectsName").gameObject.GetComponent<Text>() as Text;

                obj = hitTouch.collider.transform.gameObject as GameObject;

                try
                {
                    sound = obj.GetComponent<AudioSource>() as AudioSource;
                    sound.Play();
                }
                catch { sound = new AudioSource(); }
                try
                {
                    animator = obj.GetComponent<Animator>() as Animator;
                }
                catch { }
                if (!scaleWithoutAnim && animator != null)
                    animator.SetBool("zoom", true);
                else if (scaleWithoutAnim)
                    scale = true;
            }
            else if (!hasSound && (transform == hitTouch.collider.transform))
            {
                //sound = new AudioSource();
                if (hasName && text == null)
                    text = GameObject.FindGameObjectWithTag("ObjectsName").gameObject.GetComponent<Text>() as Text;

                obj = hitTouch.collider.transform.gameObject as GameObject;

                isShowing = true;
                try
                {
                    animator = obj.GetComponent<Animator>() as Animator;
                }
                catch { }
                if (!scaleWithoutAnim && animator != null)
                    animator.SetBool("zoom", true);
                else if(scaleWithoutAnim)
                    scale = true;
            }
        }
    }
    bool isShowing = false;
    bool wait = false;
    private IEnumerator hideText()
    {
        yield return new WaitForSeconds(2);
        isShowing = false;
        wait = false;
    }
    private void Update()
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


        if (animator != null || scaleWithoutAnim) {
            if (isShowing || (sound != null && sound.isPlaying))
            {
                if(!scaleWithoutAnim && animator != null)
                        animator.SetBool("zoom", true);
                else if(scaleWithoutAnim)
                    scale = true;
                if(hasName)
                    text.text = obj.name;
                try
                {
                    GetComponent<SpriteRenderer>().sortingLayerName = "Selected";
                }
                catch { }

                if (!wait)
                {
                    wait = true;
                    StartCoroutine(hideText());
                }
            }
            else if(!isShowing)
            {
                try
                {
                    sound.Stop();
                }
                catch { }

                isShowing = false;
                if(!scaleWithoutAnim && animator != null)
                    animator.SetBool("zoom", false);
                else if(scaleWithoutAnim)
                    scale = false;

                try
                {
                    GetComponent<SpriteRenderer>().sortingLayerName = "Top";
                }
                catch { }
                try {
                    if (hasName && text.text == obj.name)
                        text.text = null;
                }
                catch { }
            }
        }

        if(scale && scaleWithoutAnim)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(xScale  + (xScale * scaleR), yScale + (yScale * scaleR)), .05f);
        }else if(!scale && scaleWithoutAnim)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, scaleS, .05f);
        }
    }
}
