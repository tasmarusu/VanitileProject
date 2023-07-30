using UnityEngine;

/// <summary>
/// プレイヤーデータスクリプタ
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerDataScriptable", order = 3)]
public class PlayerDataScriptable : ScriptableObject
{
    /// <summary>
    /// 移動速度
    /// </summary>
    [SerializeField, Header("移動速度")] private float moveSpeed = .0f;

    /// <summary>
    /// 重力最高速
    /// </summary>
    [SerializeField, Header("重力最高速")] private float gravityMaxSpeed = .0f;

    /// <summary>
    /// 上ジャンプ力
    /// </summary>
    [SerializeField, Header("上ジャンプ力")] private float jumpUpPower = .0f;

    /// <summary>
    /// 壁ジャンプ上
    /// </summary>
    [SerializeField, Header("壁ジャンプ上 力")] private float jumpWallUpPower = .0f;

    /// <summary>
    /// 壁ジャンプ横
    /// </summary>
    [SerializeField, Header("壁ジャンプ横 力")] private float jumpWallSidePower = .0f;

    /// <summary>
    /// 壁ジャンプ無操作時間 時間経過後に速度が落ちる
    /// </summary>
    [SerializeField, Header("壁ジャンプ無操作時間")] private float jumpWallOnlyTime = .0f;

    /// <summary>
    /// 壁ジャンプ速度の終了時間
    /// </summary>
    [SerializeField, Header("壁ジャンプ速度の終了時間")] private float jumpWallDownTime = .0f;

    /// <summary>
    /// 壁のへばり付き秒数
    /// </summary>
    [SerializeField, Header("壁のへばり付き秒数")] private float wallStickTimer = .0f;

    /// <summary>
    /// 壁のへばり加速度
    /// </summary>
    [SerializeField, Header("壁のへばり加速度")] private float wallStickVelocity = .0f;

    /// <summary>
    /// 移動速度
    /// </summary>
    public float MoveSpeed => this.moveSpeed;

    /// <summary>
    /// 重力最高速
    /// </summary>
    public float GravityMaxSpeed => this.gravityMaxSpeed;

    /// <summary>
    /// 上ジャンプ力
    /// </summary>
    public float JumpUpPower => this.jumpUpPower;

    /// <summary>
    /// 壁ジャンプ上
    /// </summary>
    public float JumpWallUpPower => this.jumpWallUpPower;

    /// <summary>
    /// 壁ジャンプ横
    /// </summary>
    public float JumpWallSidePower => this.jumpWallSidePower;

    /// <summary>
    /// 壁ジャンプ無操作時間 時間経過後に速度が落ちる
    /// </summary>
    public float JumpWallOnlyTime => this.jumpWallOnlyTime;

    /// <summary>
    /// 壁ジャンプ速度の終了時間
    /// </summary>
    public float JumpWallDownTime => this.jumpWallDownTime;

    /// <summary>
    /// 壁のへばり付き秒数
    /// </summary>
    public float WallStickTimer => this.wallStickTimer;

    /// <summary>
    /// 壁のへばり加速度
    /// </summary>
    public float WallStickVelocity => this.wallStickVelocity;
}
