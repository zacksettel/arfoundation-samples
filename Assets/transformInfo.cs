using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transformInfo : MonoBehaviour
{

    [SerializeField] bool debug = false;

    // Start is called before the first frame update
    void Start()
    {
        Transform t = transform;

        Debug.Log("TRANSFORM INFO for: "+name);
        int i = 1;

        while (t = t.parent)
        {
            Debug.Log("     parent_"+ i + " "+t.name);
            i++;
        }
         
    }

    // Update is called once per frame
    void Update()
    {
        if (debug)
            Debug.Log($"{GetType()}.Update(): eulers: { transform.rotation.eulerAngles}");
    }
}
