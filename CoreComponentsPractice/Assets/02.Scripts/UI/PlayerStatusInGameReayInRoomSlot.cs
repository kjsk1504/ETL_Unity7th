using TMPro;
using UnityEngine;

namespace DiceGame.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerStatusInGameReayInRoomSlot : MonoBehaviour
    {
        private TMP_Text _status;


        /// <summary>
        /// 
        /// </summary>
        public void Refresh(bool isReady)
        {
            _status.enabled = isReady;
        }

        private void Awake()
        {
            _status = transform.Find("Text (TMP) - Status").GetComponent<TMP_Text>();
        }
    }
}