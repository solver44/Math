using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class painBackColor : MonoBehaviour
{
    public Color[] colorList;
    public Color currentColor;


    private Touch touch;
    private SpriteRenderer objectSprite = null;
    void SetRayCast(RaycastHit2D hitTouch)
    {
        if (hitTouch)
        {
            if(hitTouch.collider.transform.CompareTag("painting object"))
            {
                objectSprite = hitTouch.collider.gameObject.GetComponent<SpriteRenderer>();
                objectSprite.color = currentColor;
            }
        }
    }

    public void SetColor(string colorStr)
    {
        Color32 color = new Color32();
        switch (colorStr)
        {
            case "red":
                color = new Color32(255, 47, 47, 255);
                break;
            case "orange":
                color = new Color32(229, 138, 0, 255);
                break;
            case "green":
                color = new Color32(0, 229, 54, 255);
                break;
        }
        currentColor = color;
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
