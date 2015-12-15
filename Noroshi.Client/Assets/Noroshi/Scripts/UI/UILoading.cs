using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;

namespace Noroshi.UI {
    public class UILoading : MonoBehaviour {
        [SerializeField] GameObject loading;
        [SerializeField] GameObject loadingBar;

        public static UILoading Instance;

        public List<string> HistoryList {
            get { return _historyList; }
        }

        private static readonly List<string> _historyList = new List<string>();
        private static Dictionary<string, int> queryList = new Dictionary<string, int>();
        private static Dictionary<string, List<int>> multiQueryList = new Dictionary<string, List<int>>();
        private AsyncOperation async;
        private string sceneName;

        private void Awake() {
            if(Instance == null) {Instance = this;}
            loading.SetActive(true);
        }

        private IEnumerator ChangeScene() {
            loading.SetActive(true);
            TweenA.Add(loading, 0.01f, 1);
            Resources.UnloadUnusedAssets();
            async = Application.LoadLevelAsync(sceneName);
            async.allowSceneActivation= false;

            while(async.progress < 0.9f) {
                loadingBar.transform.localScale = new Vector3(async.progress, 1, 1);
                yield return new WaitForEndOfFrame();
            }
            HistoryList.Add(Application.loadedLevelName);
            async.allowSceneActivation = true;
        }

        public void LoadScene(string scene) {
            sceneName = scene;
            StartCoroutine("ChangeScene");
        }

        public void SetLoadBtn(BtnLoadLevel btn) {
            btn.OnChangeLevel.Subscribe(name => {
                sceneName = name;
                StartCoroutine("ChangeScene");
            }).AddTo(btn);
        }

        public void ShowLoading() {
            if(loading != null) {
                loading.SetActive(false);
                loading.SetActive(true);
            }
        }

        public void HideLoading() {
            TweenA.Add(loading, 0.2f, 0).Then(() => {
                loading.SetActive(false);
            });
        }

        public bool GetLoadingEnd() {
            return !loading.activeSelf;
        }

        public void ResetHistory() {
            HistoryList.Clear();
        }

        public int GetQuery(string key) {
            return queryList.ContainsKey(key) ? queryList[key] : -1;
        }

        public void SetQuery(string key, int value) {
            queryList[key] = value;
        }

        public List<int> GetMultiQuery(string key) {
            return multiQueryList.ContainsKey(key) ? multiQueryList[key] : new List<int>();
        }

        public void SetMultiQuery(string key, List<int> values) {
            multiQueryList[key] = values;
        }

        public void RemoveQuery(string key) {
            if(queryList.ContainsKey(key)) {
                queryList.Remove(key);
            }
            if(multiQueryList.ContainsKey(key)) {
                multiQueryList.Remove(key);
            }
        }

        public void ClearQuery() {
            queryList.Clear();
            multiQueryList.Clear();
        }
    }
}
