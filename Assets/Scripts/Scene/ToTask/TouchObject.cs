using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TouchObject : MonoBehaviour
{
    [SerializeField] private bool hasSound = false;
    [SerializeField] private bool hasName = false;
    [SerializeField] private bool scaleWithoutAnim = false;
    [SerializeField] private float scaleR = .1f;
    [SerializeField] private bool hide = false;
    [SerializeField] private bool dontChangeEnd = false;
    [Space]
    public int CountOfObjects = 0;
    public WasUnitComplete Parent = null;

    private Animator animator = null;
    private AudioSource sound = new AudioSource();
    private Text text = null;

    Vector3 scaleS;
    float xScale = 0;
    float yScale = 0;

    private Touch touch;
    private bool calculated = false;

    private static int currentCount = 0;
    private void Start()
    {
        scaleS = transform.localScale;
        xScale = scaleS.x;
        yScale = scaleS.y;


        this.TryGetComponent<SpriteRenderer>(out renderer);

        this.TryGetComponent<AudioSource>(out sound);
        this.TryGetComponent<Animator>(out animator);

        if (hasName && text == null)
            text = GameObject.FindGameObjectWithTag("ObjectsName").gameObject.GetComponent<Text>() as Text;

        if (hide)
            StartCoroutine(SetSpriteNull());

    }
    bool scale = false;


    private IEnumerator SetSpriteNull()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<SpriteRenderer>().sprite = null;
    }

    SpriteRenderer renderer = null;
    void SetRayCast(RaycastHit2D hitTouch)
    {
        if (hitTouch && transform == hitTouch.collider.transform)
        {
            //if(hitTouch.collider.CompareTag("Difference"))
            //{
            //    obj = hitTouch.collider.transform.gameObject as GameObject;

            //    if (obj.GetComponent<SpriteRenderer>().color.a == 0)
            //        Parent.GetComponent<WasUnitComplete>().SetCountOfDifference +=  1;
            //    obj.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 255);
            //}
            if (hasSound && (sound == null || !sound.isPlaying))
            {     
                if(sound != null)
                    sound.Play();

                if (!scaleWithoutAnim && animator != null)
                    animator.SetBool("zoom", true);
                else if (scaleWithoutAnim)
                    scale = true;
            }
            else if (!hasSound)
            {
                isShowing = true;
                if (!scaleWithoutAnim && animator != null)
                    animator.SetBool("zoom", true);
                else if(scaleWithoutAnim)
                    scale = true;
            }

            if (Parent != null && !calculated)
            {
                calculated = true;
                currentCount++;
                if (currentCount == CountOfObjects)
                {
                    Parent.CompleteUnit();
                }
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
                    text.text = this.name;

                if (renderer != null)
                    renderer.sortingLayerName = "Selected";

                if (!wait)
                {
                    wait = true;
                    StartCoroutine(hideText());
                }
            }
            else if(!isShowing)
            {
                if (sound != null)
                    sound.Stop();

                isShowing = false;
                if(!scaleWithoutAnim && animator != null)
                    if(!dontChangeEnd)
                        animator.SetBool("zoom", false);
                else if(scaleWithoutAnim)
                    if (!dontChangeEnd)
                        scale = false;

                if (renderer != null)
                    renderer.sortingLayerName = "Top";

                if (hasName && (text != null && text.text == this.name))
                    text.text = null;
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
