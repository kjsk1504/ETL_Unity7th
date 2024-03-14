using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceGame.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UICreatingRoom : UIPopUpBase
    {
        /// <summary>  </summary>
        private TMP_InputField _roomName;
        /// <summary>  </summary>
        private Scrollbar _maxPlayer;
        /// <summary>  </summary>
        private TMP_Text _maxPlayerValue;
        /// <summary>  </summary>
        private Button _confirm;
        /// <summary>  </summary>
        private Button _cancel;


        protected override void Awake()
        {
            base.Awake();

            _roomName = transform.Find("Panel/InputField (TMP) - RoomName").GetComponent<TMP_InputField>();
            _maxPlayer = transform.Find("Panel/Scrollbar - MaxPlayer").GetComponent<Scrollbar>();
            _maxPlayerValue = transform.Find("Panel/Text (TMP) - MaxPlayerValue").GetComponent<TMP_Text>();
            _confirm = transform.Find("Panel/Button - Confirm").GetComponent<Button>();
            _cancel = transform.Find("Panel/Button - Cancel").GetComponent<Button>();

            _roomName.onValueChanged.AddListener(value => _confirm.interactable = value.Length > 1);

            _maxPlayer.value = 0;
            _maxPlayerValue.text = Mathf.RoundToInt(_maxPlayer.value * (_maxPlayer.numberOfSteps - 1) + 1).ToString();
            _maxPlayer.onValueChanged.AddListener(value =>
            {
                _maxPlayerValue.text = Mathf.RoundToInt(value * (_maxPlayer.numberOfSteps - 1) + 1).ToString();
            });

            _confirm.interactable = false;
            _confirm.onClick.AddListener(() =>
            {
                RoomOptions roomOptions = new RoomOptions
                {
                    CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
                    {
                        { "levelLimit", 10 }
                    },
                    MaxPlayers = Mathf.RoundToInt(_maxPlayer.value * (_maxPlayer.numberOfSteps - 1) + 1),
                    PublishUserId = true,
                };

                PhotonNetwork.CreateRoom(_roomName.text, roomOptions);
            });
            _cancel.onClick.AddListener(Hide);
        }
    }
}