using DiceGame.Character;
using UnityEngine;

namespace DiceGame.Level
{
    public class InverseNode : Node
    {
        public override void OnPlayerHere()
        {
            base.OnPlayerHere();
            PlayerController.instance.Rotate(180);
        }

        public override void OnDiceRolled(int diceValue)
        {
            PlayerController.instance.direction = PlayerController.DIRECTION_NEGATIVE;
        }
    }
}
