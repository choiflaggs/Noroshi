using Noroshi.WebApi;
using UniRx;
using UnityEngine;
using Noroshi.UI;

namespace Noroshi.Login
{
    public class PlayerIDRepository : MonoBehaviour
    {

        private void Awake()
        {
            WebApiRequester.Login()
            .SelectMany(_ => GlobalContainer.Load())
            .Do(_ =>
            {
                if(UILoading.Instance != null) {
                    UILoading.Instance.LoadScene(Constant.SCENE_MAIN);
                } else {
                    Application.LoadLevel(Constant.SCENE_MAIN);
                }
            }).Subscribe();
        }
    }
}