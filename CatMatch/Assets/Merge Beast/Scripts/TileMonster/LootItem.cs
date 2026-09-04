using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tiledom
{
    public class LootItem : MonoBehaviour
    {
        Image icon;
        Text txtCount;
        GameObject tick;

        private bool isDone;
        private void Awake() {
            icon = GetComponent<Image>();
            tick = transform.GetChild(0).gameObject;
            txtCount = transform.GetChild(1).GetComponent<Text>();
        }
        public void SetDone(bool done)
        {
            isDone = done;
            tick.SetActive(done);
        }

        public void UpdateIcon(int type)
        {
            icon.sprite = GameManager._Instance.GetSpriteBeast(type);
            icon.SetNativeSize();
        }

        public void UpdateText(string value) {
            txtCount.text = value;
        }

        public bool GetDone() {
            return isDone;
        }
    }
}