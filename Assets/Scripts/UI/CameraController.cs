using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float rotationDegrees = 10;
    [SerializeField] float scrollSpeed = 1;
    [SerializeField] float maxZoom = 25;
    [SerializeField] float minZoom = 10;
    [SerializeField] float zoom = 16;
    [SerializeField] CinemachineVirtualCamera vcam = null;
    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void LateUpdate()
    {
        transform.position = player.transform.position;
        CinemachineComponentBase componentBase = vcam.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (componentBase is CinemachineFramingTransposer)
        {
            (componentBase as CinemachineFramingTransposer).m_CameraDistance = zoom; // your value
        }
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            transform.RotateAround(transform.position, Vector3.up, Input.GetAxis("Mouse X") * rotationDegrees);
        }
        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            zoom = Mathf.Min(maxZoom, zoom + scrollSpeed);
        }
        else if (d < 0f)
        {
            zoom = Mathf.Max(minZoom, zoom - scrollSpeed);
        }
        // Full x & y axis control
        // if (Input.GetMouseButton(1))
        // {
        //     float h = rotationDegrees * Input.GetAxis("Mouse X");
        //     float v = rotationDegrees * Input.GetAxis("Mouse Y");

        //     transform.Rotate(-v, h, 0);

        //     float z = transform.eulerAngles.z;
        //     transform.Rotate(0, 0, -z);
        // }
    }
}