﻿using UnityEngine;
using System.Collections;

public class TweenSY : TweenVec1S
{
    public static TweenSY Add(GameObject g, float duration, float to)
    {
        return Add<TweenSY>(g, duration, to);
    }
    
    override public float value
    {
        get
        {
            return vector.y;
        }
        set
        {
            Vector3 v = vector;
            v.y = value;
            vector = v;
        }
    }
}
