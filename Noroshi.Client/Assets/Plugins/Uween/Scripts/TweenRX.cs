﻿using UnityEngine;
using System.Collections;

public class TweenRX : TweenVec1R
{
    public static TweenRX Add(GameObject g, float duration, float to)
    {
        return Add<TweenRX>(g, duration, to);
    }
    
    override public float value
    {
        get
        {
            return vector.x;
        }
        set
        {
            Vector3 v = vector;
            v.x = value;
            vector = v;
        }
    }
}
