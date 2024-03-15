using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using DiceGame.Game;

namespace DiceGame.Network
{
    /// <summary>
    /// Photon 서버를 관리하는 매니저 (콜백 함수)
    /// </summary>
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        #region Singleton
        public static PhotonManager instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new GameObject().AddComponent<PhotonManager>();
                }

                return s_instance;
            }
        }
        private static PhotonManager s_instance;
        #endregion

        /// <summary> 마스터서버에 연결되면 호출되는 이벤트 </summary>
        public event Action onConnectedToMaster;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (PhotonNetwork.IsConnected == false)
            {
                if (PhotonNetwork.ConnectUsingSettings()) // PhotonServerSettings을 가지고 Photon 서버에 연결을 시도하는 함수
                    Debug.Log($"[PhotonManager] : Connected to photon server.");
                else
                    throw new Exception($"[PhotonManager] : Failed to photon server.");
            }
        }

        private void OnApplicationQuit()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            onConnectedToMaster?.Invoke();
            PhotonNetwork.AutomaticallySyncScene = true; // 얘 안해주면 PhotonNetwork.LoadLevel()이 방장 외 다른 클라이언트의 씬을 동기화하지 않음.
            Debug.Log("[PhotonManager]: Connected to master.");
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();

            Debug.Log($"[PhotonManager] : Created room.");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);
            Debug.Log($"[PhotonManager] : Failed to create room, returnCode : {returnCode}, {message}");
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            SceneManager.LoadScene("GameReady");
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable()
            {
                { "isReady", false }
            });
            GameManager.instance.state = GameState.InGameReady;
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();

            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable()
            {
            });
        }
    }
}
