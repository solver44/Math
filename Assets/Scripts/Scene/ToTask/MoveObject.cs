using System.Collections;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public bool DontMoving = false;

    [SerializeField] private Transform placeObject = null;
    public bool HasPlaceObjectCollider = false;
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
    public bool IsLocalPos = true;

    [Header("Effects")]
    public bool EffectScale = false;
    public GameObject EffectLight = null;
    public bool DestroyEffect = false;
    public GameObject EffectToDestroy = null;


    private Vector2 firstPosition;
    private bool locked = false;
    private float deltaX, deltaY;
    private Vector3 mousePosition;
    private Animator anim = null;

    private Vector2 placeLocation = Vector2.zero;
    private Touch touch;

    //this Transform
    private SpriteRenderer render = null;

    private OnCollision2D collOfPlaceObject = null;
    private void Awake()
    {
        if (DontMoving)
            locked = true;

        render = GetComponent<SpriteRenderer>() as SpriteRenderer;
        if (placeObject != null && !HasPlaceObjectCollider)
        {
            placeObjectAnim = placeObject.GetComponent<Animator>() as Animator;
            if(IsLocalPos)
                placeLocation = placeObject.transform.localPosition;
            else
                placeLocation = placeObject.transform.position;

        }
        else if (toThisLocation != Vector2.zero && !HasPlaceObjectCollider)
        {
            placeLocation = toThisLocation;
        }else if (HasPlaceObjectCollider)
        {
            placeLocation = Vector2.zero;
            collOfPlaceObject = placeObject.GetComponent<OnCollision2D>();
        }
        else
            empty = true;
        
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

        if (toThisScale == new Vector2(0, 0))
        {
            toThisScale = Vector2.zero;
        }

        if (toThisScale == Vector2.zero)
            toThisScale = this.transform.localScale;

        if (IsLocalPos)
            firstPosition = this.transform.localPosition;
        else
            firstPosition = this.transform.position;
    }

    Vector3 scaleS;
    float xScale = 0;
    float yScale = 0;

    ScaleEffect effect = new ScaleEffect();
    private void OnMouseDown()
    {
        if (DontMoving)
            return;

        if (!locked)
        {
            if (isAnimator)
                anim.SetBool("zoom", true);
            else
                StartCoroutine(effect.Scale(transform, new Vector3(xScale + (xScale * scaleRadius), yScale + (yScale * scaleRadius)), 1f));

            if (!dontSortLayer)
                this.GetComponent<SpriteRenderer>().sortingOrder += 1;
            deltaX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
            deltaY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
        }
    }

    private bool scale = false;
    private void OnMouseDrag()
    {
        if (DontMoving)
            return;

        if (!locked)
        {
            if (anyLocation)
            {
                if (IsLocalPos)
                    firstPosition = transform.localPosition;
                else
                    firstPosition = transform.position;
            }

            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector2(mousePosition.x - deltaX, mousePosition.y - deltaY);
        }
    }
    bool toLocalPos = false;
    bool destroy = false;
    bool empty = false;

    [HideInInspector] public bool DontMoveFirstPosition = false;
    private void OnMouseUp()
    {
        if (DontMoving)
            return;

        if (!locked && !empty)
        {
            if(!dontSortLayer)
                render.sortingOrder -= 1;

            Vector2 location = Vector2.zero;
            if (IsLocalPos)
                location = GetComponent<Transform>().localPosition;
            else
                location = GetComponent<Transform>().position;

            if ((collOfPlaceObject != null && collOfPlaceObject.OnCollision && collOfPlaceObject.NameOfObject.Equals(placeObject.transform.name) && collOfPlaceObject.NameOfTriggeredObject.Equals(this.transform.name))
                || (((Mathf.Abs(location.x - placeLocation.x)) <= rangeX &&
                Mathf.Abs(location.y - placeLocation.y) <= rangeY) && collOfPlaceObject == null))
            {
                if (isAnimator)
                {
                    anim.SetTrigger("destroy");
                    //currentCnt++;
                }
                else
                {
                    StartCoroutine(methodScale(transform, scaleS, false, 1f)); //Уменшить когда отпускат мышку

                    if (placeObject == null || HasPlaceObjectCollider)
                    {
                        if (!HasPlaceObjectCollider)
                            StartCoroutine(moveAnim(placeLocation, IsLocalPos));
                        else
                        {
                            StartCoroutine(moveAnim(toThisLocation, IsLocalPos));
                        }
                    }
                    else
                    {
                        StartCoroutine(methodScale(this.transform, new Vector2(0, 0), true, .5f));
                    }
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
                    StartCoroutine(methodScale(transform, scaleS, false, 1f));
                }

                toLocalPos = true;

                if (IsLocalPos && !DontMoveFirstPosition)
                    transform.localPosition = Vector2.Lerp(transform.localPosition, firstPosition, 2f);
                else if(!DontMoveFirstPosition)
                    transform.position = Vector2.Lerp(transform.position, firstPosition, 2f);

            }
        }
        else if (empty)
        {
            StartCoroutine(methodScale(transform, scaleS, false, 1f));

            if (IsLocalPos && !DontMoveFirstPosition)
                transform.localPosition = Vector2.Lerp(transform.localPosition, firstPosition, 2f);
            else if(!DontMoveFirstPosition)
                transform.position = Vector2.Lerp(transform.position, firstPosition, 2f);
        }
    }

    public void DontMove()
    {
        currentCnt++;
        locked = true;
    }
    private IEnumerator makeInActive()
    {
        yield return new WaitForSeconds(1);
        //this.transform.gameObject.SetActive(false);
        if(!HasPlaceObjectCollider && placeObject != null)
            currentCnt++;

        if (countOfObjects == currentCnt)
        {
            currentCnt = 0;
            this.transform.parent.transform.parent.transform.GetComponentInParent<WasUnitComplete>().CompleteUnit();
        }
        if (destroy)
        {
            Destroy(this.transform.gameObject);
        }
        if(killPlaceObject)
            Destroy(placeObject.transform.gameObject);
    }

    void Update()
    {
        if (locked)
            return;

        if (placeObject != null)
        {
            if(IsLocalPos)
                placeLocation = placeObject.transform.localPosition;
            else
                placeLocation = placeObject.transform.position;
        }

        //if (!isAnimator && isScale)
        //{

        //    if (scale && !destroy)
        //    {
        //        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(xScale + (xScale * scaleRadius), yScale + (yScale * scaleRadius)), .08f);
        //    }
        //    else if (!destroy)
        //    {
        //        transform.localScale = Vector3.Lerp(transform.localScale, scaleS, .08f);
        //    }
        //}

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(touch.deltaPosition).x, Camera.main.ScreenToWorldPoint(touch.deltaPosition).y), Vector2.zero, 0);
            if (hit && transform.name == hit.collider.transform.name)
            {
                if(touch.phase == TouchPhase.Began)
                {
                    if (firstPosition != null)
                    {
                        if (IsLocalPos)
                            firstPosition = GetComponent<Transform>().localPosition;
                        else
                            firstPosition = GetComponent<Transform>().position;
                    }
                    if (!locked)
                    {
                        if (!dontSortLayer)
                            this.GetComponent<SpriteRenderer>().sortingOrder += 1;
                        deltaX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
                        deltaY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
                    }
                }
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (!locked)
                    {
                        if (anyLocation)
                        {
                            if (IsLocalPos)
                                firstPosition = GetComponent<Transform>().localPosition;
                            else
                                firstPosition = GetComponent<Transform>().position;
                        }


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
                if(touch.phase == TouchPhase.Ended)
                {
                    if (!locked && !empty)
                    {
                        if (!dontSortLayer)
                            render.sortingOrder -= 1;

                        Vector2 location = Vector2.zero;
                        if (IsLocalPos)
                            location = GetComponent<Transform>().localPosition;
                        else
                            location = GetComponent<Transform>().position;

                        if ((collOfPlaceObject != null && collOfPlaceObject.OnCollision && collOfPlaceObject.NameOfObject.Equals(placeObject.transform.name) & collOfPlaceObject.NameOfTriggeredObject.Equals(this.transform.name))
                            || (((Mathf.Abs(location.x - placeLocation.x)) <= rangeX &&
                            Mathf.Abs(location.y - placeLocation.y) <= rangeY) && collOfPlaceObject == null))
                        {
                            if (isAnimator)
                            {
                                anim.SetTrigger("destroy");
                                //currentCnt++;
                            }
                            else
                            {
                                scale = false;
                                if (placeObject == null || HasPlaceObjectCollider)
                                {
                                    if (!HasPlaceObjectCollider)
                                        StartCoroutine(moveAnim(placeLocation, IsLocalPos));
                                    else
                                        StartCoroutine(moveAnim(toThisLocation, IsLocalPos));
                                }
                                else
                                {
                                    StartCoroutine(methodScale(this.transform, new Vector2(0, 0), true, 1f));
                                }
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

                            if (IsLocalPos)
                                transform.localPosition = Vector2.Lerp(transform.localPosition, firstPosition, 2f);
                            else
                                transform.position = Vector2.Lerp(transform.position, firstPosition, 2f);

                        }
                    }
                    else if (empty)
                    {
                        if (IsLocalPos)
                            transform.localPosition = Vector2.Lerp(transform.localPosition, firstPosition, 2f);
                        else
                            transform.position = Vector2.Lerp(transform.position, firstPosition, 2f);
                    }
                }

            }
        }
    }
    public int countOfObjects = 0;
    static int currentCnt1 = 0;

    private int currentCnt
    {
        get { return currentCnt1; }
        set { currentCnt1 = value;
	        if(DestroyEffect){
	            Destroy(EffectToDestroy);
	        }

            if (countOfObjects == currentCnt1)
            {
                currentCnt1 = 0;
                this.transform.GetComponentInParent<WasUnitComplete>().CompleteUnit();
            }
        }
    }
    IEnumerator moveAnim(Vector2 _placeLocation, bool isLocal) 
    {
        float scaleDuration = 2f;
        for (float t = 0; t < 1; t += Time.deltaTime / scaleDuration)
        {
            if(isLocal)
                transform.localPosition = Vector3.Lerp(transform.localPosition, _placeLocation, t);
            else
                transform.position = Vector3.Lerp(transform.position, _placeLocation, t);

            transform.localScale = Vector3.Lerp(transform.localScale, toThisScale, t);
            if ((IsLocalPos && transform.localPosition.Equals(_placeLocation)) || (!IsLocalPos && transform.position.Equals(_placeLocation)))
            {
                currentCnt++;
                //transform.localScale = toThisScale;
                locked = true;
                if (destroyEnd)
                {
                    StartCoroutine(methodScale(this.transform, new Vector2(0, 0), true, .1f));
                }
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator methodScale(Transform parent, Vector2 toScale, bool moreEffect, float duration)
    {
        if (destroy)
            yield break;

        if (moreEffect)
        {
            SpriteRenderer render = null;
            parent.TryGetComponent<SpriteRenderer>(out render);


            Vector2 target = new Vector2(parent.localScale.x + (parent.localScale.x * .2f), parent.localScale.y + (parent.localScale.y * .2f));
            for (float i = 0; i <= 1; i += Time.deltaTime / duration)
            {
                if (render == null)
                    parent.localScale = Vector2.MoveTowards(parent.localScale, target, 3f);
                else
                {
                    parent.localScale = Vector2.MoveTowards(parent.localScale, target, .1f);
                    Debug.Log(parent.localScale);
                }

                if (parent.localScale.Equals(target))
                    break;
                yield return new WaitForEndOfFrame();
            }
        }

        for (float i = 0; i <= 1; i += Time.deltaTime / duration)
        {
            parent.localScale = Vector2.Lerp(parent.localScale, toScale, i);
            if (parent.localScale.Equals(toScale))
                yield break;
            yield return new WaitForEndOfFrame();
        }

        destroy = true;
    }
}
