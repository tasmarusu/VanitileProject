using UnityEngine;
using UniRx;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace VANITILE
{
    /// <summary>
    /// タイトルマネージャー
    /// </summary>
    public class TitleManager : MonoBehaviour
    {
        /// <summary>
        /// TitleSelectController
        /// </summary>
        [SerializeField] private TitleSelectController titleSelectController = null;

        /// <summary>
        /// タイトル画面の開始
        /// </summary>
        private void Start()
        {
            StageDataModel.Instance.Release();

            this.titleSelectController.Init();
            this.CheckDecideSelect();
        }

        /// <summary>
        /// 選択画面での決定
        /// </summary>
        private void CheckDecideSelect()
        {
            this.titleSelectController.SelectSubject.Subscribe(type =>
            {
                switch (type)
                {
                    case TitleSelectController.TitleSelectType.Start:
                        Debug.Log($"[TitleSelect]{type.ToString()} が選択されました");
                        StageDataModel.Instance.CurrentStageId = 0;
                        SceneManager.LoadScene(DefineData.SceneName.GameMainScene.ToString());
                        break;

                    case TitleSelectController.TitleSelectType.Continue:
                        Debug.Log($"[TitleSelect]{type.ToString()} が選択されました");
                        // TDOO:クリアステージ数の保存をする
                        StageDataModel.Instance.CurrentStageId = 0;
                        SceneManager.LoadScene(DefineData.SceneName.GameMainScene.ToString());
                        break;

                    case TitleSelectController.TitleSelectType.StageSelect:
                        Debug.Log($"[TitleSelect]{type.ToString()} が選択されました");
                        break;

                    case TitleSelectController.TitleSelectType.Option:
                        Debug.Log($"[TitleSelect]{type.ToString()} が選択されました");
                        break;

                    case TitleSelectController.TitleSelectType.Exit:
                        Debug.Log($"[TitleSelect]{type.ToString()} が選択されました");
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#elif !UNITY_EDITOR
                        UnityEngine.Application.Quit();
#endif
                        break;

                    default:
                        Debug.LogError($"[TitleSelect]設定忘れか？");
                        break;
                }
            }).AddTo(this);
        }
    }
}