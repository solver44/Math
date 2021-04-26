using UnityEngine;

public class MoveBG : MonoBehaviour
{
    //Move
    [SerializeField]
    private float speedMove = 0;

    private float startPosY;

    [SerializeField]
    private float range = 0;
    [SerializeField]
    private bool toTop = true;
    
    private float moveS;

    //Rotate
    [SerializeField]
    private bool rotate = false;
    [SerializeField]
    private float rotationleft = 360;
    [SerializeField]
    private float rotationspeed = 10;

    void Start()
    {
        startPosY = this.transform.localPosition.y;
    }

    void Update()
    {
        if (rotate)
        {
            float rotation = rotationspeed * Time.deltaTime;
            if (rotationleft > rotation)
            {
                rotationleft -= rotation;
            }
            else
            {
                rotation = rotationleft;
                rotationleft = 0;
            }
            transform.Rotate(0, 0, rotation);
        }
        else
        {
            if (toTop)
            {
                if (this.transform.localPosition.y <= startPosY)
                    moveS = speedMove * 0.1f;
                if (this.transform.localPosition.y >= startPosY + range)
                    moveS = -speedMove * 0.1f;
            }
            else
            {
                if (this.transform.localPosition.y >= startPosY)
                    moveS = -speedMove * 0.1f;
                if (this.transform.localPosition.y <= startPosY - range)
                    moveS = speedMove * 0.1f;
            }
        }

        this.transform.Translate(Vector3.up * moveS * Time.deltaTime);
    }

}
