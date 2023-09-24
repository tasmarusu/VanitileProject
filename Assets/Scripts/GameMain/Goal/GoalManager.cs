namespace VANITILE
{
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;

    /// <summary>
    /// ゴールマネージャー
    /// </summary>
    public class GoalManager : StageManagerBase
    {
        /// <summary>
        /// ゴール本体
        /// </summary>
        private List<GoalPart> goalParts = new List<GoalPart>();

        /// <summary>
        /// ゴール本体
        /// </summary>
        public List<GoalPart> GoalParts => this.goalParts;

        /// <summary>
        /// 初期化
        /// </summary>
        public override void Init()
        {
            this.goalParts.Clear();
            base.Init();
            this.CheckAbleGoal();
            this.CheckLookGoal();
        }

        /// <summary>
        /// ゴールの作成と配置
        /// </summary>
        protected override void CreateStageAndPlace()
        {
            var models = GameMain.Instance.CurrentStageData.Parts.FindAll(model => model.Type == DefineData.StagePartType.Goal);
            Debug.Log($"[Create]Goalmodels:{models.Count}");
            foreach (var model in models)
            {
                var part = GameObject.Instantiate(
                    model.Prefab,
                    model.Point,
                    Quaternion.identity,
                    this.transform).GetComponent<GoalPart>();

                this.goalParts.Add(part);
                part.Initialize();
            }
        }

        /// <summary>
        /// 見た目変更監視か
        /// </summary>
        private void CheckLookGoal()
        {
            // 全鍵取得後、ゴール可能へ
            this.ObserveEveryValueChanged(x => StageDataModel.Instance.IsChangeLookGoal())
                .Where(isChangeLook => isChangeLook)
                .First()
                .Subscribe(x =>
                {
                    this.goalParts.ForEach(x => x.StartChangeLooking());
                }).AddTo(this.goalParts[0]);
        }

        /// <summary>
        /// ゴール可能か監視
        /// </summary>
        private void CheckAbleGoal()
        {
            // 全鍵取得後、ゴール可能へ
            this.ObserveEveryValueChanged(x => StageDataModel.Instance.IsAbleGoal())
                .Where(isAbleGoal => isAbleGoal)
                .First()
                .Subscribe(x =>
                {
                    this.goalParts.ForEach(x => x.StartCheckHitPlayer());
                }).AddTo(this.goalParts[0]);
        }
    }
}