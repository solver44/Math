using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineBetweenTwoObjs : MonoBehaviour
{
    [SerializeField] private Transform pointA = null;
    [SerializeField] private Transform pointB = null;
    private Vector3 pointAB = new Vector3();

    LineRenderer line;
    private Vector3 startPos = Vector3.zero;
    private Vector3 endPos = Vector3.zero;

    private float interpolateAmount;

    private bool startAnim = false;
    
    private void Start()
    {
        line = GetComponent<LineRenderer>();
        resetColor();
    }
    void resetColor()
    {
        byte r = (byte)Random.Range(10, 220);
        byte g = (byte)Random.Range(10, 220);
        byte b = (byte)Random.Range(10, 220);
        line.startColor = new Color32(r, g, b, 220);
        line.endColor = new Color32(r, g, b, 220);
        line.enabled = false;

    }
    void SetRayCast(RaycastHit2D hitTouch)
    {
        if (hitTouch && !startAnim && hitTouch.collider.name == transform.name)
        {
            line.enabled = true;
            startPos = pointA.position;
            startPos.z = 0;
            pointAB = startPos;
            line.SetPosition(0, startPos);
            endPos = pointB.transform.position;
            line.SetPosition(1, startPos);
            startAnim = true;
        }
    }
    private void Update()
    {
        interpolateAmount =(interpolateAmount + 0.0001f) % 1f;
        //pointAB.position = Vector3.Lerp(pointA.position, pointB.position, interpolateAmount);
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hitTouch = new RaycastHit2D();
            hitTouch = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0);

            SetRayCast(hitTouch);
        }
        if (Input.touchCount > 0)
        {
            RaycastHit2D hitTouch = new RaycastHit2D();
            Touch touch;
            touch = Input.GetTouch(0);
            hitTouch = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(touch.deltaPosition).x, Camera.main.ScreenToWorldPoint(touch.deltaPosition).y), Vector2.zero, 0);

            SetRayCast(hitTouch);
        }

        if (startAnim)
        {
            pointAB = Vector3.Lerp(pointAB, endPos, interpolateAmount);
            line.SetPosition(1, pointAB);

            float x = ((float)((int)(pointAB.x * 100)) / 100f);
            float y = ((float)((int)(pointAB.y * 100)) / 100f);
            float x1 = ((float)((int)(endPos.x * 100)) / 100f);
            float y1 = ((float)((int)(endPos.y * 100)) / 100f);

            Debug.Log(x);
            if (x == x1 && y1 == y)
            {
                startAnim = false;
                StartCoroutine(makeHide());
            }
        }
        
    }

    private IEnumerator makeHide()
    {
        yield return new WaitForSeconds(2);
        pointAB = new Vector3();
        startPos = new Vector3();
        endPos = new Vector3();
        line.enabled = false;
        resetColor();
    }

}
