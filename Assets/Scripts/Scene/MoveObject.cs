using System.Collections;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    [SerializeField] private Transform placeObject = null;
    [SerializeField] private Vector2 toThisLocation = Vector2.zero;
    [SerializeField] private Vector2 toThisScale = Vector2.zero;
    private Animator placeObjectAnim = null;
    [SerializeField] private bool killPlaceObject = false;
    [SerializeField] private bool anyLocation = false;
    [SerializeField] private float rangeX = 3f;
    [SerializeField] private float rangeY = 1f;
    public bool isAnimator = true;
    [SerializeField] private float scaleRadius = 0.1f;
    [SerializeField] private bool isScale = false;
    [SerializeField] private bool destroyEnd = false;
    [SerializeField] private bool dontSortLayer = false;

    [Header("Effects")]
    public bool EffectScale = false;
    public GameObject EffectLight = null;


    private Vector2 firstPosition;
    private bool locked = false;
    private float deltaX, deltaY;
    private Vector3 mousePosition;
    private Animator anim = null;

    private Vector2 placeLocation = Vector2.zero;
    private Touch touch;

    private SpriteRenderer render = null;
    private Transform currentTransform = null;

    private void Awake()
    {
        render = GetComponent<SpriteRenderer>() as SpriteRenderer;
        if (placeObject != null)
        {
            placeObjectAnim = placeObject.GetComponent<Animator>() as Animator;
            placeLocation = placeObject.transform.position;
        }
        else if (toThisLocation != Vector2.zero)
        {
            placeLocation = toThisLocation;
        }
        else
            empty = true;

        currentTransform = GetComponent<Transform>() as Transform;
        
        if(EffectLight != null)
        {
            EffectLight.AddComponent<RotateObject>();
        }
        if (EffectScale)
            this.gameObject.AddComponent<ScaleEffect>();
    }

    private void Start()
    {
        if (isAnimator)
            anim = GetComponent<Animator>() as Animator;
        scaleS = transform.localScale;
        xScale = scaleS.x;
        yScale = scaleS.y;

        if (toThisScale == Vector2.zero)
            toThisScale = this.transform.localScale;
    }

    Vector3 scaleS;
    float xScale = 0;
    float yScale = 0;
    private void OnMouseDown()
    {
        if(firstPosition != null)
            firstPosition = GetComponent<Transform>().position;
        if (!locked)
        {
            if(!dontSortLayer)
                this.GetComponent<SpriteRenderer>().sortingOrder += 1;
            deltaX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
            deltaY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
        }
    }

    private bool scale = false;
    private void OnMouseDrag()
    {
        if (!locked)
        {
            if(anyLocation)
                firstPosition = GetComponent<Transform>().position;


            if (isAnimator)
                anim.SetBool("zoom", true);
            else
            {
                scale = true;
            }
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector2(mousePosition.x - deltaX, mousePosition.y - deltaY);
        }
    }
    bool toLocalPos = false;
    bool destroy = false;
    bool empty = false;
    private void OnMouseUp()
    {
        if (!locked && !empty)
        {
            if(!dontSortLayer)
                render.sortingOrder -= 1;
            Debug.Log(transform.position + "\n" + placeLocation);
            if (Mathf.Abs(transform.position.x - placeLocation.x) <= rangeX && 
                Mathf.Abs(transform.position.y - placeLocation.y) <= rangeY)
            {
                currentCnt++;
                if (isAnimator)
                {
                    anim.SetTrigger("destroy");
                }
                else
                {
                    scale = false;
                    if (placeObject == null)
                        StartCoroutine(moveAnim());
                    else
                        destroy = true;
                }
                if (killPlaceObject)
                    placeObjectAnim.SetTrigger("destroy");
                StartCoroutine(makeInActive());
            }
            else
            {
                if (isAnimator)
                    anim.SetBool("zoom", false);
                else
                {
                    scale = false;
                }
                toLocalPos = true;
                transform.position = Vector2.Lerp(transform.position, firstPosition, 2f);
            }
        }
        else if (empty)
        {
            transform.position = Vector2.Lerp(transform.position, firstPosition, 2f);
        }
    }
    private IEnumerator makeInActive()
    {
        yield return new WaitForSeconds(1);
        //this.transform.gameObject.SetActive(false);
        if (countOfObjects == currentCnt)
        {
            currentCnt = 0;
            this.transform.parent.transform.parent.transform.GetComponentInParent<WasUnitComplete>().CompleteUnit();
        }
        if (destroy)
            Destroy(this.transform.gameObject);
        if(killPlaceObject)
            Destroy(placeObject.transform.gameObject);
    }

    void Update()
    {
        if (locked)
            return;

        if(placeObject != null)
            placeLocation = placeObject.transform.position;

        if (!isAnimator && isScale)
        {

            if (scale && !destroy)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(xScale + (xScale * scaleRadius), yScale + (yScale * scaleRadius)), .08f);
            }
            else if (!destroy)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, scaleS, .08f);
            }
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0, 0), .08f);
            }
        }

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(touch.deltaPosition).x, Camera.main.ScreenToWorldPoint(touch.deltaPosition).y), Vector2.zero, 0);
            if (hit && transform.name == hit.collider.transform.name)
            {
                if(touch.phase == TouchPhase.Began)
                {
                    if (firstPosition != null)
                        firstPosition = currentTransform.localPosition;
                    if (!locked)
                    {
                        render.sortingOrder += 1;
                        deltaX = Camera.main.ScreenToWorldPoint(touch.deltaPosition).x - transform.position.x;
                        deltaY = Camera.main.ScreenToWorldPoint(touch.deltaPosition).y - transform.position.y;
                    }
                }
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (!locked)
                    {
                        if (anyLocation)
                            firstPosition = currentTransform.localPosition;
                        if (isAnimator)
                            anim.SetBool("zoom", true);
                        else
                            scale = true;
                        mousePosition = Camera.main.ScreenToWorldPoint(touch.deltaPosition);
                        transform.position = new Vector2(mousePosition.x - deltaX, mousePosition.y - deltaY);
                    }
                }
                if(touch.phase == TouchPhase.Ended)
                {
                    if (!locked)
                    {
                        if ((isAnimator))
                            anim.SetBool("zoom", false);
                        else
                            scale = false;
                        render.sortingOrder -= 1;
                        if (Mathf.Abs(transform.position.x - placeLocation.x) <= rangeX &&
                            Mathf.Abs(transform.position.y - placeLocation.y) <= rangeY)
                        {
                            currentCnt++;
                            if (isAnimator)
                            {
                                anim.SetTrigger("destroy");
                            }
                            else
                            {
                                if(placeObject == null)
                                    StartCoroutine(moveAnim());
                                else
                                    destroy = true;
                            }
                            if (killPlaceObject)
                                placeObjectAnim.SetTrigger("destroy");
                            StartCoroutine(makeInActive());
                        }
                        else
                        {
                            if (isAnimator)
                                anim.SetBool("zoom", false);
                            else
                            {
                                scale = false;
                            }
                            toLocalPos = true;
                            transform.localPosition = Vector2.Lerp(transform.localPosition, firstPosition, 2f);
                        }
                    }
                }

            }
        }
    }
    public int countOfObjects = 0;
    static int currentCnt = 0;
    IEnumerator moveAnim() 
    {
        float scaleDuration = 2f;
        for (float t = 0; t < 1; t += Time.deltaTime / scaleDuration)
        {
            transform.position = Vector3.Lerp(transform.position, placeLocation, t);
            transform.localScale = Vector3.Lerp(transform.localScale, toThisScale, t);
            if (transform.localPosition.Equals(placeLocation))
            {
                //transform.localScale = toThisScale;
                IEnumerator co = moveAnim();
                locked = true;
                if (countOfObjects == currentCnt)
                {
                    currentCnt = 0;
                    this.transform.parent.transform.parent.transform.GetComponentInParent<WasUnitComplete>().CompleteUnit();
                }
                if (destroyEnd)
                    destroy = true;
                StopCoroutine(co);
                yield break;
            }
            yield return null;
        }
    }
}
