using System;
using System.Collections;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerControllerHp : MonoBehaviour, IHp
{
    public float hp => _hp;
    public float hpMin => _hpMin;
    public float hpMax => _hpMax;

    private float _hp;
    private float _hpMin = 0.0f;
    private float _hpMax = 100.0f;
    // event 한정자 : 외부 클래스에서는 이 대리자를 쓸 때 +=, -=의 피연산자로만 사용가능
    public event Action<float> onHpDepleted;

    //public delegate void OnHpDepletedHandler(float value);
    //public event OnHpDepletedHandler onHpDepleted2;

    //public delegate CodeIdentifier Action2<in T>(T t);
    //public event Action2<float> onHpDepleted3;

    private void Awake()
    {
        _hp = _hpMax; // 초기화
    }

    public void DepleteHp(float amount)
    {
        if (_hp <= _hpMin || amount <= 0)
            return;

        _hp -= amount;
        onHpDepleted?.Invoke(_hp);
    }
    IEnumerator DoSomthing(Action completed)
    {
        // ~~ do Something
        yield return new WaitForEndOfFrame();
        // ~~ do Something2
        yield return new WaitForSeconds(5);
        completed.Invoke();
    }
}
