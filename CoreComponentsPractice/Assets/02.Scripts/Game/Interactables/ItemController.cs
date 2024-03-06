using DiceGame.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DiceGame.Game.Interactables
{
    public class ItemController : MonoBehaviour, IInteractable
    {
        [field: SerializeField] public int itemID { get; private set; }
        [field: SerializeField] public int itemNum { get; private set; }


        public void SetUp(InventorySlotDataModel slotData)
        {
            itemID = slotData.itemID;
            itemNum = slotData.itemNum;
        }

        public void Interaction()
        {
            PickUp();
        }

        /* 내 숙제
        private void PickUp()
        {
            var inventoryRepository = GameManager.instance.unitOfWork.inventoryRepository;
            List<InventorySlotDataModel> inventoryItemLists = (List<InventorySlotDataModel>) inventoryRepository.GetAllItems();

            // 인벤토리의 모든 아이템 슬롯을 순회하면서
            for (int i = 0; i < inventoryItemLists.Count; i++)
            {
                // 인벤토리 슬롯 아이템과 주운 아이템의 id가 같으면
                if (inventoryItemLists[i].itemID == itemID)
                {
                    // 슬롯 아이템이 최대 갯수보다 얼마나 모자른지 비교
                    int diff = ItemInfoResources.instance[itemID].maxNum - inventoryItemLists[i].itemNum;

                    // 슬롯 아이템의 갯수가 최대면 넘어감
                    if (diff == 0)
                        continue;
                    
                    // 슬롯 아이템의 남은 갯수가 주운 아이템의 갯수보다 많으면 주운 아이템의 갯수만큼 더하고 주운 아이템의 갯수는 0으로 설정
                    if (diff > itemNum)
                    {
                        inventoryItemLists[i].itemNum += itemNum;
                        inventoryRepository.UpdateItem(i, inventoryItemLists[i]);
                        itemNum = 0;
                        break;
                    }
                    // 슬롯 아이템의 남은 갯수가 주운 아이템 갯수보다 적으면 남은 갯수만큼 더하고 나머지는 주운 아이템의 갯수에 반영
                    else
                    {
                        inventoryItemLists[i].itemNum += diff;
                        inventoryRepository.UpdateItem(i, inventoryItemLists[i]);
                        itemNum -= diff;
                    }
                }
                i++;
            }

            // 주운 아이템의 갯수가 없으면 주운 아이템 파괴
            if (itemNum == 0)
                Destroy(gameObject);

            // 주운 아이템 갯수가 남아 있으면 빈 슬롯에 주운 아이템의 나머지를 적용
            for (int i = 0; i < inventoryItemLists.Count; i ++)
            {
                if (inventoryItemLists[i].isEmpty)
                {
                    inventoryItemLists[i].itemID = itemID;
                    inventoryItemLists[i].itemNum = itemNum;
                    inventoryRepository.UpdateItem(i, inventoryItemLists[i]);
                    itemNum = 0;
                    break;
                }
            }
        }
        // 문제점 : slotdata를 직접적으로 수정하고 updateitem의 콜백함수 부분만 일부 사용하기 위해 호출함
        // 데이터 영역이 updateitem을 호출하기 전에 이미 수정되어 있음 (직접 수정하기 보단 수정함수 호출로 수정해야 함)
        */

        private void PickUp()
        {
            // todo -> 
            // 저장소를 전체 순회하면서
            // 현재 ID의 아이템을 차례대로 itemInfo.numMax까지 채우는 것을 시도하면서 모든 itemNum을 가능한 만큼 소모하고
            // 남은 갯수가 있으면 DB를 갱신한 후에 itemNum 남은 갯수로 갱신.

            var inventoryRepository = GameManager.instance.unitOfWork.inventoryRepository;

            int slotID = 0;
            int maxNum = ItemInfoResources.instance[itemID].maxNum;
            int remains = itemNum;

            foreach (var slotData in inventoryRepository.GetAllItems().ToList())
            {
                if (slotData.isEmpty || 
                    (slotData.itemID == itemID && slotData.itemNum < maxNum))
                {
                    int vacancy = maxNum - slotData.itemNum;
                    remains = remains - vacancy;

                    // 다 채우고 남은 갯수가 양수면, 현재 슬롯은 최대치로 다 채우고도 남은 것이므로 최대치로 설정
                    if (remains > 0)
                    {
                        inventoryRepository.UpdateItem(slotID, new InventorySlotDataModel(itemID, maxNum));
                    }
                    // 다 채우고 남은 갯수가 음수면, 다 못채웠으므로 최대치에서 다 못 채운 양만큼 더해야 함 (음수를 더함)
                    else if (remains < 0)
                    {
                        inventoryRepository.UpdateItem(slotID, new InventorySlotDataModel(itemID, maxNum + remains));
                        remains = 0;
                        break;
                    }
                    // 다 채우고 남은 갯수가 0이면 딱 현재 슬롯까지 남은 갯수로 다 채운 상황.
                    else
                    {
                        inventoryRepository.UpdateItem(slotID, new InventorySlotDataModel(itemID, maxNum));
                        break;
                    }
                }

                slotID++;
            }

            // 주운 아이템의 갯수가 없으면 주운 아이템 파괴
            if (remains == 0)
                Destroy(gameObject);
            else
                itemNum = remains;
        }

    }
}
