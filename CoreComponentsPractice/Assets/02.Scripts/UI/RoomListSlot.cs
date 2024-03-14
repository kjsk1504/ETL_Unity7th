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
    /// 
    /// </summary>
    public class RoomListSlot : MonoBehaviour
    {
        /// <summary>  </summary>
        public int roomIndex;
        /// <summary>  </summary>
        private TMP_Text _roomName;
        /// <summary>  </summary>
        private TMP_Text _playerRatio;
        /// <summary>  </summary>
        private Button _select;
        /// <summary>  </summary>
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
        /// 
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
