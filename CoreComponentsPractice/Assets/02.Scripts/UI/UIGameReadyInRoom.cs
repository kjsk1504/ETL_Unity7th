using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using DiceGame.Game;

namespace DiceGame.UI
{
    /// <summary>
    /// 게임 시작전 대기 방
    /// </summary>
    public class UIGameReadyInRoom : UIBase, IUIScreen, IInRoomCallbacks
    {
        /// <summary> 플레이어 슬롯이 들어갈 부모 위치 </summary>
        [SerializeField] Transform _playerStatusInGameReayInRoomContent;
        /// <summary> 플레이어의 슬롯 </summary>
        [SerializeField] PlayerStatusInGameReayInRoomSlot _playerStatusInGameReayInRoomSlotPrefab;
        /// <summary> 레디 버튼 </summary>
        private Button _ready;
        /// <summary> 시작 버튼 </summary>
        private Button _start;
        /// <summary> 플레이어의 상태를 저장하는 딕셔너리 </summary>
        private Dictionary<string, PlayerStatusInGameReayInRoomSlot> _playerStatusInGameReayInRoomSlots
            = new Dictionary<string, PlayerStatusInGameReayInRoomSlot>();


        protected override void Awake()
        {
            base.Awake();

            // 초기화
            _ready = transform.Find("Button - Ready").GetComponent<Button>();
            _start = transform.Find("Button - Start").GetComponent<Button>();

            // 준비 버튼 클릭시 발동
            _ready.onClick.AddListener(() =>
            {
                // 포톤 서버에 이 플레이어의 준비 상태를 토글
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
                {
                    { "isReady", !(bool)PhotonNetwork.LocalPlayer.CustomProperties["isReady"] }
                });
            });

            // 시작 버튼 클릭시 발동
            _start.onClick.AddListener(() =>
            {
                // 방장 여부 한번 더 확인
                if (PhotonNetwork.IsMasterClient == false)
                    throw new System.Exception($"[UIGameReadyInRoom] : Tried to start game despite I'm not a master client.");

                // 모든 플레이어 준비됐는지 확인
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (player.IsMasterClient)
                        continue;

                    if (player.CustomProperties.TryGetValue("isReady", out bool isReady))
                    {
                        // 준비 안된 player 찾음
                        if (isReady == false)
                            return;
                    }
                    else
                    {
                        return; // 이 player의 CustomProperty가 아직 설정되지 못함..
                    }
                }

                // 현재 방의 모든 클라이언트의 씬을 이동
                PhotonNetwork.LoadLevel("GamePlay"); // 방 안에 있는 모든 플레이어의 씬을 바꾸는 함수
            });
        }

        private void Start()
        {
            StartCoroutine(C_Init());
        }

        /// <summary>
        /// 시작시 발동할 코루틴 (방 안의 플레이어 준비 상태 반영)
        /// </summary>
        IEnumerator C_Init()
        {
            // 포톤 서버에서 방에 참가함이 반영될 때까지 기다림
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Joined);
            // 포톤 서버에 준비 상태가 반영될 때까지 기다림
            yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("isReady"));

            // 방장이라면 시작 버튼은 활성화, 레디 버튼은 비활성화
            _start.gameObject.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient == true);
            _ready.gameObject.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient == false);

            // 방 안의 플레이어들 리스트를 포톤 서버에서 받아옴
            Player[] players = PhotonNetwork.PlayerList;

            // 받아온 리스트를 기반으로 슬롯을 생성하고 준비 상태를 반영
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

        /// <summary>
        /// 새로 들어온 플레이어에 대한 정보 갱신
        /// </summary>
        /// <param name="player"> 새로 들어온 플레이어 </param>
        /// <returns></returns>
        IEnumerator C_RefreshSlot(Player player)
        {
            // 포톤 서버에 새로운 플레이어의 준비 상태가 반영될 때까지 대기
            yield return new WaitUntil(() => player.CustomProperties.ContainsKey("isReady"));
            // 새로운 플레이어의 준비 상태를 반영
            _playerStatusInGameReayInRoomSlots[player.UserId].Refresh((bool)player.CustomProperties["isReady"]);
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            var slot = Instantiate(_playerStatusInGameReayInRoomSlotPrefab, _playerStatusInGameReayInRoomContent);
            _playerStatusInGameReayInRoomSlots.Add(newPlayer.UserId, slot);
            StartCoroutine(C_RefreshSlot(newPlayer));
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
