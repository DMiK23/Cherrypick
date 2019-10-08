using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldInfoScript : MonoBehaviour //class that manages the fields
{
    public byte _fieldType  = 0; //0 - free, 1 - wall, 2 - start, 3 - end
    public int _pathWeight  = int.MaxValue; //path weight (how far it is from the start field)
    public FieldInfoScript _previousField = null; //ref to a field closer to the start field

    public void Awake()//colors the board in the chekes pattern
    {
        if ((this.gameObject.transform.position.x + this.gameObject.transform.position.z) % 2 == 0)
            this.gameObject.GetComponent<Renderer>().material.color = Color.black;
        else
            this.gameObject.GetComponent<Renderer>().material.color = Color.white;
    }

    public void SetAsStart()//sets field as the start field with all the relevant values
    {
        _fieldType = 2;
        _pathWeight = 0;
        this.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
    }

    public void SetAsEnd()//sets field as the end field with all the relevant values
    {
        _fieldType = 3;
        this.gameObject.GetComponent<Renderer>().material.color = Color.red;
    }

    public bool SetAsObstacle()//sets field as the obstale field with all the relevant values
    {
        if (_fieldType == 2 || _fieldType == 3) return false;//if the field is not start or end
        _fieldType = 1;
        this.gameObject.GetComponent<Renderer>().material.color = Color.blue;
        return true;
    }

    public void SetAsPath()//colors the field as path
    {
        this.gameObject.GetComponent<Renderer>().material.color = Color.cyan;
    }
}
