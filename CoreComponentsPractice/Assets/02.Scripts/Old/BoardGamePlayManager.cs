using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardGamePlayManager : MonoBehaviour
{
    [SerializeField] Transform[] _nodes;
    [SerializeField] GameObject _player;
    [SerializeField] float _moveSpeed = 1.0f;
    [SerializeField] LayerMask _targetMask;
    private int _playerLocationIndex;
    private bool isFoward = true;
    [SerializeField] BoardGamePlayStatusUI _statusUI;
    //public event Action<int> onDiceRolled;
    private Animator _animator;

    private void Awake()
    {
        _animator = _player.GetComponent<Animator>();
    }

    private void Start()
    {
        RollADice();
    }

    public void RollADice()
    {
        int value = Random.Range(1, 7);
        _statusUI.PlayRollingAnimation(value, isFoward, DoMove);
    }

    private void DoMove(int value, bool isFoward)
    {
        StartCoroutine(C_Move(_playerLocationIndex, value, isFoward, RayNode));
        if (isFoward)
        {
            _playerLocationIndex = _playerLocationIndex + value <= _nodes.Length - 1 ? _playerLocationIndex + value : _nodes.Length - 1;
        }
        else
        {
            _playerLocationIndex = _playerLocationIndex - value >= 0 ? _playerLocationIndex - value : 0;
        }
    }

    IEnumerator C_Move(int currentIndex, int value, bool isFoward, Action onFinished)
    {
        int a;
        if (isFoward) a = 1;
        else a = -1;

        for (int i = 0; i < value; i++)
        {
            int targetIndex = currentIndex + a;
            if (currentIndex >= _nodes.Length || targetIndex < 0)
            {
                _animator.SetBool("Run", false);
                break;
            }

            if (i == value - 1 || targetIndex == _nodes.Length - 1  || targetIndex == 0)
            {
                _animator.SetBool("Run", false);
                _animator.SetTrigger("Jump");
            }
            else
            {
                _animator.SetBool("Run", true);
            }

            float t = 0.0f;
            while (t < 1.0f)
            {
                _player.transform.position = Vector3.Lerp(_nodes[currentIndex].position, _nodes[targetIndex].position, t);
                t += _moveSpeed * Time.deltaTime;
                yield return null;
            }
            currentIndex = targetIndex;
            _player.transform.position = _nodes[currentIndex].position;
            yield return new WaitForSeconds(0f);
        }
        onFinished?.Invoke();
    }
    void RayNode()
    {
        if (Physics.Raycast(_player.transform.position, Vector3.down, out RaycastHit hit, float.PositiveInfinity, _targetMask))
        {
            if (hit.collider.TryGetComponent(out INode Node))
            {
                isFoward = Node.Ward;
                if (isFoward) _player.transform.rotation = new Quaternion(0, 0, 0, 0);
                else _player.transform.rotation = new Quaternion(0, 180, 0, 0);
                Node.NodeBehaviour();
            }
        }
    }
}
