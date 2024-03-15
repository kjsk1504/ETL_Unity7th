using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiceGame.Singleton;
using Photon.Pun;
using System.Linq;
using System;
using Photon.Realtime;

namespace DiceGame.Game
{
    public class GamePlayManager : MonoBehaviour, IPunObservable
    {
        public enum State
        {
            None,
            SpawnCharacter,
            ReadyForBattle,
            Battle,
        }
        [field: SerializeField] public State state {  get; private set; }
        private int[] _spawnPointIndexes;
        private PhotonView _photonView;


        public void StartWorkflow()
        {
            if (state == State.None)
                state = State.SpawnCharacter;
            else
                throw new Exception($"[GamePlayManager] : Game has already started.");
        }

        protected void Awake()
        {
            _photonView = GetComponent<PhotonView>();

            if (!_photonView.IsMine)
                return;

            GameManager.instance.state = GameState.InGamePlay;

            _spawnPointIndexes = new int[PhotonNetwork.PlayerList.Length];
            for (int i = 0;  i < _spawnPointIndexes.Length; i++)
            {
                _spawnPointIndexes[i] = -1;
            }
        }

        private void Start()
        {
            if (!_photonView.IsMine)
                return;

            if (PhotonNetwork.IsMasterClient)
            {
                List<int> list = new List<int>();
                for (int i = 0; i < GameObject.Find("SpawnPoints").transform.childCount; i++)
                {
                    list.Add(i);
                }
                _spawnPointIndexes = list.OrderBy(x => Guid.NewGuid()).ToArray();
            }

            StartWorkflow();
        }

        private void Update()
        {
            if (!_photonView.IsMine)
                return;

            Workflow();
        }

        private void Workflow()
        {
            switch (state)
            {
                case State.None:
                    break;
                case State.SpawnCharacter:
                    {
                        if (_spawnPointIndexes[0] >= 0)
                        {
                            SpwanCharacter();
                            state++;
                        }
                    }
                    break;
                case State.ReadyForBattle:
                    break;
                case State.Battle:
                    break;
                default:
                    break;
            }
        }

        private void SpwanCharacter()
        {
            List<Transform> tranforms = new List<Transform>(GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>());
            tranforms.Remove(transform);

            IEnumerable<Player> filtered = PhotonNetwork.PlayerList.OrderBy(x => x.ActorNumber);
            int index = 0;

            foreach(Player player in filtered)
            {
                if (player.IsLocal)
                {
                    PhotonNetwork.Instantiate("NetworkObjects/Player", tranforms[_spawnPointIndexes[index]].position, Quaternion.identity);
                }

                index++;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (!_photonView.IsMine)
                return;

            if (stream.IsWriting)
            {
                if (PhotonNetwork.IsMasterClient)
                    for (int i = 0; i < _spawnPointIndexes.Length; i++)
                    {
                        stream.SendNext(_spawnPointIndexes[i]);
                    }
            }
            else // stream.IsReading
            {
                if (!PhotonNetwork.IsMasterClient)
                    for (int i = 0; i < _spawnPointIndexes.Length; i++)
                    {
                        _spawnPointIndexes[i] = (int)stream.ReceiveNext();
                    }
            }
        }
    }
}
