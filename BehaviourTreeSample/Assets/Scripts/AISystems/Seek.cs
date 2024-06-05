using AISystems;
using UnityEngine;

/// <summary>
/// ��ǥ�� ã��, �i�� �ൿ ���
/// </summary>
public class Seek : Node
{
    /// <summary>
    /// </summary>
    /// <param name="tree"> �� ��带 �����ϴ� Ʈ�� </param>
    /// <param name="radius"> ��ǥ ���� �ݰ� </param>
    /// <param name="height"> ��ǥ ���� ����  </param>
    /// <param name="angle"> �þ߰� </param>
    /// <param name="targetMask"> ��ǥ ���� ����ũ </param>
    /// <param name="maxDistance"> ��ǥ ������ �����ϴ� �ִ� �Ÿ� </param>
    public Seek(BehaviourTree tree, float radius, float height, float angle, LayerMask targetMask, float maxDistance) : base(tree)
    {
        _radius = radius;
        _height = height;
        _angle = angle;
        _targetMask = targetMask;
        _maxDistance = maxDistance;
    }


    private float _radius;
    private float _height;
    private float _angle;
    private LayerMask _targetMask;
    private float _maxDistance;


    /// <summary>
    /// ��ǥ�� ã��, ã�� ��ǥ�� ������ �� ���� �i�ư�.
    /// �i�ư��� ���߿� ��ǥ�� Ư�� �Ÿ��� ����� ������ �ߴ���.
    /// </summary>
    public override Result Invoke()
    {
        // �̹� ������ ��ǥ�� �ִٸ�
        if (blackboard.target)
        {
            // ��ǥ ������ �Ÿ�
            float distance = Vector3.Distance(blackboard.transform.position, blackboard.target.position);

            // ��ǥ�� ���� ���� ��
            if (distance <= blackboard.agent.stoppingDistance)
            {
                return Result.Success; // ��ǥ ���� ����
            }
            // ��ǥ ���� ��
            else if (distance <= _maxDistance)
            {
                blackboard.agent.SetDestination(blackboard.target.position); // ��ã�� ��ǥ ��ġ ����
                return Result.Running; // ��ǥ ���� ���� �����ӿ��� ��� �̾ �ؾ���.
            }
            // ��ǥ�� ���� �ִ� ������ ����� ��
            else
            {
                blackboard.target = null; // ��ǥ ����.
                blackboard.agent.ResetPath(); // ��ã�� ��� ����.
                return Result.Failure; // ��ǥ ���� ����
            }
        }
        // ������ ��ǥ�� ������
        else
        {
            // ĸ�� �ݰ� ����
            Collider[] cols =
            Physics.OverlapCapsule(blackboard.transform.position,
                                   blackboard.transform.position + Vector3.up * _height,
                                   _radius,
                                   _targetMask);

            // ������ ��ǥ�� �ִ���?
            if (cols.Length > 0)
            {
                // ������ ��ǥ�� �þ߹��� ���� �ִ���?
                if (IsInSight(cols[0].transform))
                {
                    blackboard.target = cols[0].transform; // ��ǥ ����
                    blackboard.agent.SetDestination(blackboard.target.position); // ��ã�� ��ǥ ��ġ ����
                    return Result.Running;
                }
            }
        }

        return Result.Failure;
    }

    /// <summary>
    /// �þ߰� ���� �ִ��� �Ǻ�.
    /// </summary>
    /// <param name="target"> �Ǻ� ��� </param>
    /// <returns> true: �þ߰����� ����, false: �þ߰����� ���� </returns>
    private bool IsInSight(Transform target)
    {
        Vector3 origin = blackboard.transform.position; // �� ��ġ
        Vector3 forward = blackboard.transform.forward; // �� ���� ���� ���⺤��
        Vector3 lookDir = (target.position - origin).normalized; // Ÿ���� �ٶ󺸴� ���⺤��
        float theta = Mathf.Acos(Vector3.Dot(forward, lookDir)) * Mathf.Rad2Deg; // ���̰� ����

        // ��ǥ���� ���̰��� �þ߰� ���� ������
        if (theta < _angle / 2.0f)
        {
            // ��ǥ�� �� ���̿� ��ֹ��� ������ ���� ��Ƽ� Ȯ��
            if (Physics.Raycast(origin + Vector3.up * _height / 2.0f,
                                lookDir,
                                out RaycastHit hit,
                                Vector3.Distance(origin, target.position),
                                _targetMask))
            {
                return hit.collider.transform == target; // ���� ���� ���� �þ߰� �� ã�� ����� �´ٸ� OK
            }
        }

        return false;
    }
}