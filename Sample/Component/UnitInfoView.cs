using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HH {

    public class UnitInfoView : MonoBehaviour {

        public GameObject goActive;
        public Text txtString;
        public Image imgReplace;
        public Image imgColorChange;
        public Button btnCallback;

        public UnitInfoView Set(bool show) {
            if (goActive != null) {
                goActive.SetActive(show);
            }
            return this;
        }

        public UnitInfoView Set(string show, bool autoActive = false, params object[] objs) {
            if (txtString != null && show != null) {
                if (objs != null && objs.Length > 0) {
                    show = string.Format(show, objs);
                }
                txtString.text = show;
            }
            if (autoActive) {
                Set(!string.IsNullOrEmpty(show));
            }
            return this;
        }

        public UnitInfoView Set(Sprite show, bool autoActive = false) {
            if (imgReplace != null && show != null) {
                imgReplace.sprite = show;
            }
            if (autoActive) {
                Set(show != null);
            }
            return this;
        }

        public UnitInfoView Set(Color show) {
            if (imgColorChange != null) {
                imgColorChange.color = show;
            }
            return this;
        }

        public UnitInfoView Set(UnityAction callback, bool singleListener = true, bool autoActive = false) {
            if (btnCallback != null) {
                if (singleListener) {
                    btnCallback.onClick.RemoveAllListeners();
                }
                btnCallback.onClick.AddListener(callback);
            }
            if (autoActive) {
                Set(callback != null);
            }
            return this;
        }

#if UNITY_EDITOR
        [ContextMenu("挂载组件")]
        void 挂载组件() {
            goActive = gameObject;
            txtString = GetComponentInChildren<Text>();
            imgReplace = GetComponentInChildren<Image>();
            imgColorChange = imgReplace;
            btnCallback = GetComponentInChildren<Button>();
        }
#endif
    }
}