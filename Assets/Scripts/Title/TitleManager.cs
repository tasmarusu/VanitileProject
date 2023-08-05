using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DefineData;

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
            TitleDataModel.Instance.Release();
            StageDataModel.Instance.Release();

            InputManager.Instance.StartVerticalSubject();

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
                    case TitleSelectType.Start:
                        Debug.Log($"[TitleSelect]{type.ToString()} が選択されました");
                        StageDataModel.Instance.CurrentStageId = 0;
                        SceneManager.LoadScene(SceneName.GameMainScene.ToString());
                        break;

                    case TitleSelectType.Continue:
                        Debug.Log($"[TitleSelect]{type.ToString()} が選択されました");
                        // TDOO:クリアステージ数の保存をする
                        StageDataModel.Instance.CurrentStageId = 0;
                        SceneManager.LoadScene(SceneName.GameMainScene.ToString());
                        break;

                    case TitleSelectType.StageSelect:
                        Debug.Log($"[TitleSelect]{type.ToString()} が選択されました");
                        break;

                    case TitleSelectType.Option:
                        var option = GameObject.Instantiate(this.optionManager.gameObject, this.parent).GetComponent<OptionManager>();
                        option.Init();
                        this.StartCoroutine(this.StartBackSelect(option, type));

                        Debug.Log($"[TitleSelect]{type.ToString()} が選択されました");
                        break;

                    case TitleSelectType.Exit:
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

        /// <summary>
        /// セレクト選択画面に戻る処理
        /// </summary>
        /// <returns>IEnumerator</returns>
        private IEnumerator StartBackSelect(TitleSelectBase select, TitleSelectType type)
        {
            yield return select.Finalize();
            this.titleSelectController.SetEventSelectedState(type);
        }
    }
}