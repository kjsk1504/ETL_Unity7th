using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DiceGame.UI
{
    /// <summary>
    /// 로비 UI 관리
    /// </summary>
    public class UILobby : UIBase, IUIScreen, ILobbyCallbacks
    {
        /// <summary> 선택된 방의 인덱스 </summary>
        public int selectedRoomListSlotIndex
        {
            get => _selectedRoomListSlotIndex;
            set
            {
                _selectedRoomListSlotIndex = value;
                _join.interactable = value >= 0;
            }
        }

        /// <summary> Join 버튼 </summary>
        private Button _join;
        /// <summary> Create 버튼 </summary>
        private Button _create;
        /// <summary> 방의 부모 될 컨텐트 위치 </summary>
        [SerializeField] RectTransform _roomListContent;
        /// <summary> 방 슬롯의 프리펩 </summary>
        [SerializeField] RoomListSlot _roomListSlotPrefab;
        /// <summary> 방 슬롯 리스트 </summary>
        private List<RoomListSlot> _roomListSlots = new List<RoomListSlot>();
        /// <summary> 선택된 방의 인텍스 </summary>
        private int _selectedRoomListSlotIndex;
        /// <summary> 룸의 정보 리스트 </summary>
        private List<RoomInfo> _localRoomInfos;


        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this); // PhotonNetwork interface 상속받았으면, 콜백 호출 대상으로 등록해야 구현한 콜백함수들이 호출됨.
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        protected override void Awake()
        {
            base.Awake();

            _join = transform.Find("Button - Join").GetComponent<Button>();
            _create = transform.Find("Button - Create").GetComponent<Button>();

            _join.onClick.AddListener(() =>
            {
                if (PhotonNetwork.JoinRoom(_localRoomInfos[_selectedRoomListSlotIndex].Name))
                {
                }
                else
                {
                    UIManager.instance.Get<UIWarningWindow>()
                                      .Show("The room is invalid.");
                }
            });

            _create.onClick.AddListener(() => UIManager.instance.Get<UICreatingRoom>().Show());
        }

        private void Start()
        {
            StartCoroutine(C_JoinLobyAtTheVeryFirstTime());
        }

        IEnumerator C_JoinLobyAtTheVeryFirstTime()
        {
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);
            PhotonNetwork.JoinLobby();
        }

        public void OnJoinedLobby()
        {
            Debug.Log($"Joined Lobby");
        }

        public void OnLeftLobby()
        {
            throw new System.NotImplementedException();
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            throw new System.NotImplementedException();
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            _localRoomInfos = roomList;

            Debug.Log("Room list updated...");
            for (int i = _roomListSlots.Count - 1; i >= 0; i--)
                Destroy(_roomListSlots[i].gameObject);

            _roomListSlots.Clear();

            for (int i = 0; i < roomList.Count; i++)
            {
                RoomListSlot tmpSlot = Instantiate(_roomListSlotPrefab, _roomListContent);
                tmpSlot.roomIndex = i;
                tmpSlot.Refresh(roomList[i].Name, roomList[i].PlayerCount, roomList[i].MaxPlayers);
                tmpSlot.onSelect += () => selectedRoomListSlotIndex = tmpSlot.roomIndex;
                _roomListSlots.Add(tmpSlot);
            }
        }
    }
}
