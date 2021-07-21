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
    public float ScaleRadius = 0.1f;

    [Header("Other")]
    public bool MoveUp = false;
    private Vector2 firstPos = Vector2.zero;

    private Color currentColor;
    private string currentShapesName;

    private Touch touch;
    private SpriteRenderer objectSprite = null;
    private Image objectImage = null;


    private void Start()
    {
        prevBtn = null;
        currentColor = Color.white;
    }
    void SetRayCast(RaycastHit2D hitTouch)
    {
        if (hitTouch && hitTouch.collider.transform.CompareTag("painting object"))
        {
            if (CheckEqualShapes)
            {
                EqualShapes checkingEquals = hitTouch.collider.transform.GetComponent<EqualShapes>();
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
                EqualShapes checkingEquals = hitTouch.collider.transform.GetComponent<EqualShapes>();
                bool sum = false;
                if (hitTouch.collider.gameObject.TryGetComponent<SpriteRenderer>(out objectSprite))
                {
                    if ((!IsWhite && !objectSprite.color.Equals(currentColor)) || (IsWhite && objectSprite.color.Equals(Color.white)))
                        sum = true;
                }
                else
                {
                    objectImage = hitTouch.collider.gameObject.GetComponent<Image>();
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
                StartCoroutine(effect.MoveAnim(prevBtn.transform, firstPos, false, 1f));
            }
        }

        prevBtn = pressedBtn.gameObject;
        if (!MoveUp)
            firstScale = pressedBtn.transform.localScale;
        else
            firstPos = prevBtn.transform.position;

        Color32 color = pressedBtn.GetComponent<Image>().color;
        RectTransform scale = pressedBtn.GetComponent<RectTransform>();

        if(!MoveUp)
            StartCoroutine(SmoothScaling(scale, IsZoomScale, false));
        else
        {
            StartCoroutine(effect.MoveAnim(prevBtn.transform, new Vector2(prevBtn.transform.position.x, firstPos.y + .6f), false, 1f));
        }

        currentShapesName = pressedBtn.GetComponentInParent<Toggle>().name;
        currentColor = color;
    }


    bool sleep = false;
    Vector2 firstScale;
    IEnumerator SmoothScaling(RectTransform scale, bool pros, bool isFirst)
    {
        sleep = true;
        float scaleDuration = 1;
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
