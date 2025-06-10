using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStartMenu : MonoBehaviour
{
    public float height = 0.5f;     
    public float rate = 1f;      
    public Vector3 direction = Vector3.up; 
    public Vector3 startPos;     

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        FloatMovement();
    }

    void FloatMovement()
    {
        float offset = Mathf.Sin(Time.time * rate) * height;
        transform.position = startPos + direction * offset;
    }
}
