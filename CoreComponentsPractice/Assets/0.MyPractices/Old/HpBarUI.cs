using UnityEngine;
using UnityEngine.UI;
using DiceGame.Game.Character;

public class HpBarUI : MonoBehaviour
{
    private Slider _hp;

    private void Awake()
    {
        _hp = GetComponentInChildren<Slider>();
    }
    private void Start()
    {
        PlayerController controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _hp.minValue = controller.hpMin;
        _hp.maxValue = controller.hpMax;
        _hp.value = controller.hp;

        controller.onHpDepleted += (value) => _hp.value = value;
    }
}
