using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase;
using System;
using DiceGame.Data;
using Firebase.Extensions;

namespace DiceGame.UI
{
    //[RequireComponent(typeof(GoogleAuth))]
    public class LoginUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField _id;
        [SerializeField] TMP_InputField _pw;
        [SerializeField] Button _tryLogin;
        //private GoogleAuth _googleAuth;


        private async void Start()
        {
            var dependencyState = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (dependencyState != DependencyStatus.Available)
            {
                throw new Exception();
            }

            //_googleAuth = GetComponent<GoogleAuth>();

            _tryLogin.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(_id.text))
                    return;

                if (string.IsNullOrEmpty(_pw.text))
                    return;

                FirebaseAuth auth = FirebaseAuth.DefaultInstance;
                auth.SignInWithEmailAndPasswordAsync(_id.text, _pw.text)
                    .ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled)
                            Debug.LogError("Canceles login");
                        else if (task.IsFaulted)
                            Debug.LogError("Faulted login");
                        else
                        {
                            Debug.Log("ID PW is correct");
                            _ = LoginInformation.RefreshInformationAsync(_id.text);
                            //_googleAuth.OnSignIn();
                        }
                    });
            });
        }
    }
}
