using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    Vector3 FirstPoint;
    Vector3 SecondPoint;
    float xAngle;
    float yAngle;
    float xAngleTemp;
    float yAngleTemp;

    public Transform Player;

    [HideInInspector]
    public bool isCanRotate = true;
    private bool isMouseDown = false;

    void Start()
    {
        transform.rotation = Quaternion.Euler(yAngle, xAngle, 0);
        transform.parent.position = Player.transform.position;
    }

    void Update()
    {
        transform.parent.position = Vector3.Lerp(transform.parent.position, Player.position, Time.deltaTime * 10f);

        if (isCanRotate != false)
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    FirstPoint = Input.GetTouch(0).position;
                    xAngleTemp = xAngle;
                    yAngleTemp = yAngle;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    SecondPoint = Input.GetTouch(0).position;
                    xAngle = xAngleTemp + (SecondPoint.x - FirstPoint.x) * 180 / Screen.width;
                    yAngle = yAngleTemp - (SecondPoint.y - FirstPoint.y) * 90 * 3f / Screen.height; // Y값 변화가 좀 느려서 3배 곱해줌.
 
                    // 회전값을 -85~85로 제한
                    if (yAngle < -85f)
                        yAngle = 85f;
                    if (yAngle > 85f)
                        yAngle = 85f;
 
                    transform.rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);
                }
            }
#else
            // 마우스가 눌림
            if (Input.GetMouseButtonDown(0))
            {
                FirstPoint = Input.mousePosition;
                xAngleTemp = xAngle;
                yAngleTemp = yAngle;
                isMouseDown = true;
            }

            // 마우스가 떼짐
            if (Input.GetMouseButtonUp(0))
            {
                isMouseDown = false;
            }

            if (isMouseDown)
            {
                SecondPoint = Input.mousePosition;
                xAngle = xAngleTemp + (SecondPoint.x - FirstPoint.x) * 180 / Screen.width;
                yAngle = yAngleTemp - (SecondPoint.y - FirstPoint.y) * 90 * 3f / Screen.height; // Y값 변화가 좀 느려서 3배 곱해줌.

                // 회전값을 -85~85로 제한
                if (yAngle < -85f)
                    yAngle = -85f;
                if (yAngle > 85f)
                    yAngle = 85f;

                transform.rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);
            }
#endif
        }
    }

}