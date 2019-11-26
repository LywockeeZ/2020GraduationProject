using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokeEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Destroy()
    {
        GameObject.Destroy(transform.parent.gameObject);
    }
}
