﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    static new UnityEngine.Camera camera;
    void Start()
    {
        camera = UnityEngine.Camera.main;
    }
    public static Vector3 GetCameraForwardDirectionNormalized()
    {
        Vector3 forward = camera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }
    public static Vector3 GetCameraRightDirectionNormalized()
    {
        Vector3 right = camera.transform.right;
        right.y = 0;
        return right.normalized;
    }
}
