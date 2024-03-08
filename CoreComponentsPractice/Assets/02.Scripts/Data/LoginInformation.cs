using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using DiceGame.Game;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiceGame.Data
{
    public static class LoginInformation
    {
        public static bool loggedIn => profile == null ? false : profile.IsValid;
        public static ProfileDataModel profile { get; private set; }
        public static bool isTesting = GameManager.instance.isTesting;

        
        public static async Task<bool> TryLogin(string id, string pw)
        {
            if (isTesting)
            {
                id = "tester";
                pw = "0000";
            }

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

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
                        if (id.Equals((string)documentDictionary["id"]) &&
                            pw.Equals((string)documentDictionary["pw"]))
                        {
                            profile = new ProfileDataModel()
                            {
                                id = (string)documentDictionary["id"],
                                pw = (string)documentDictionary["pw"],
                                nickname = (string)documentDictionary["nickname"]
                            };

                            return;
                        }
                    }
                }
            });

            if (loggedIn)
            {
                Debug.Log($"[LoginInformation] : Logged in with {profile.id}");
                return true;
            }

            Debug.Log($"LoginInformation] : Failed to Login with {profile.id}");
            return false;
        }
    }
}
