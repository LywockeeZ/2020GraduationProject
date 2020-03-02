using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockAtPlayer : MonoBehaviour
{
    public Transform target;
    public float moveSmooth = 5f;
    Vector3 offset;
    void Start() 
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        offset = (transform.position - target.position)/2;//获取相对位置
    }


    void Update()
    {
        Vector3 targetPostion = offset + target.position;
        transform.position = Vector3.Lerp(transform.position, targetPostion, moveSmooth * Time.deltaTime);
    }
}
