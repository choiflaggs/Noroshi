using UnityEngine;
using System.Collections;

public class EditPolygonCollider : MonoBehaviour {
    public Vector2[] polyPoints = new Vector2[3]; // Inspectorに表示させて、頂点座標を入力するためのVector2配列
    private PolygonCollider2D mPolygonCollider2D;
    private PolygonCollider2D polygonCollider2D {
        get { // 初回のみGetComponentする
            if (mPolygonCollider2D == null) {
                mPolygonCollider2D = gameObject.GetComponent<PolygonCollider2D>();
            }
            return mPolygonCollider2D;
        }
    }
    
    void OnValidate() { // Inspectorの値が変更される度に呼ばれる関数
        polygonCollider2D.points = polyPoints; // Inspectorに入力した座標をsetする
    }
}