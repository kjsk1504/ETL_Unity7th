using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Extensions;
using DiceGame.UI;

namespace Assignment.DiceGame.UI
{
    public class A_SignupUI : UIBase
    {
        [SerializeField] TMP_InputField _id;
        [SerializeField] TMP_InputField _pw;
        [SerializeField] Button _sign;
        [SerializeField] Button _exit;
        [SerializeField] A_UIWarningWindow _warningWindow;
        [SerializeField] A_UIAlarmWindow _alarmWindow;


        private void Start()
        {
            _exit.onClick.AddListener(() =>
            {
                Hide();
            });

            _sign.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(_id.text))
                {
                    _warningWindow.Show("아이디를 입력해주세요.");
                    return;
                }

                if (string.IsNullOrEmpty(_pw.text))
                {
                    _warningWindow.Show("비밀번호를 입력해주세요.");
                    return;
                }

                if (_pw.text.Length < 6)
                {
                    _warningWindow.Show("비밀번호는 최소 6자 이상이어야 합니다.");
                    return;
                }

                FirebaseAuth auth = FirebaseAuth.DefaultInstance;
                auth.CreateUserWithEmailAndPasswordAsync(_id.text, _pw.text).ContinueWithOnMainThread(task => 
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                        return;
                    }

                    // Firebase user has been created.
                    AuthResult result = task.Result;
                    Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                                    result.User.DisplayName, result.User.UserId);

                    _alarmWindow.Show("회원가입 완료", async () =>
                    {
                        await Task.Delay(1000);
                        _alarmWindow.Hide();
                    });

                    Hide();
                });

            });
        }
    }
}
