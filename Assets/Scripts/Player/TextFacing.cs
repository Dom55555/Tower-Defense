using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFacing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180f, 0);
    }
}
