using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintBackColor : MonoBehaviour
{
    public bool CheckEqualShapes = true;
    public bool DontCheck = false;
    public bool IsZoomScale = false;
    public bool IsWhite = false;
    public bool SortLayer = false;
    public float ScaleRadius = 0.1f;
    [Space(20)]
    public bool PaintTwoSameObjectToSameColor = false;

    [Header("Other")]
    public bool MoveUp = false;
    private Vector2 firstPos = Vector2.zero;

    private Color currentColor = Color.white;
    private string currentShapesName;

    private Touch touch;
    private SpriteRenderer objectSprite = null;
    private Image objectImage = null;

    private void Start()
    {
        prevBtn = null;
        currentColor = Color.white;
        if (PaintTwoSameObjectToSameColor)
        {
            IsWhite = false;
            DontCheck = false;
            CheckEqualShapes = false;
        }
    }

    EqualShapes checkingEquals = null;
    Dictionary<Transform, Color> temps = new Dictionary<Transform, Color>();
    List<string> completed = new List<string>();
    private void PaintTwoSame(RaycastHit2D hitTouch)
    {
        bool sum = false;    
        if (hitTouch.collider.TryGetComponent<SpriteRenderer>(out objectSprite))
        {
            if(temps.ContainsKey(objectSprite.transform)
               && temps[objectSprite.transform] == currentColor
               && !completed.Contains(objectSprite.name))
            {
                sum = true;
                objectSprite.color = currentColor;
                completed.Add(objectSprite.name);
            }
            else if (temps.ContainsKey(objectSprite.transform)
                    && temps.ContainsValue(currentColor)
                    && !completed.Contains(objectSprite.name))
            {
                sum = false;
                objectSprite.color = currentColor;
                temps[objectSprite.transform] = currentColor;
            }
            else if(!temps.ContainsKey(objectSprite.transform)
                    && !temps.ContainsValue(currentColor))
            {
                sum = true;
                objectSprite.color = currentColor;
                temps.Add(objectSprite.transform, currentColor);
            }
            else
                sum = false;
        }
        else if (hitTouch.collider.TryGetComponent<Image>(out objectImage))
        {
            if (temps.ContainsKey(objectImage.transform)
               && temps[objectImage.transform] == currentColor
               && !completed.Contains(objectImage.name))
            {
                sum = true;
                objectImage.color = currentColor;
                completed.Add(objectImage.name);
            }
            else if(temps.ContainsKey(objectImage.transform)
                    && !completed.Contains(objectImage.name))
            {
                sum = false;
                objectImage.color = currentColor;
                temps[objectImage.transform] = currentColor;
            }
            else if (!temps.ContainsKey(objectImage.transform)
                    && !temps.ContainsValue(currentColor))
            {
                sum = true;
                objectImage.color = currentColor;
                temps.Add(objectImage.transform, currentColor);
            }
            else
                sum = false;
        }

        if(hitTouch.collider.TryGetComponent<EqualShapes>(out checkingEquals))
            checkingEquals.CheckIt(currentShapesName, sum, DontCheck);
    }
    void SetRayCast(RaycastHit2D hitTouch)
    {
        if (currentColor == Color.white)
            return;

        if (hitTouch && hitTouch.collider.transform.CompareTag("painting object"))
        {
            if (PaintTwoSameObjectToSameColor)
            {
                PaintTwoSame(hitTouch);
                return;
            }

            if (CheckEqualShapes)
            {
                checkingEquals = hitTouch.collider.GetComponent<EqualShapes>();
                bool sum = false;
                if (hitTouch.collider.gameObject.TryGetComponent<SpriteRenderer>(out objectSprite))
                {
                    if ((!IsWhite && !objectSprite.color.Equals(currentColor)) || (IsWhite && objectSprite.color.Equals(Color.white)))
                        sum = true;
                }else
                {
                    objectImage = hitTouch.collider.gameObject.GetComponent<Image>();
                    if ((!IsWhite && !objectImage.color.Equals(currentColor)) || (IsWhite && objectImage.color.Equals(Color.white)))
                        sum = true;
                    if (SortLayer)
                        objectSprite.sortingLayerID += 2;
                }

                if (checkingEquals.CheckIt(currentShapesName, sum, false))
                {
                    if (objectSprite != null)
                        objectSprite.color = currentColor;
                    else
                        objectImage.color = currentColor;
                }
            }else
            {
                hitTouch.collider.TryGetComponent<EqualShapes>(out checkingEquals);
                bool sum = false;
                if (hitTouch.collider.TryGetComponent<SpriteRenderer>(out objectSprite))
                {
                    if ((!IsWhite && !objectSprite.color.Equals(currentColor)) || (IsWhite && objectSprite.color.Equals(Color.white)))
                        sum = true;
                    if (SortLayer)
                        objectSprite.sortingOrder += 2;
                }
                else
                {
                    objectImage = hitTouch.collider.GetComponent<Image>();
                    if ((!IsWhite && !objectImage.color.Equals(currentColor)) || (IsWhite && objectImage.color.Equals(Color.white)))
                        sum = true;
                }


                checkingEquals.CheckIt(currentShapesName, sum, DontCheck);
                if (objectSprite != null)
                    objectSprite.color = currentColor;
                else
                    objectImage.color = currentColor;
            }
        }
    }

    private GameObject prevBtn = null;

    private ScaleEffect effect = new ScaleEffect();
    public void SetColor(GameObject pressedBtn)
    {
        if (sleep || prevBtn == pressedBtn)
            return;

        if (prevBtn != null)
        {
            //firstScale = prevBtn.transform.localScale;
            if (!MoveUp)
                StartCoroutine(SmoothScaling(prevBtn.GetComponent<RectTransform>(), !IsZoomScale, true));
            else
            {
                StartCoroutine(effect.MoveAnim(prevBtn.transform, firstPos, true, 1f));
            }
        }

        prevBtn = pressedBtn.gameObject;
        if (!MoveUp)
            firstScale = pressedBtn.transform.localScale;
        else
            firstPos = prevBtn.transform.localPosition;

        color = pressedBtn.GetComponent<Image>().color;
        scale = pressedBtn.GetComponent<RectTransform>();

        if(!MoveUp)
            StartCoroutine(SmoothScaling(scale, IsZoomScale, false));
        else
        {
            StartCoroutine(effect.MoveAnim(prevBtn.transform, new Vector2(prevBtn.transform.localPosition.x, firstPos.y + 60f), true, 1f));
        }

        currentShapesName = pressedBtn.GetComponentInParent<Toggle>().name;
        currentColor = color;
    }
    RectTransform scale;
    Color32 color;


    bool sleep = false;
    Vector2 firstScale;
    IEnumerator SmoothScaling(RectTransform scale, bool pros, bool isFirst)
    {
        sleep = true;
        float scaleDuration = 1f;
        float startX = firstScale.x;
        float startY = firstScale.y;
        Vector3 toScale = new Vector3();
        if (!pros)
            toScale = new Vector3(startX - ScaleRadius * startX, startY - ScaleRadius * startY, 1);
        else
            toScale = new Vector3(startX + ScaleRadius * startX, startY + ScaleRadius * startY, 1);

        if (isFirst)
            toScale = firstScale;
        for (float t = 0; t < 1; t += Time.deltaTime / scaleDuration)
        {
            scale.localScale = Vector3.Lerp(scale.localScale, toScale, t);
            if (scale.localScale == toScale)
            {
                sleep = false;
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
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
    }
}
