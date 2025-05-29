using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysterySpinning : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0,150*Time.deltaTime,0);
    }
}
