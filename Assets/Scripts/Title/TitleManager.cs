using UnityEngine;
using UniRx;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace VANITILE
{
    /// <summary>
    /// タイトルマネージャー
    /// </summary>
    public class TitleManager : MonoBehaviour
    {
        /// <summary>
        /// ParentCanvas
        /// </summary>
        [SerializeField] private Transform parent = null;

        /// <summary>
        /// TitleSelectController
        /// </summary>
        [SerializeField] private TitleSelectController titleSelectController = null;

        /// <summary>
        /// OptionManager
        /// </summary>
        [SerializeField] private OptionManager optionManager = null;

        /// <summary>
        /// 表示中オブジェクト
        /// </summary>
        private GameObject appearingObj = null;

        /// <summary>
        /// タイトル画面の開始
        /// </summary>
        private void Start()
        {
            StageDataModel.Instance.Release();

            InputManager.Instance.StartVerticalSubject();

            //this.optionManager.gameObject.SetActive(false);
            this.titleSelectController.Init();

            this.StartCoroutine(this.CheckDecideSelect());
        }

        /// <summary>
        /// 選択画面での決定
        /// </summary>
        private IEnumerator CheckDecideSelect()
        {
            // TODO:あとでけす
            yield return null;

            this.titleSelectController.SelectSubject.Subscribe(type =>
            {
                switch (type)
                {
                    case DefineData.TitleSelectType.Start:
                        Debug.Log($"[TitleSelect]{type.ToString()} が選択されました");
                        StageDataModel.Instance.CurrentStageId = 0;
                        SceneManager.LoadScene(DefineData.SceneName.GameMainScene.ToString());
                        break;

                    case DefineData.TitleSelectType.Continue:
                        Debug.Log($"[TitleSelect]{type.ToString()} が選択されました");
                        // TDOO:クリアステージ数の保存をする
                        StageDataModel.Instance.CurrentStageId = 0;
                        SceneManager.LoadScene(DefineData.SceneName.GameMainScene.ToString());
                        break;

                    case DefineData.TitleSelectType.StageSelect:
                        Debug.Log($"[TitleSelect]{type.ToString()} が選択されました");
                        break;

                    case DefineData.TitleSelectType.Option:
                        var obj = GameObject.Instantiate(this.optionManager.gameObject, this.parent);
                        obj.GetComponent<OptionManager>().Init();

                        Debug.Log($"[TitleSelect]{type.ToString()} が選択されました");
                        break;

                    case DefineData.TitleSelectType.Exit:
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