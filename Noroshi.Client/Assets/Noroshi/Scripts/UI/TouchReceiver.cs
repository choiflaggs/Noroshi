using UnityEngine;
using System.Collections;

public class TouchReceiver : MonoBehaviour {
    TouchData data = new TouchData();
    Vector3 startWorldPos;
    float startTime;
    int[] judgeDirection = new int[1];

    public void ReceiveStart(Vector3 worldPos, Vector3 screenPos, int index, int len) {
        startWorldPos = worldPos;
        startTime = Time.time;
        judgeDirection[0] = 0;

        data.startPosition = worldPos;
        data.worldPosition = worldPos;
        data.screenPosition = screenPos;
        data.distance = 0.0f;
        data.distanceX = 0.0f;
        data.distanceY = 0.0f;
        data.angle = 0.0f;
        data.direction = null;
        data.index = index;
        data.length = len;

        SendMessage("Touch", data, SendMessageOptions.DontRequireReceiver);
    }

    public void ReceiveEnd(Vector3 worldPos, Vector3 screenPos, int index, int len) {
        data.worldPosition = worldPos;
        data.screenPosition = screenPos;
        data.index = index;
        data.length = len;

        if(Time.time - startTime < 1.0f) {
            if(data.distance < 0.1f) {
                SendMessage("Tap", data, SendMessageOptions.DontRequireReceiver);
            } else {
                SendMessage("Swipe", data, SendMessageOptions.DontRequireReceiver);
            }
        }
        SendMessage("Release", data, SendMessageOptions.DontRequireReceiver);
    }
    
    public void ReceiveLeave(Vector3 worldPos, Vector3 screenPos, int index, int len) {
        data.worldPosition = worldPos;
        data.screenPosition = screenPos;
        data.index = index;
        data.length = len;

        SendMessage("Leave", data, SendMessageOptions.DontRequireReceiver);
    }

    public void ReceiveMove(Vector3 worldPos, Vector3 screenPos, int index, int len) {
        float disX = worldPos.x - startWorldPos.x;
        float disY = worldPos.y - startWorldPos.y;
        float angle = Mathf.Atan2(disY, disX) * 180 / Mathf.PI;

        if(judgeDirection[0] == 0 && (disX != 0 || disY != 0)) {
            if(angle >= 45 && angle <= 135) {
                judgeDirection[0] = 2;
                data.direction = "up";
            } else if(angle >= -135 && angle <= -45) {
                judgeDirection[0] = -2;
                data.direction = "down";
            } else if(angle > -45 && angle < 45) {
                judgeDirection[0] = 1;
                data.direction = "right";
            } else {
                judgeDirection[0] = -1;
                data.direction = "left";
            }
        }

        if(judgeDirection[0] != 0) {
            data.worldPosition = worldPos;
            data.screenPosition = screenPos;
            data.distance = Mathf.Sqrt(disX * disX + disY * disY);
            data.distanceX = disX;
            data.distanceY = disY;
            data.angle = angle;
            data.index = index;
            data.length = len;

            SendMessage("Drag", data, SendMessageOptions.DontRequireReceiver);
        }
    }
}

public class TouchData {
    public Vector3 startPosition;
    public Vector3 worldPosition;
    public Vector3 screenPosition;
    public float distance;
    public float distanceX;
    public float distanceY;
    public float angle;
    public string direction;
    public int index;
    public int length;
}
