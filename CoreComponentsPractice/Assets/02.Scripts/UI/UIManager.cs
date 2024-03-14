using DiceGame.Singleton;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DiceGame.UI
{
    public class UIManager : SingletonMonoBase<UIManager>
    {
        private Dictionary<Type, IUI> _uis = new Dictionary<Type, IUI>();
        public List<IUIScreen> _screens = new List<IUIScreen>();
        private LinkedList<IUIPopUp> _popUps = new LinkedList<IUIPopUp>();
        private List<RaycastResult> _raycastResult = new List<RaycastResult>();


        private void Update()
        {
            if (_popUps.Count > 0)
            {
                if (_popUps.Last.Value.inputActionEnable)
                    _popUps.Last.Value.InputAction();
            }

            if (_screens.Count > 0)
            {
                foreach (var screen in _screens)
                {
                    screen.InputAction();
                }
            }
        }

        /// <summary>
        /// UI 최초 등록
        /// </summary>
        /// <param name="ui"> 등록할 UI </param>
        /// <exception cref="동일한 UI 가 씬에 두개 이상 존재함. 하나 지워야함"> 동일한 UI 가 씬에 두개 이상 존재함. 하나 지워야함. </exception>
        public void Register(IUI ui)
        {
            if (_uis.TryAdd(ui.GetType(), ui))
            {
                if (ui is IUIScreen)
                    _screens.Add((IUIScreen)ui);
            }
            else
            {
                throw new Exception($"[UIManager] : Failed to register {ui.GetType()}... already exist.");
            }
        }

        /// <summary>
        /// UI 인터페이스 검색
        /// </summary>
        /// <typeparam name="T"> 가져오려는 UI 타입 </typeparam>
        /// <returns> UI 인터페이스 </returns>
        /// <exception cref="가져오려는 UI 가 존재하지 않음"> 가져오려는 UI가 존재하지 않음. </exception>
        public T Get<T>()
            where T : IUI
        {
            if (_uis.TryGetValue(typeof(T), out IUI ui))
            {
                return (T)ui;
            }
            else
            {
                throw new Exception($"[UIManager] : Failed to get {typeof(T)}... not exist.");
            }
        }

        /// <summary>
        /// 새로 보여줄 PopUp을 정렬순서 가장 뒤로 보냄.
        /// </summary>
        /// <param name="ui"> 새로 보여줄 PopUp </param>
        public void Push(IUIPopUp ui)
        {
            int sortingOrder = 1;

            // 기존에 PopUp이 하나 이상 존재한다면
            if (_popUps.Last?.Value != null)
            {
                _popUps.Last.Value.inputActionEnable = false; // 기존 PopUp의 입력처리 막음.
                sortingOrder = _popUps.Last.Value.sortingOrder + 1; // 정렬 인덱스 기존 PopUp보다 크게
            }

            ui.inputActionEnable = true; // 새 PopUp의 입력처리 활성화.
            ui.sortingOrder = sortingOrder; // 앞으로 가져오기
            _popUps.Remove(ui); // 새 PopUp이 기존에 존재하던 PopUp이면 제거
            _popUps.AddLast(ui); // 새 PopUp을 가장 뒤에 추가

            if (sortingOrder > 256)
                RearrangePopUpSortingOrders();

            if (_popUps.Count == 1)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
        }

        /// <summary>
        /// PopUp을 제거, 이전 PopUp이 있다면 이전 PopUp의 입력처리 활성화.
        /// </summary>
        /// <param name="ui"> 제거할 popup </param>
        public void Pop(IUIPopUp ui)
        {
            // 제거 하려는 Popup이 마지막이면서 이전 Popup이 있다면 이전 PopUp의 입력처리 활성화.
            if (_popUps.Count >= 2 && _popUps.Last.Value == ui)
                _popUps.Last.Previous.Value.inputActionEnable = true;

            _popUps.Remove(ui); // 제거

            if (_popUps.Count == 0)
            {
                //Cursor.visible = false;
                //Cursor.lockState = CursorLockMode.Locked;
            }
        }

        /// <summary>
        /// 현재 포인터 위치에서 다른 캔버스의 RaycastTarget이 있는지 검출
        /// (마우스포인터가 현재 캔버스의 ui component 말고 다른 캔버스의 ui component 위에 있는지 검출)
        /// </summary>
        /// <param name="ui"> 현재 ui </param>
        /// <param name="other"> 다른 캔버스 ui </param>
        /// <param name="hovered"> 마우스가 올라가 있는 캔버스 </param>
        /// <returns> 검출 여부 </returns>
        public bool TryCastOther(IUI ui, out IUI other, out GameObject hovered)
        {
            other = null;
            hovered = null;
            _raycastResult.Clear();

            for (LinkedListNode<IUIPopUp> node = _popUps.Last; node != null; node = node.Previous)
            {
                node.Value.Raycast(_raycastResult);

                if (_raycastResult.Count > 0)
                {
                    hovered = _raycastResult[0].gameObject;

                    if (node.Value == ui)
                        return false;
                    // GraphicRaycaster로 캐스팅 할수있는 대상은
                    // Canvas 하위에 있는 RaycastTarget 프로퍼티가 true인 UI Component들만 가능. 
                    // 즉, _rayCastResult에 들어온 GameObject는 반드시 어떤 Canvas에 종속되어있는
                    // UGUI Component임. 
                    // 따라서 IUI 인터페이스는 해당 Component가 종속된 Canvas에 있으므로 
                    // root까지 올라와서 IUI 인터페이스를 찾아야함.
                    other = hovered.transform.root.GetComponent<IUI>();
                    return true;
                }
            }

            foreach (var screen in _screens)
            {
                screen.Raycast(_raycastResult);

                if (_raycastResult.Count > 0)
                {
                    hovered = _raycastResult[0].gameObject;

                    if (screen == ui)
                        return false;

                    other = hovered.transform.root.GetComponent<IUI>();
                    return true;
                }
            }

            return false;
        }

        public List<RaycastResult> RayCastAll()
        {
            _raycastResult.Clear();

            for (LinkedListNode<IUIPopUp> node = _popUps.Last; node != null; node = node.Previous)
            {
                node.Value.Raycast(_raycastResult);
            }

            foreach (var screen in _screens)
            {
                screen.Raycast(_raycastResult);
            }

            return _raycastResult;
        }

        private void RearrangePopUpSortingOrders()
        {
            int sortingOrder = 1;
            foreach (var popUp in _popUps)
            {
                popUp.sortingOrder = sortingOrder++;
            }
        }
    }
}
