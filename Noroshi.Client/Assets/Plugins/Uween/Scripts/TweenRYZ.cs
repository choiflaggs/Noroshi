﻿using UnityEngine;
using System.Collections;

public class TweenRYZ : TweenVec2R
{
    public static TweenRYZ Add(GameObject g, float duration, Vector2 to)
    {
        return Add<TweenRYZ>(g, duration, to);
    }
    
    public static TweenRYZ Add(GameObject g, float duration, float toRY, float toRZ)
    {
        return Add<TweenRYZ>(g, duration, toRY, toRZ);
    }
    
    public static TweenRYZ Add(GameObject g, float duration, float toRYZ)
    {
        return Add(g, duration, toRYZ, toRYZ);
    }
    
    override public Vector2 value
    {
        get
        {
            return new Vector2(vector.y, vector.z);
        }
        set
        {
            Vector3 v = vector;
            v.y = value.x;
            v.z = value.y;
            vector = v;
        }
    }
}
