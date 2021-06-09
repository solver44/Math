using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintBackColor : MonoBehaviour
{
    public float ScaleRadius = 0.1f;

    private Color currentColor;
    private string currentShapesName;

    private Touch touch;
    private SpriteRenderer objectSprite = null;

    void SetRayCast(RaycastHit2D hitTouch)
    {
        if (hitTouch && hitTouch.collider.transform.CompareTag("painting object"))
        {
            EqualShapes checkingEquals = hitTouch.collider.transform.GetComponent<EqualShapes>();
            bool sum = false;
            objectSprite = hitTouch.collider.gameObject.GetComponent<SpriteRenderer>();
            if (objectSprite.color.Equals(Color.white))
                sum = true;

            if(checkingEquals.CheckIt(currentShapesName, sum))
            {
                objectSprite.color = currentColor;
            }
        }
    }

    GameObject prevBtn = null;
    public void SetColor(GameObject pressedBtn)
    {
        if (prevBtn == pressedBtn || sleep)
            return;

        if (prevBtn != null)
        {
            StartCoroutine(SmoothScaling(prevBtn.GetComponent<RectTransform>(), true));
        }

        prevBtn = pressedBtn.gameObject;   

        Color32 color = pressedBtn.GetComponent<Image>().color;
        RectTransform scale = pressedBtn.GetComponent<RectTransform>();

        StartCoroutine(SmoothScaling(scale, false));

        currentShapesName = pressedBtn.GetComponentInParent<Toggle>().name;
        currentColor = color;
    }


    bool sleep = false;
    IEnumerator SmoothScaling(RectTransform scale, bool pros)
    {
        sleep = true;
        float scaleDuration = 1;
        float startX = scale.localScale.x;
        float startY = scale.localScale.y;
        Vector3 toScale = new Vector3();
        if (!pros)
            toScale = new Vector3(startX - ScaleRadius * startX, startY - ScaleRadius * startY);
        else
            toScale = new Vector3(startX + ScaleRadius * startX, startY + ScaleRadius * startY);

        for (float t = 0; t < 1; t += Time.deltaTime / scaleDuration)
        {
            scale.localScale = Vector3.Lerp(scale.localScale, toScale, t);
            if (scale.localScale == toScale)
            {
                sleep = false;
                StopAllCoroutines();
                yield break;
            }
            yield return null;
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
