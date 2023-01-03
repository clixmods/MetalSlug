using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Transform lookAt;

    public float boundX = 2.0f;
    public float boundY = 1.5f;
    public float speed = 0.15f;

    private Vector3 desiredPosition;

    // Clamp code

    // the transform of the camera
    public Transform cameraTransform;
    // the left limit
    public float leftBoundary;
    // the right limit
    public float rightBoundary;

    private void Update()
    {
        // stock the pos of the camera in a vector3
        Vector3 targetPosition = cameraTransform.position;

        // clamp the camera position so it doesnt go beyond the left and right limits
        targetPosition.x = Mathf.Clamp(targetPosition.x, leftBoundary, rightBoundary);

        transform.position = targetPosition;
    }

    private void LateUpdate()
    {
        Vector3 delta = Vector3.zero;

        float dx = lookAt.position.x - transform.position.x;
        // X Axis
        if (dx > boundX || dx < -boundX)
        {
            if (transform.position.x < lookAt.position.x)
            {
                delta.x = dx - boundX;
            }
            else
            {
                delta.x = dx + boundX;
            }
        }

        float dy = lookAt.position.y - transform.position.y;
        // Y Axis
        if (dy > boundY || dy < -boundY)
        {
            if (transform.position.y < lookAt.position.y)
            {
                delta.y = dy - boundY;
            }
            else
            {
                delta.y = dy + boundY;
            }
        }

        // Move the camera
        desiredPosition = transform.position + delta;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, speed);
    }
}