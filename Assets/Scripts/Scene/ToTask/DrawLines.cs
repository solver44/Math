using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLines : MonoBehaviour
{
    //Public Variables
    public bool SpecialPositions = true; 
    public bool StartFromFirstObject = true;
    public GameObject[] Objects;

    //Private Variables
    private LineRenderer line;
    private Vector3 mousePos;
    public Material material;
    private int currLines = 0;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            if (line == null)
            {
                createLine();
            }

            line.SetPosition(0, mousePos);
            line.SetPosition(1, mousePos);
        }
        else if (Input.GetMouseButtonUp(0) && line)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            line.SetPosition(1, mousePos);
            line = null;
            currLines++;
        }
        else if (Input.GetMouseButton(0) && line)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            line.SetPosition(1, mousePos);
        }
    }

    void createLine()
    {
        line = new GameObject("Line" + currLines).AddComponent<LineRenderer>();
        line.transform.parent = this.transform;
        line.transform.position = new Vector3(line.transform.position.x, line.transform.position.y, -1);
        line.material = material;
        line.positionCount = 2;
        line.startWidth = 0.15f;
        line.endWidth = 0.15f;
        line.useWorldSpace = false;
        line.numCapVertices = 50;
    }
}
