using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollision2D : MonoBehaviour
{
    private bool _onCollision = false;
    public bool OnCollision
    {
        get { return _onCollision; }
    }

    private string _nameOfObject = "";
    private string _nameOfTrggeredObject = "";
    public string NameOfObject
    {
        get { return _nameOfObject; }
    }
    public string NameOfTriggeredObject
    {
        get { return _nameOfTrggeredObject; }
    }

    void Awake()
    {
        _nameOfObject = this.transform.name;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        _onCollision = true;
        _nameOfTrggeredObject = coll.gameObject.name;
    }
    void OnTriggerExit2D(Collider2D coll)
    {
        _onCollision = false;
    }
}
