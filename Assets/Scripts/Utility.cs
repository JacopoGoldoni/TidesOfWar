using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    private static Camera _camera;
    public static Camera Camera
    {
        get
        {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        }
    }

    public static Vector2 V3toV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.z);
    }

    public static Vector3 V2toV3(Vector2 v2)
    {
        return new Vector3(v2.x, 0, v2.y);
    }
}
