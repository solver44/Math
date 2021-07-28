using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineBetweenTwoObjs : MonoBehaviour
{
    [SerializeField]
    private float animationDuration = 2f;
    [SerializeField]
    private float visibleTime = 1f;
    public bool startOnAwake = false;

    private LineRenderer lineRenderer;
    private Vector3[] linePoints;
    private int pointsCount;

    private bool startAnim = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        resetColor();

        // Store a copy of lineRenderer's points in linePoints array
        pointsCount = lineRenderer.positionCount;
        linePoints = new Vector3[pointsCount];

        for (int i = 0; i < pointsCount; i++)
        {
            linePoints[i] = lineRenderer.GetPosition(i);
        }

        if (startOnAwake)
            StartCoroutine(AnimateLine());
    }
    void resetColor()
    {
        byte r = (byte)Random.Range(10, 220);
        byte g = (byte)Random.Range(10, 220);
        byte b = (byte)Random.Range(10, 220);
        lineRenderer.startColor = new Color32(r, g, b, 220);
        lineRenderer.endColor = new Color32(r, g, b, 220);
        lineRenderer.enabled = false;

    }
    void SetRayCast(RaycastHit2D hitTouch)
    {
        if (hitTouch && !startAnim && hitTouch.collider.name == transform.name)
        {
            StartCoroutine(AnimateLine());
            startAnim = true;
        }
    }

    Touch touch;
    RaycastHit2D hitTouch = new RaycastHit2D();
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            hitTouch = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0);

            SetRayCast(hitTouch);
        }
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            hitTouch = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(touch.deltaPosition).x, Camera.main.ScreenToWorldPoint(touch.deltaPosition).y), Vector2.zero, 0);

            SetRayCast(hitTouch);
        }
    }

    private IEnumerator makeHide()
    {
        startAnim = false;
        yield return new WaitForSeconds(visibleTime);
        resetColor();
    }

    private IEnumerator AnimateLine()
    {
        lineRenderer.enabled = true;
        startAnim = true;
        float segmentDuration = animationDuration / pointsCount;

        for (int i = 0; i < pointsCount - 1; i++)
        {
            float startTime = Time.time;

            Vector3 startPosition;
            Vector3 endPosition;

            startPosition = linePoints[i];
            endPosition = linePoints[i + 1];

            Vector3 pos = startPosition;
            while (pos != endPosition)
            {
                float t = (Time.time - startTime) / segmentDuration;
                pos = Vector3.Lerp(startPosition, endPosition, t);

                // animate all other points except point at index i
                for (int j = i + 1; j < pointsCount; j++)
                    lineRenderer.SetPosition(j, pos);

                yield return null;
            }
        }

        if(startAnim && visibleTime != 0)
            StartCoroutine(makeHide());
    }
}
