using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokeEvent : MonoBehaviour
{
    public Rigidbody[] parts;
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

    public void Broken()
    {
        foreach (var item in parts)
        {
            item.isKinematic = false;
            item.useGravity = true;
        }
        StartCoroutine(destroySelf());

    }

    IEnumerator destroySelf()
    {
        yield return new WaitForSeconds(5f);
        foreach (var item in parts)
        {
            GameObject.Destroy(item.gameObject);
        }
        StopCoroutine(destroySelf());
    }
}
