using Firebase.Firestore;
using Firebase.Extensions;
using DiceGame.Game;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiceGame.Data
{
    public static class LoginInformation
    {
        public static bool loggedIn => string.IsNullOrEmpty(s_id) == false;
        public static string userKey { get; private set; }
        public static ProfileDataModel profile { get; set; }
        private static string s_id;

        
        public static async Task<bool> RefreshInformationAsync(string id)
        {
            bool result = false;

            if (GameManager.instance.isTesting)
            {
                id = "tester";
            }

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            userKey = id.Replace("@", "").Replace(".", "");
            DocumentReference docRef = db.Collection("users").Document(userKey);

            await docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                Dictionary<string, object> documentDictionary = task.Result.ToDictionary();

                if (documentDictionary != null)
                {
                    profile = new ProfileDataModel
                    {
                        nickname = (string)documentDictionary["nickname"],
                    };
                    result = true;
                }
            });

            s_id = id;
            return result;

            /* id/pw 방식
            CollectionReference usersRef = db.Collection("users");

            await usersRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                // Snapshot : 읽은 데이터, 읽어서 받아본 데이터는 사실 이미 지난 과거의 데이터이기 때문에 Snapshot이라고 함.
                QuerySnapshot snapshots = task.Result; // 요청(Query)하고 받은 Snapshot
                foreach (DocumentSnapshot documentSnapshot in snapshots.Documents)
                {
                    Dictionary<string, object> documentDictionary = documentSnapshot.ToDictionary();
                    if (documentDictionary.ContainsKey("id"))
                    {
                        if (id.Equals((string)documentDictionary["id"]))
                        {
                            if (pw.Equals((string)documentDictionary["pw"]))
                            {
                                profile = new ProfileDataModel()
                                {
                                    nickname = (string)documentDictionary["nickname"]
                                };

                                return;
                            }
                            else
                            {
                                UIManager.instance.Get<UIWarningWindow>()
                                                  .Show("Wrong password.");

                                return;
                            }
                        }
                    }
                }

                UIManager.instance.Get<UIWarningWindow>()
                                  .Show("Wrong ID.");
            });

            if (loggedIn)
            {
                Debug.Log($"[LoginInformation] : Logged in with {id}");
                return true;
            }

            Debug.Log($"LoginInformation] : Failed to Login with {id}");
            return false;
            */
        }
    }
}
