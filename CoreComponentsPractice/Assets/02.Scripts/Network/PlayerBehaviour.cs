using DiceGame.Data;
using DiceGame.UI;
using Photon.Pun;
using UnityEngine;

namespace DiceGame.Network
{
    public class PlayerBehaviour : ClientBehaviour
    {
        private void Update()
        {
            if (photonView.IsMine)
            {
                horizontal = Input.GetAxis("Horizontal");
                vertical = Input.GetAxis("Vertical");
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                photonView.RPC(nameof(AlertSomething), RpcTarget.Others, new object[] { $"Hi everyone !, I'm {LoginInformation.profile.nickname}"});
            }
        }


        [PunRPC]
        private void AlertSomething(string message)
        {
            UIManager.instance.Get<UIWarningWindow>().Show(message);
        }
    }
}
