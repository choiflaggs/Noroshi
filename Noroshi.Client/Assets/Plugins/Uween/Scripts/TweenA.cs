using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TweenA : TweenVec1
{
    public static TweenA Add(GameObject g, float duration, float to)
    {
        return Add<TweenA>(g, duration, to);
    }

    Graphic g;
    CanvasGroup cg;

    protected Graphic GetGraphic()
    {
        if (g == null) {
            g = GetComponent<Graphic>();
        }
        return g;
    }

    protected CanvasGroup GetCanvasGroup()
    {
        if (cg == null) {
            cg = GetComponent<CanvasGroup>();
        }
        return cg;
    }
    
    override public float value
    {
        get
        {
            CanvasGroup cg = GetCanvasGroup();
            float a = cg != null ? cg.alpha : GetGraphic().color.a;
            return a;
        }
        set
        {
            CanvasGroup cg = GetCanvasGroup();
            if(cg != null) {
                cg.alpha = value;
            } else {
                Graphic g = GetGraphic();
                Color c = g.color;
                c.a = value;
                g.color = c;
            }
        }
    }
}