using DiceGame.Data;
using DiceGame.Game;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DiceGame.UI
{
    public class UIInventory : UIPopUpBase
    {
        [SerializeField] InventorySlot _slotPrefab;
        [SerializeField] Transform _slotContent;
        private List<InventorySlot> _slots;
        private IRepository<InventorySlotDataModel> _repository;


        protected override void Awake()
        {
            base.Awake();

            _repository = GameManager.instance.unitOfWork.inventoryRepository;
            _slots = new List<InventorySlot>(_repository.GetAllItems().Count());
            InventorySlot tmpSlot = null;
            int tmpSlotID = 0;

            foreach (var slotData in _repository.GetAllItems())
            {
                tmpSlot = Instantiate(_slotPrefab, _slotContent);
                tmpSlot.slotID = tmpSlotID++;
                tmpSlot.Refresh(slotData.itemID, slotData.itemNum);
                _slots.Add(tmpSlot);
            }

            _repository.onItemUpdated += (slotID, slotData) =>
            {
                _slots[slotID].Refresh(slotData.itemID, slotData.itemNum);
            };
        }
    }
}