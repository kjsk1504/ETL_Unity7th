using Firebase.Firestore;
using TMPro;
using UnityEngine.UI;
using DiceGame.Data;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace DiceGame.UI
{
    public class A_NickNameWindow : UIPopUpBase
    {
        [SerializeField] TMP_InputField _nickname;
        [SerializeField] Button _confirm;

        protected override void Awake()
        {
            base.Awake();

            _confirm.onClick.AddListener(() =>
            {
                CollectionReference collectionReference = FirebaseFirestore.DefaultInstance.Collection("users");
                collectionReference.GetSnapshotAsync()
                                   .ContinueWithOnMainThread(task =>
                                   {
                                       // nickname 중복검사
                                       foreach (DocumentSnapshot documentSnapshot in task.Result.Documents)
                                       {
                                           documentSnapshot.TryGetValue("nicname", out string nickname);
                                           if (nickname == _nickname.text)
                                           {
                                               UIManager.instance.Get<UIWarningWindow>()
                                                                 .Show($"{_nickname.text} is already in use.");
                                               return;
                                           }
                                       }

                                       FirebaseFirestore.DefaultInstance.Collection("users").Document(LoginInformation.userKey)
                                            .SetAsync(new Dictionary<string, object>
                                            {
                                                {"nickname", _nickname.text },
                                            })
                                            .ContinueWith(task =>
                                            {
                                                LoginInformation.profile = new ProfileDataModel
                                                {
                                                    nickname = _nickname.text,
                                                };
                                            });
                                   });
            });

            _confirm.interactable = false;
            _nickname.onValueChanged.AddListener(value => _confirm.interactable = IsFormatValid());
        }

        private bool IsFormatValid()
        {
            return (_nickname.text.Length >= 2) && (_nickname.text.Length <= 10);
        }
    }
}
