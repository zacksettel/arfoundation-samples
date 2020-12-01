using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transformInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Transform t = transform;

        while (t = t.parent)
        {
            Debug.Log("TRANSFORM INFO: parents: " + t.name);
        }
         
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
