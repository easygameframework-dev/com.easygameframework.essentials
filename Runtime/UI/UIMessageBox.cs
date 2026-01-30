using System;
using System.Collections.Generic;
using EasyGameFramework.Core.Resource;
using EasyToolkit.Inspector.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EasyGameFramework.Essentials
{
    public class UIMessageBox : UIPanel
    {
        [Title("Bindings")]
        [SerializeField] private TextMeshProUGUI _titleText;

        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private RectTransform _buttonGroup;

        [Title("Settings")]
        [SerializeField] private AssetReference _buttonAsset;

        [SerializeField] private Vector2 _buttonSize = new Vector2(200, 60);

        private GameObject _buttonPrefab;

        private readonly Queue<(string text, Action<Button> onClick)> _addButtonsQueue = new();

        public string Title
        {
            get => _titleText.text;
            set => _titleText.text = value;
        }

        public string Message
        {
            get => _messageText.text;
            set => _messageText.text = value;
        }

        public void AddButton(string text, Action<Button> onClick)
        {
            _addButtonsQueue.Enqueue((text, onClick));
        }

        public void ClearButtons()
        {
            for (int i = _buttonGroup.childCount - 1; i >= 0; i--)
            {
                Destroy(_buttonGroup.GetChild(i).gameObject);
            }
        }

        protected override void OnInit(object userData, Action completed, Action<Exception> failed)
        {
            base.OnInit(userData);
            GameEntry.Resource.LoadAsset(
                _buttonAsset.ToAssetAddress(),
                new LoadAssetCallbacks(
                    (address, asset, duration, data) =>
                    {
                        _buttonPrefab = (GameObject)asset;
                        completed();
                    },
                    (address, status, message, data) => { failed(new Exception(message)); }),
                typeof(GameObject));
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            ClearButtons();
        }

        private void Update()
        {
            if (_buttonPrefab != null)
            {
                while (_addButtonsQueue.Count > 0)
                {
                    var (text, onClick) = _addButtonsQueue.Dequeue();
                    var instantiate = Instantiate(_buttonPrefab, _buttonGroup);
                    instantiate.GetComponent<RectTransform>().sizeDelta = _buttonSize;
                    instantiate.GetComponentInChildren<TextMeshProUGUI>().text = text;

                    var button = instantiate.GetComponent<Button>();
                    button.onClick.AddListener(() => onClick(button));
                }
            }
        }
    }
}
