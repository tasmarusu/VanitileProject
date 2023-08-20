namespace VANITILE
{
    using UnityEngine;

    /// <summary>
    /// ステージ遷移
    /// ステージ作成シーンでの使用を兼ねて、Monoを継承しない
    /// </summary>
    public class ChangeStage
    {
        /// <summary>
        /// MonoBehaviour
        /// </summary>
        private MonoBehaviour mono;

        /// <summary>
        /// 遷移先の保存データ
        /// </summary>
        private StageTransitionScriptable.Data scriptable;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mono">MonoBehaviour</param>
        public ChangeStage(MonoBehaviour mono)
        {
            this.mono = mono;
        }

        /// <summary>
        /// StageDataManager
        /// </summary>
        public StageSaveData.Data CurrentStageData { get; private set; }

        /// <summary>
        /// 指定シーンの情報へ変える
        /// </summary>
        public void Transition(StageTransitionScriptable.Data data)
        {
            this.DeleteAllPartObjects();

            // ラスト3文字が数字の決まりなのでそのデータを取得
            var stageId = int.Parse(data.Json.name.Substring(data.Json.name.Length - 3));
            Debug.Log($"[Transition]Load:{data.Json.name} StageId:{stageId}");
            this.LoadStageData(stageId);
        }

        /// <summary>
        /// ステージデータのロード
        /// </summary>
        private void LoadStageData(int stageId)
        {
            var stageData = new StageSaveData();
            this.CurrentStageData = stageData.Load(stageId);

            if (this.CurrentStageData == default)
            {
                Debug.LogError($"[Load]ステージ情報取得失敗");
            }

            // ステージを配置していく
            // TODO:ここで生成もありだが、各々のManagerがやった方が今後の拡張性ありそうだから。無いかもだが…
            // やっぱりここに生成書くべきかも
            ////foreach (var part in this.StageData.Parts)
            ////{
            ////    var parent = this.managers.Find(x => x.PartTypes.Contains(part.Type)).transform;
            ////    GameObject.Instantiate(part.Prefab, part.Point, Quaternion.identity, parent);
            ////}
        }

        /// <summary>
        /// 地形用オブジェクトの全削除
        /// </summary>
        private void DeleteAllPartObjects()
        {
            // 現在配置されているステージを削除
            var commonParts = this.mono.GetComponentsInChildren<CommonPart>();
            for (int i = commonParts.Length - 1; i >= 0; i--)
            {
                GameObject.Destroy(commonParts[i].gameObject);
            }
        }
    }
}