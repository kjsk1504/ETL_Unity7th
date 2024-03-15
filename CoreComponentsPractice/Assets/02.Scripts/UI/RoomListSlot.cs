using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace DiceGame.UI
{
    /// <summary>
    /// 방의 슬롯
    /// </summary>
    public class RoomListSlot : MonoBehaviour
    {
        /// <summary> 방의 인덱스 </summary>
        public int roomIndex;
        /// <summary> 방의 이름 </summary>
        private TMP_Text _roomName;
        /// <summary> 방 참가자 비율 </summary>
        private TMP_Text _playerRatio;
        /// <summary> 선택되었을때 (슬롯 자체) </summary>
        private Button _select;
        /// <summary> 선택되었을 때 호출되는 이벤트 (wrapping) </summary>
        public event UnityAction onSelect
        {
            add
            {
                _select.onClick.AddListener(value);
            }
            remove
            {
                _select.onClick.RemoveListener(value);
            }
        }

        /// <summary>
        /// 방의 정보를 갱신하는 함수
        /// </summary>
        public void Refresh(string roomName, int currentPlayersInRoom, int MaxPlayers)
        {
            _roomName.text = roomName;
            _playerRatio.text = $"{currentPlayersInRoom} / {MaxPlayers}";
        }

        private void Awake()
        {
            _roomName = transform.Find("Text (TMP) - RoomName").GetComponent<TMP_Text>();
            _playerRatio = transform.Find("Text (TMP) - PlayerRatio").GetComponent<TMP_Text>();
            _select = GetComponent<Button>();
        }
    }
}
