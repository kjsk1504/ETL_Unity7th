using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using DiceGame.UI;

namespace Assignment.DiceGame.UI
{
    public class A_UIWarningWindow : UIPopUpBase
    {
        private TMP_Text _message;
        private Button _confirm;


        protected override void Awake()
        {
            base.Awake();

            _message = transform.Find("Panel/Text (TMP) - Message").GetComponent<TMP_Text>();
            _confirm = transform.Find("Panel/Button - Confirm").GetComponent<Button>();
        }

        public void Show(string message, UnityAction onConfirm = null)
        {
            _message.text = message;

            _confirm.onClick.RemoveAllListeners();
            _confirm.onClick.AddListener(Hide);

            if (onConfirm != null)
                _confirm.onClick.AddListener(onConfirm);

            base.Show();
        }
    }
}