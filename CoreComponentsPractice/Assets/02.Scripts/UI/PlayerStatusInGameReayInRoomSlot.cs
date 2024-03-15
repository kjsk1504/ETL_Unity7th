using TMPro;
using UnityEngine;

namespace DiceGame.UI
{
    /// <summary>
    /// 대기방에서 사용할 플레이어의 상태 슬롯
    /// </summary>
    public class PlayerStatusInGameReayInRoomSlot : MonoBehaviour
    {
        private TMP_Text _status;


        /// <summary>
        /// 레디 여부를 갱신하는 함수
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
