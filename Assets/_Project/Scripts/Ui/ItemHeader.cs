using UnityEngine;
using TMPro;

namespace DigFight
{
    public class ItemHeader : MonoBehaviour
    {
        private TextMeshProUGUI _headerText;

        public void SetHeader(string header)
        {
            if (_headerText == null)
                _headerText = GetComponent<TextMeshProUGUI>();

            _headerText.text = header;
        }
    }
}
