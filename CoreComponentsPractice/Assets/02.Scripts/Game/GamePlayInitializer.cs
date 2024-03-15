using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiceGame.Game
{
    public class GamePlayInitializer : MonoBehaviour
    {
        private void Start()
        {
            PhotonNetwork.Instantiate("NetworkObjects/Player-PunObservable", Vector3.zero, Quaternion.identity);
        }
    }
}
