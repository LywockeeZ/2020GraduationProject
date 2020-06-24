
using System.Collections;

using System.Collections.Generic;

using UnityEngine;



/// <summary>

/// 物品浮动效果

/// </summary>

public class ItemFloating : MonoBehaviour

{

    //漂浮幅度

    public float speed = 1f;

    //漂浮范围X,Y,Z

    public Vector3 offsetXYZ = new Vector3(1, 1, 1);

    //原点

    private Vector3 originalPos;



    void Start()

    {

        if (speed <= 0)

        {

            speed = 1f;

        }

        //获取原点

        originalPos = transform.localPosition;

    }



    void Update()

    {

        //根据三角函数以时间为变化量模拟浮动效果(看个人喜好如何偏移但是乘上的值必须要在[-1,1]合理范围内变化)

        transform.localPosition = originalPos + new Vector3(offsetXYZ.x * Mathf.Sin(Time.time * speed), offsetXYZ.y * Mathf.Cos(Time.time * speed) * Mathf.Sin(Time.time * speed), offsetXYZ.z * Mathf.Cos(Time.time * speed));

    }

}
