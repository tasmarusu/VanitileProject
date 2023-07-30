using UnityEngine;

/// <summary>
/// �v���C���[�f�[�^�X�N���v�^
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerDataScriptable", order = 3)]
public class PlayerDataScriptable : ScriptableObject
{
    /// <summary>
    /// �ړ����x
    /// </summary>
    [SerializeField, Header("�ړ����x")] private float moveSpeed = .0f;

    /// <summary>
    /// �d�͍ō���
    /// </summary>
    [SerializeField, Header("�d�͍ō���")] private float gravityMaxSpeed = .0f;

    /// <summary>
    /// ��W�����v��
    /// </summary>
    [SerializeField, Header("��W�����v��")] private float jumpUpPower = .0f;

    /// <summary>
    /// �ǃW�����v��
    /// </summary>
    [SerializeField, Header("�ǃW�����v�� ��")] private float jumpWallUpPower = .0f;

    /// <summary>
    /// �ǃW�����v��
    /// </summary>
    [SerializeField, Header("�ǃW�����v�� ��")] private float jumpWallSidePower = .0f;

    /// <summary>
    /// �ǃW�����v�����쎞�� ���Ԍo�ߌ�ɑ��x��������
    /// </summary>
    [SerializeField, Header("�ǃW�����v�����쎞��")] private float jumpWallOnlyTime = .0f;

    /// <summary>
    /// �ǃW�����v���x�̏I������
    /// </summary>
    [SerializeField, Header("�ǃW�����v���x�̏I������")] private float jumpWallDownTime = .0f;

    /// <summary>
    /// �ǂ̂ւ΂�t���b��
    /// </summary>
    [SerializeField, Header("�ǂ̂ւ΂�t���b��")] private float wallStickTimer = .0f;

    /// <summary>
    /// �ǂ̂ւ΂�����x
    /// </summary>
    [SerializeField, Header("�ǂ̂ւ΂�����x")] private float wallStickVelocity = .0f;

    /// <summary>
    /// �ړ����x
    /// </summary>
    public float MoveSpeed => this.moveSpeed;

    /// <summary>
    /// �d�͍ō���
    /// </summary>
    public float GravityMaxSpeed => this.gravityMaxSpeed;

    /// <summary>
    /// ��W�����v��
    /// </summary>
    public float JumpUpPower => this.jumpUpPower;

    /// <summary>
    /// �ǃW�����v��
    /// </summary>
    public float JumpWallUpPower => this.jumpWallUpPower;

    /// <summary>
    /// �ǃW�����v��
    /// </summary>
    public float JumpWallSidePower => this.jumpWallSidePower;

    /// <summary>
    /// �ǃW�����v�����쎞�� ���Ԍo�ߌ�ɑ��x��������
    /// </summary>
    public float JumpWallOnlyTime => this.jumpWallOnlyTime;

    /// <summary>
    /// �ǃW�����v���x�̏I������
    /// </summary>
    public float JumpWallDownTime => this.jumpWallDownTime;

    /// <summary>
    /// �ǂ̂ւ΂�t���b��
    /// </summary>
    public float WallStickTimer => this.wallStickTimer;

    /// <summary>
    /// �ǂ̂ւ΂�����x
    /// </summary>
    public float WallStickVelocity => this.wallStickVelocity;
}
