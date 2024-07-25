using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline;
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
    
    public static int IndexFromMask(int mask)
    {
        for (int i = 0; i < 32; i++)
        {
            if ((1 << i) == mask)
            {
                return i;
            }
        }
        return -1;
    }

    public static bool IsInView(GameObject gameObject)
    {
        Transform viewTransform = Camera.transform;
        
        if( Vector3.Dot( gameObject.transform.position - viewTransform.position, viewTransform.forward) > 0)
        {
            return true;
        }
        return false;
    }
}

public static class SFXUtility
{
    private static Dictionary<string, AudioClip> Audios = new Dictionary<string, AudioClip>()
    {
        { "MusketFire1", Resources.Load<AudioClip>("SFX/MusketFire1")},
        { "MusketFire2", Resources.Load<AudioClip>("SFX/MusketFire2")},
        { "MusketFire3", Resources.Load<AudioClip>("SFX/MusketFire3")}
    };

    public static AudioClip GetAudio(string clipName)
    {
        return Audios[clipName];
    }
}
