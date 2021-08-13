using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceMachine : MonoBehaviour
{
    public Transform ScaleMachine;
    public string Tag = "BalanceItem";

    void SetRayCast(RaycastHit2D hitTouch)
    {
        if (hitTouch && hitTouch.collider.transform.CompareTag(Tag))
        {
            Debug.Log(hitTouch.collider.transform.name);
        }
    }

    Touch touch;
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
