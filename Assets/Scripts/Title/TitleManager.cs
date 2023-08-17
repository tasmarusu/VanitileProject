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
        [SerializeField] private OptionManager optionManagerPrefab = null;

        /// <summary>
        /// StageSelectManager
        /// </summary>
        [SerializeField] private StageSelectManager stageSelectManagerPrefab = null;

        /// <summary>
        /// CompositeDisposable
        /// </summary>
        private CompositeDisposable disposables = new CompositeDisposable();

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

            this.disposables.Add(InputManager.Instance.StartVerticalSubject());

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
                Debug.Log($"[TitleSelect]{type.ToString()} が選択されました");

                switch (type)
                {
                    case TitleSelectType.Start:
                        StageDataModel.Instance.CurrentStageId = 0;
                        SceneManager.LoadScene(SceneName.GameMainScene.ToString());
                        break;

                    case TitleSelectType.Continue:
                        StageDataModel.Instance.CurrentStageId = GameSaveDataModel.Instance.PlayLastStageId;
                        SceneManager.LoadScene(SceneName.GameMainScene.ToString());
                        break;

                    case TitleSelectType.StageSelect:
                        var select = GameObject.Instantiate(this.stageSelectManagerPrefab.gameObject, this.parent).GetComponent<StageSelectManager>();
                        select.Init();
                        this.StartCoroutine(this.StartBackSelect(select, type));
                        break;

                    case TitleSelectType.Option:
                        var option = GameObject.Instantiate(this.optionManagerPrefab.gameObject, this.parent).GetComponent<OptionManager>();
                        option.Init();
                        this.StartCoroutine(this.StartBackSelect(option, type));
                        break;

                    case TitleSelectType.Exit:
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
            yield return null;  // 反映まで1f待つが、Animation制御したらいらなくなるかも知れない
            this.titleSelectController.RestartInput();
        }

        /// <summary>
        /// 破棄時
        /// </summary>
        private void OnDestroy()
        {
            this.disposables?.Clear();
        }
    }
}