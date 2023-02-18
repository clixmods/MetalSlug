using UnityEngine;

public static class CameraUtility
{
    private static Camera _camera;
    public static Camera Camera
    {
        get
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
            return _camera;
        }
    }
    const float ValueNegative = 0f;
    const float ValuePositive = 1f;
    
    public static bool IsOutOfCameraVision(this Transform transform, float toleranceMin = ValueNegative, float toleranceMax = ValuePositive)
    {
        return IsOutOfCameraVision(transform.position,toleranceMin,  toleranceMax);
    }
    public static Vector3 GetPositionInWorldToViewportPointCamera(this Transform transform)
    {
        return Camera.WorldToViewportPoint(transform.position);
    }
    public static Vector3 GetPositionInWorldToViewportPointCamera(this Vector3 position)
    {
        return Camera.WorldToViewportPoint(position);
    }
    public static Vector3 GetPositionInWorldToScreenPoint(this Transform transform)
    {
        return Camera.WorldToScreenPoint(transform.position);
    }
    public static Vector3 GetPositionInWorldToScreenPoint(this Vector3 position)
    {
        return Camera.WorldToScreenPoint(position);
    }
 
    public static bool IsOutOfCameraVision(this Vector3 positionOfTarget ,float toleranceMin = ValueNegative, float toleranceMax = ValuePositive)
    {
        // Check if the object is out of the camera
        var isOutCameraNegative = IsOutCameraNegative(positionOfTarget,toleranceMin);
        var isOutCameraPositive = IsOutCameraPositive(positionOfTarget,toleranceMax);
        if (isOutCameraNegative || isOutCameraPositive)
        { 
            return true;
        }
        return false;
    }
    public static bool IsOutCameraPositive(this Vector3 position, float valuePositive)
    {
        return position.GetPositionInWorldToViewportPointCamera().x > valuePositive || position.GetPositionInWorldToViewportPointCamera().y > valuePositive;
    }
    public static bool IsOutCameraNegative(this Vector3 position, float valueNegative)
    {
        return position.GetPositionInWorldToViewportPointCamera().x < valueNegative || position.GetPositionInWorldToViewportPointCamera().y < valueNegative;
    }
}