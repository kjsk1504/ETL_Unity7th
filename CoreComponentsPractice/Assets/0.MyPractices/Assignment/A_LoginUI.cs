using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase;
using System;
using DiceGame.Data;
using Firebase.Extensions;
using System.Collections.Generic;
using DiceGame.UI;

namespace Assignment.DiceGame.UI
{
    //[RequireComponent(typeof(GoogleAuth))]
    public class A_LoginUI : A_UIScreenBase
    {
        [SerializeField] A_SignupUI _signupUI;

        private TMP_InputField _id;
        private TMP_InputField _pw;
        private Button _tryLogin;
        private Button _signUp;
        private List<Selectable> _loginFields;
        private int _currentState;
        //private GoogleAuth _googleAuth;

        protected override void Awake()
        {
            base.Awake();

            _loginFields = new List<Selectable>();

            _loginFields.Add(item: _id = transform.Find("LoginPanel/ID Panel/InputField (TMP)").GetComponent<TMP_InputField>());
            _loginFields.Add(item: _pw = transform.Find("LoginPanel/PW Panel/InputField (TMP)").GetComponent<TMP_InputField>());
            _tryLogin = transform.Find("LoginPanel/Button - Login").GetComponent<Button>();
            _signUp = transform.Find("LoginPanel/Button - SignUp").GetComponent<Button>();
        }

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
                if (_id.text.IndexOf('@') > 0)
                {
                    UIManager.instance.Get<UIWarningWindow>().Show("Email형식을 갖춰야 합니다.");
                    return;
                }

                if (_pw.text.Length >= 6)
                {
                    UIManager.instance.Get<UIWarningWindow>().Show("비밀번호는 최소 6자 이상이어야 합니다.");
                    return;
                }

                FirebaseAuth auth = FirebaseAuth.DefaultInstance;
                auth.SignInWithEmailAndPasswordAsync(_id.text, _pw.text)
                    .ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled)
                            Debug.LogError("Canceled login");
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

            _signUp.onClick.AddListener(() =>
            {
                _signupUI.Show();
            });
        }

        public override void InputAction()
        {
            base.InputAction();

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (_currentState == _loginFields.Count - 1)
                    _currentState = 0;
                else
                    _currentState++;
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                _tryLogin.onClick?.Invoke();
            }

            _loginFields[_currentState].Select();
        }
    }
}
