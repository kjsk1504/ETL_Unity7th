using DiceGame.Data;
using DiceGame.Game;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DiceGame.UI
{
    /// <summary>
    /// InventorySlot들에 대한 데이터를 보여주고 유저와의 상호작용(UI)을 관리함.
    /// </summary>
    public class UIInventory : UIPopUpBase
    {
        /// <summary> 슬롯 데이터를 보여줄 기본 단위 원본 </summary>
        [SerializeField] InventorySlot _slotPrefab;
        /// <summary> 슬롯 생성 위치 </summary>
        [SerializeField] Transform _slotContent;
        /// <summary> 생성된 슬롯들 </summary>
        private List<InventorySlot> _slots;
        /// <summary> 슬롯 데이터를 가지고 있는 저장소 </summary>
        private IRepository<InventorySlotDataModel> _repository;
        /// <summary> 선택된 슬롯의 ID (선택되지 않았으면 <see cref="NOT_SELECTED"/> </summary>
        private int _selectedSlotID = NOT_SELECTED;
        /// <summary> 선택되지 않았을 때 적용할 상수 </summary>
        private const int NOT_SELECTED = -1;
        [SerializeField] Image _selectedFollowingItemIcon;
        private List<RaycastResult> _results = new List<RaycastResult>();


        protected override void Awake()
        {
            base.Awake();

            // 초기화
            _repository = GameManager.instance.unitOfWork.inventoryRepository; // 저장소 주소 참조 받아옴 (의존성 주입)
            _slots = new List<InventorySlot>(_repository.GetAllItems().Count()); // 저장소에 있는 데이터 갯수만큼 슬롯이 들어갈 자리 생성
            InventorySlot tmpSlot = null;
            int tmpSlotId = 0;

            // 저장소에 있는 모든 데이터 순회하면서 슬롯 생성 및 갱신
            foreach (var slotData in _repository.GetAllItems())
            {
                tmpSlot = Instantiate(_slotPrefab, _slotContent);
                tmpSlot.slotID = tmpSlotId++;
                tmpSlot.Refresh(slotData.itemID, slotData.itemNum);
                _slots.Add(tmpSlot);
            }

            // 저장소의 데이터가 변경될 때마다 해당 슬롯을 갱신
            _repository.onItemUpdated += (slotID, slotData) =>
            {
                _slots[slotID].Refresh(slotData.itemID, slotData.itemNum);
            };
        }

        public override void InputAction()
        {
            base.InputAction();

            if (Input.GetMouseButtonDown(0))
            {
                // 선택된 슬롯이 없다면
                if (_selectedSlotID == NOT_SELECTED)
                {
                    _results.Clear();
                    Raycast(_results);

                    // 뭔가 상호작용할만한 게 이 캔버스에 있다..!
                    if (_results.Count > 0)
                    {
                        foreach (var result in _results)
                        {
                            // 캐스팅 타겟 중에 슬롯이 있다면
                            if (result.gameObject.TryGetComponent(out InventorySlot slot))
                            {
                                if (_repository.GetItemById(slot.slotID).isEmpty == false)
                                {
                                    Select(slot.slotID);
                                    return;
                                }
                            }
                        }
                    }
                }
                // 선택된 슬롯이 있다면
                else
                {
                    _results.Clear();
                    Raycast(_results);

                    // 뭔가 상호작용할만한 게 이 캔버스에 있다..!
                    if (_results.Count > 0)
                    {
                        foreach (var result in _results)
                        {
                            // 캐스팅 타겟 중에 슬롯이 있다면 해당슬롯을 선택
                            if (result.gameObject.TryGetComponent(out InventorySlot slot))
                            {
                                // 다른 슬롯이 선택됐다면 스왑.
                                if (_selectedSlotID != slot.slotID)
                                {
                                    var selectedSlotData = _repository.GetItemById(_selectedSlotID);
                                    var castedSlotData = _repository.GetItemById(slot.slotID);
                                    _repository.UpdateItem(slot.slotID, new InventorySlotDataModel(selectedSlotData)); 
                                    _repository.UpdateItem(_selectedSlotID, castedSlotData);
                                    Deselect();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (_selectedSlotID != NOT_SELECTED)
                    Deselect();
            }

            // 현재 선택된 슬롯이 있다면, 아이콘이 마우스 포인터 따라다니게 함.
            if (_selectedSlotID != NOT_SELECTED)
                _selectedFollowingItemIcon.transform.position = Input.mousePosition;
        }

        private void Select(int slotID)
        {
            _selectedSlotID = slotID;
            _selectedFollowingItemIcon.sprite = _slots[slotID].sprite;
            _selectedFollowingItemIcon.enabled = true;
        }

        private void Deselect()
        {
            _selectedSlotID = NOT_SELECTED;
            _selectedFollowingItemIcon.enabled = false;
        }
    }
}
