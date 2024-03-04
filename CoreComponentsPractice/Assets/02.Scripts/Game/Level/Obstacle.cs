using DiceGame.Character;
using System;
using System.Collections;
using UnityEngine;

namespace DiceGame.Level
{
    public abstract class Obstacle : MonoBehaviour
    {
        public Node node { get; set; }
        public abstract IEnumerator C_Interaction(PlayerController interactor);

        protected virtual void Awake() { }
    }
}
