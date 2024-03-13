using System;
using System.Collections.Generic;
using System.Text;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

namespace DiceGame.Data
{
    public class InGameContext
    {
        public InGameContext() 
        {
            Load();
        }


        public bool hasInitialized {  get; private set; }
        public List<InventorySlotDataModel> inventorySlotDataModels { get; private set; } // dependency source
        private FirebaseDatabase _realtimeDB = FirebaseDatabase.DefaultInstance;

        public event Action<int, InventorySlotDataModel> onInventorySlotDataChanged;


        public async void Load()
        {
            DatabaseReference inventorySlotRef =
                _realtimeDB.GetReference($"users/{LoginInformation.userKey}/inventorySlots");

            DataSnapshot snapshot = await inventorySlotRef.GetValueAsync();  // 그냥 Data가 담긴 Snapshot

            if (snapshot.Exists == false)
            {
                int defaultSlotTotal = 30;
                string head = "[";
                string SlotDataJson = "{\"itemID\": 0, \"itemNum\": 0}, ";
                string tail = "]";

                StringBuilder stringBuilder = new StringBuilder(head.Length + SlotDataJson.Length * defaultSlotTotal + tail.Length);
                stringBuilder.Append(head);
                stringBuilder.Insert(stringBuilder.Length, SlotDataJson, defaultSlotTotal);
                stringBuilder.Append(tail);

                await inventorySlotRef.SetRawJsonValueAsync(stringBuilder.ToString());
                snapshot = await inventorySlotRef.GetValueAsync();
            }

            inventorySlotDataModels = new List<InventorySlotDataModel>();

            foreach (var item in snapshot.Children)
            {
                inventorySlotDataModels.Add(JsonConvert.DeserializeObject<InventorySlotDataModel>(item.GetRawJsonValue()));
            }

            // DB에서 InventorySlot 각각의 데이터가 변경되었을때 알림을 통지할 이벤트
            inventorySlotRef.ChildChanged += (sender, childEventArgs) =>
            {
                DataSnapshot childSnapshot = childEventArgs.Snapshot;
                int slotID = int.Parse(childSnapshot.Key); // 바뀐 슬롯 ID
                InventorySlotDataModel dataChanged = 
                    JsonConvert.DeserializeObject<InventorySlotDataModel>(childSnapshot.GetRawJsonValue()); // 바뀐 슬롯 데이터
                inventorySlotDataModels[slotID] = dataChanged; // DependencySource 갱신
                onInventorySlotDataChanged?.Invoke(slotID, dataChanged); // 슬롯 데이터 변경 통지
            };

            hasInitialized = true;
        }

        /// <summary>
        /// InventorySlotsData의 DependencySource의 특정 슬롯 데이터를 DB에 저장
        /// </summary>
        /// <param name="slotID"> 저장할 DependencySource의 인덱스 </param>
        /// <param name="onCompleted"> 저장 완료 후 실행할 행동 </param>
        public void SaveInventorySlotDataModel(int slotID, InventorySlotDataModel newData, Action<InventorySlotDataModel> onCompleted)
        {
            string json = JsonConvert.SerializeObject(newData); // 저장할 객체를 json으로 직렬화
            // 새로운 Task를 만듬 -> 이 함수는 종료
            _realtimeDB.GetReference($"users/{LoginInformation.userKey}/inventorySlots/{slotID}") // 저장할 위치 참조
                       .SetRawJsonValueAsync(json) // 해당 위치에 저장
                       .ContinueWithOnMainThread(task =>
                       {
                           if (task.IsCompleted) 
                           {
                               inventorySlotDataModels[slotID] = newData;
                               onCompleted?.Invoke(newData);
                           }
                       }); // 저장 끝나고나서 추가 내용 수행
        }

        /// <summary>
        /// InventorySlotsData의 DependencySource의 특정 슬롯 데이터를 DB로부터 읽어서 갱신
        /// </summary>
        /// <param name="slotID"> 읽을 DependencySource의 인덱스 </param>
        /// <param name="onCompleted"> 읽기 완료 후 실행할 행동 </param>
        public void LoadInventorySlotDataModel(int slotID, Action<InventorySlotDataModel> onCompleted)
        {
            _realtimeDB.GetReference($"users/{LoginInformation.userKey}/inventorySlots/{slotID}") // 읽을 위치 참조
                       .GetValueAsync() // 해당 위치에서 읽기
                       .ContinueWith(task =>
                       {
                           DataSnapshot snapshot = task.Result;

                           // 해당 위치에서 정상적으로 데이터를 읽었으면
                           if (snapshot.Exists)
                           {
                               InventorySlotDataModel data = 
                                   JsonConvert.DeserializeObject<InventorySlotDataModel>(snapshot.GetRawJsonValue()); // 데이터 역직렬화
                               inventorySlotDataModels[slotID] = data; // DependencySource 갱신
                               onCompleted?.Invoke(data); // 완료 액션
                           }
                       }); // 읽기 끝나고나서 추가 수행
        }
    }
}
