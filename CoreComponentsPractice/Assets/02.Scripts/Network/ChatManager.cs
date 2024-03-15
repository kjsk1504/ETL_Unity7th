using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using DiceGame.Data;
using TMPro;

namespace DiceGame.Network
{
    public class ChatManager : MonoBehaviour, IChatClientListener
    {
        private ChatClient _chatClient;
        private TMP_Text _chatlog;
        private Dictionary<string, TMP_Text> _chatLogs = new Dictionary<string, TMP_Text>();


        private void Start()
        {
            _chatClient = new ChatClient(this);
            _chatClient.Connect("7850197e-c87e-43ad-9fa8-1784e0b5c7d3", 
                                PhotonNetwork.AppVersion, 
                                new Photon.Chat.AuthenticationValues(LoginInformation.profile.nickname));
        }

        private void Update()
        {
            _chatClient?.Service();
        }

        public void PublishMessage(string message)
        {
            _chatClient.PublishMessage("General", message);
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            throw new System.NotImplementedException();
        }

        public void OnChatStateChange(ChatState state)
        {
            throw new System.NotImplementedException();
        }

        public void OnConnected()
        {
            _chatClient.Subscribe(new string[] { "General" });
        }

        public void OnDisconnected()
        {
            throw new System.NotImplementedException();
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            for (int i = 0; i < senders.Length; i++)
            {
                _chatLogs[channelName].text += $"{senders[i]} : {messages[i]}\n";
            }
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            throw new System.NotImplementedException();
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            throw new System.NotImplementedException();
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            throw new System.NotImplementedException();
        }

        public void OnUnsubscribed(string[] channels)
        {
            throw new System.NotImplementedException();
        }

        public void OnUserSubscribed(string channel, string user)
        {
            throw new System.NotImplementedException();
        }

        public void OnUserUnsubscribed(string channel, string user)
        {
            throw new System.NotImplementedException();
        }
    }
}
