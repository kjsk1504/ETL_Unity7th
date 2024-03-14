using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace DiceGame.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class UIGameReadyInRoom : UIBase, IUIScreen, IInRoomCallbacks
    {
        /// <summary>  </summary>
        [SerializeField] Transform _playerStatusInGameReayInRoomContent;
        /// <summary>  </summary>
        [SerializeField] PlayerStatusInGameReayInRoomSlot _playerStatusInGameReayInRoomSlotPrefab;
        /// <summary>  </summary>
        private Button _ready;
        /// <summary>  </summary>
        private Button _start;
        /// <summary>  </summary>
        private Dictionary<string, PlayerStatusInGameReayInRoomSlot> _playerStatusInGameReayInRoomSlots
            = new Dictionary<string, PlayerStatusInGameReayInRoomSlot>();


        protected override void Awake()
        {
            base.Awake();

            _ready = transform.Find("Button - Ready").GetComponent<Button>();
            _start = transform.Find("Button - Start").GetComponent<Button>();

            _ready.onClick.AddListener(() =>
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
                {
                    { "isReady", !(bool)PhotonNetwork.LocalPlayer.CustomProperties["isReady"] }
                });
            });

            _start.onClick.AddListener(() =>
            {
                if (PhotonNetwork.IsMasterClient == false)
                    throw new System.Exception($"[UIGameReadyInRoom] : Tried to start game despite I'm not a master client.");

                PhotonNetwork.LoadLevel("GamePlay"); // 방 안에 있는 모든 플레이어의 씬을 바꾸는 함수
            });
        }

        private void Start()
        {
            StartCoroutine(C_Init());
        }

        /// <summary>
        /// 
        /// </summary>
        IEnumerator C_Init()
        {
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Joined);

            _start.gameObject.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient == true);
            _ready.gameObject.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient == false);

            Player[] players = PhotonNetwork.PlayerList;

            for (int i = 0; i < players.Length; i++)
            {
                var slot = Instantiate(_playerStatusInGameReayInRoomSlotPrefab, _playerStatusInGameReayInRoomContent);
                slot.Refresh((bool)players[i].CustomProperties["isReady"]);
                _playerStatusInGameReayInRoomSlots.Add(players[i].UserId, slot);
            }
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            _start.gameObject.SetActive(newMasterClient.IsLocal == true);
            _ready.gameObject.SetActive(newMasterClient.IsLocal == false);
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            var slot = Instantiate(_playerStatusInGameReayInRoomSlotPrefab, _playerStatusInGameReayInRoomContent);
            slot.Refresh((bool)newPlayer.CustomProperties["isReady"]);
            _playerStatusInGameReayInRoomSlots.Add(newPlayer.UserId, slot);
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (_playerStatusInGameReayInRoomSlots.TryGetValue(otherPlayer.UserId, out var slot))
            {
                Destroy(slot.gameObject);
                _playerStatusInGameReayInRoomSlots.Remove(otherPlayer.UserId);
            }
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (_playerStatusInGameReayInRoomSlots.TryGetValue(targetPlayer.UserId, out var slot))
            {
                if (changedProps.TryGetValue("isReady", out bool value))
                {
                    slot.Refresh(value);
                }
            }
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
        }
    }
}