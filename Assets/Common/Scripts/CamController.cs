using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    public GameObject central;
    float x, y;
    public float xspeed = 50f;
    public float yspeed = 50f;
    public float zoomSpeed = 50f;                                           //转动滑轮时缩放的距离
    public float zoomTime = 10f;                                             //缩放所用时间
    public float rotationTime = 5f;
    public float defaultDistance = 10f;
    public float distance;
    Quaternion rotation;
    Vector3 position;
    Vector3 velocity = Vector3.one;                                         //延迟缩放时限定的速度
                                                                            //状态判断变量
    public bool isRotating = false;
    public bool isZooming = true;

    private Vector2 lastPosition;
    private Vector2 currentPosition;

    void Start()
    {
        distance = defaultDistance;
        GetCurrentRotation();
    }


    private void Update()
    {
        if (central == null || !central.activeInHierarchy)
        {
            central = GameObject.FindGameObjectWithTag("Player");
        }
            if (Input.GetMouseButton(1))
            {
                isRotating = true;
                isZooming = false;
                x += Input.GetAxis("Mouse X") * Time.deltaTime * xspeed;
                y -= Input.GetAxis("Mouse Y") * Time.deltaTime * yspeed;

            }

            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                isRotating = false;
                isZooming = true;
                //滑轮转动越快，GetAxis返回值越大
                float scrollSpeed = Input.GetAxis("Mouse ScrollWheel");
                distance -= scrollSpeed * zoomSpeed;
                distance = Mathf.Clamp(distance, 3f, 8f);

            }

        if (central!=null)
        {
            SetFreeViewPosition();
        }
    }

    void GetCurrentRotation()
    {
        x = Camera.main.transform.localRotation.eulerAngles.y;
        y = Camera.main.transform.localRotation.eulerAngles.x;
    }

    void SetFreeViewPosition()
    {

        //因为运用了平滑函数，为了使延迟缩放不产生BUG，必须分状态进行
        //为了平滑函数的持续调用，需要使用状态切换给函数留出一个时间差，以完成延迟的缩放和转动
        //以Zooming状态开始可以通过缩放调整误差
        if (isRotating)
        {
            //Slerp用来延迟转动
            rotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(20, x, 0), Time.deltaTime * rotationTime);
            y = Mathf.Clamp(y, 25f, 50f);
            rotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(y, x, 0), Time.deltaTime * rotationTime);
            //rotation = Quaternion.Euler(y, x, 0);
            position = rotation * Vector3.back * distance + central.transform.position;
            transform.localRotation = rotation;
            transform.localPosition = position;
        }

        if (isZooming)
        {
            //SmoothDamp用来延迟缩放
            position = transform.localRotation * Vector3.back * distance + central.transform.position;
                                                                         //MainCamera.transform.localRotation = rotation;
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, position, ref velocity, zoomTime * Time.deltaTime);
        }


    }
}
