using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DiceGame.UI
{
    /// <summary>
    /// 방을 만드는 UI
    /// </summary>
    public class UICreatingRoom : UIPopUpBase
    {
        /// <summary> 방 이름 넣는 필드 </summary>
        private TMP_InputField _roomName;
        /// <summary> 최대 플레이어 결정하는 스크롤바 </summary>
        private Scrollbar _maxPlayer;
        /// <summary> 최대 플레이어 값을 표현해주는 텍스트 </summary>
        private TMP_Text _maxPlayerValue;
        /// <summary> 결정 버튼 </summary>
        private Button _confirm;
        /// <summary> 취소 버튼 </summary>
        private Button _cancel;


        protected override void Awake()
        {
            base.Awake();

            // 초기화
            _roomName = transform.Find("Panel/InputField (TMP) - RoomName").GetComponent<TMP_InputField>();
            _maxPlayer = transform.Find("Panel/Scrollbar - MaxPlayer").GetComponent<Scrollbar>();
            _maxPlayerValue = transform.Find("Panel/Text (TMP) - MaxPlayerValue").GetComponent<TMP_Text>();
            _confirm = transform.Find("Panel/Button - Confirm").GetComponent<Button>();
            _cancel = transform.Find("Panel/Button - Cancel").GetComponent<Button>();

            // 방 이름이 2자 이상일때 결정 버튼 활성화
            _roomName.onValueChanged.AddListener(value => _confirm.interactable = value.Length > 1);

            // 최대 플레이어 값을 스크롤바와 연동
            _maxPlayer.value = 0;
            _maxPlayerValue.text = Mathf.RoundToInt(_maxPlayer.value * (_maxPlayer.numberOfSteps - 1) + 1).ToString();
            _maxPlayer.onValueChanged.AddListener(value =>
            {
                _maxPlayerValue.text = Mathf.RoundToInt(value * (_maxPlayer.numberOfSteps - 1) + 1).ToString();
            });

            // 결정 버튼을 누르면 방 생성 (이름, 최대인원 반영)
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

            // 취소 버튼 설정
            _cancel.onClick.AddListener(Hide);
        }
    }
}
