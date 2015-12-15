using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;

public class StressController : MonoBehaviour {
    [SerializeField] GameObject[] charaList;
    [SerializeField] BtnCommon btn;
    [SerializeField] Text txtCharaNum;

    int charaNum = 0;

    void Start() {
        btn.OnClickedBtn.Subscribe(id =>
        {
            var index = 7;//Random.Range(0, charaList.Length);
            var px = Random.Range(0f, Screen.width);
            var py = Random.Range(0f, Screen.height - 100);
            var np = Camera.main.ScreenToWorldPoint(new Vector3(px, py, 9));
            charaNum++;
            Instantiate(charaList[index], np, Quaternion.identity);
            txtCharaNum.text = charaNum.ToString();
        });
    }
}
