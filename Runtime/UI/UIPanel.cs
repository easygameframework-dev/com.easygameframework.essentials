using System;
using EasyToolkit.Fluxion;
using EasyToolkit.Fluxion.Core;
using EasyToolkit.Fluxion.Extensions;
using EasyToolkit.Inspector.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace EasyGameFramework.Essentials
{
    public enum UIAnimation
    {
        None,
        FadeIn,
        FadeOut,
        PopIn,
        PopOut,
    }

    [EasyInspector]
    public abstract class UIPanel : UIFormLogic
    {
        [Title("Animation")]
        [SerializeField] private UIAnimation _openAnimation = UIAnimation.FadeIn;
        [SerializeField] private UIAnimation _closeAnimation = UIAnimation.FadeOut;

        [SerializeField] private float _openDuration = 0.3f;
        [SerializeField] private float _closeDuration = 0.3f;
        [SerializeField] private float _backgroundAlpha = 0.3f;

        private RectTransform _rectTransform;
        private IFlux _previousFlux;

        private RectTransform[] _targets;
        private Image _backgroundImage;
        private CanvasGroup _canvasGroup;

        private bool _isInitializing;
        private Action _pendingActionAfterInitialize;

        public event Action OpenCompleted;
        public event Action CloseCompleted;

        public UIAnimation OpenAnimation
        {
            get => _openAnimation;
            set => _openAnimation = value;
        }

        public UIAnimation CloseAnimation
        {
            get => _closeAnimation;
            set => _closeAnimation = value;
        }

        public float OpenDuration
        {
            get => _openDuration;
            set => _openDuration = value;
        }

        public float CloseDuration
        {
            get => _closeDuration;
            set => _closeDuration = value;
        }

        public float BackgroundAlpha
        {
            get => _backgroundAlpha;
            set => _backgroundAlpha = value;
        }

        public virtual void CloseSelf()
        {
            GameEntry.GetComponent<UIComponent>().CloseUIForm(UIForm);
        }

        protected sealed override void OnInit(object userData)
        {
            base.OnInit(userData);

            _backgroundImage = gameObject.GetOrAddComponent<Image>();
            if (_backgroundImage != null)
            {
                _backgroundImage.color = new Color(0, 0, 0, 0);
            }

            _rectTransform = GetComponent<RectTransform>();
            // _rectTransform.anchorMin = Vector2.zero;
            // _rectTransform.anchorMax = Vector2.one;
            // _rectTransform.anchoredPosition = Vector2.zero;
            // _rectTransform.sizeDelta = Vector2.zero;
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
            _canvasGroup.blocksRaycasts = false;

            _targets = new RectTransform[_rectTransform.childCount];
            for (int i = 0; i < _targets.Length; i++)
            {
                var target = _rectTransform.GetChild(i).GetComponent<RectTransform>();
                target.gameObject.GetOrAddComponent<CanvasGroup>().alpha = 0;
                _targets[i] = target;
            }

            _isInitializing = true;
            OnInit(userData, () =>
            {
                _isInitializing = false;
                _pendingActionAfterInitialize?.Invoke();
                _pendingActionAfterInitialize = null;
            },
            exception =>
            {
                throw new NotImplementedException();
            });
        }

        protected virtual void OnInit(object userData, Action completed, Action<Exception> failed)
        {
            completed();
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            if (_isInitializing)
            {
                _pendingActionAfterInitialize += () => OnSafeOpen(userData);
            }
            else
            {
                OnSafeOpen(userData);
            }
        }

        protected virtual void OnSafeOpen(object userData)
        {
            DoAnimation(_openAnimation, _openDuration, () =>
            {
                OnOpenCompleted();
                OpenCompleted?.Invoke();
            });
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            gameObject.SetActive(true);

            DoAnimation(_closeAnimation, _closeDuration, () =>
            {
                gameObject.SetActive(false);
                OnCloseCompleted();
                CloseCompleted?.Invoke();
            });
        }

        protected virtual void OnOpenCompleted()
        {
        }

        protected virtual void OnCloseCompleted()
        {
        }

        private void DoAnimation(UIAnimation uiAnimation, float duration, Action completed)
        {
            if (_previousFlux != null)
            {
                _previousFlux.Kill();
                _previousFlux = null;
            }

            var sequence = FluxFactory.Sequence();
            switch (uiAnimation)
            {
                case UIAnimation.FadeIn:
                {
                    sequence.Join(_backgroundImage.FlowFade(_backgroundAlpha, duration));
                    foreach (var target in _targets)
                    {
                        sequence.Join(target.GetComponent<CanvasGroup>().FlowFade(1f, duration));
                    }

                    sequence.AppendCallback(() =>
                    {
                        _canvasGroup.blocksRaycasts = true;
                        completed?.Invoke();
                    });
                    break;
                }
                case UIAnimation.FadeOut:
                {
                    _canvasGroup.blocksRaycasts = false;
                    sequence.Join(_backgroundImage.FlowFade(0f, duration));
                    foreach (var target in _targets)
                    {
                        sequence.Join(target.GetComponent<CanvasGroup>().FlowFade(0f, duration));
                    }

                    sequence.AppendCallback(() => completed?.Invoke());
                    break;
                }
                case UIAnimation.PopIn:
                {
                    sequence.Join(_backgroundImage.FlowFade(_backgroundAlpha, duration));
                    foreach (var target in _targets)
                    {
                        target.localScale = Vector3.one * 0.3f;
                        target.GetComponent<CanvasGroup>().alpha = 1;
                        sequence.Join(target.FlowScale(1f, duration).WithEase.OutBack());
                    }
                    sequence.AppendCallback(() =>
                    {
                        _canvasGroup.blocksRaycasts = true;
                        completed?.Invoke();
                    });
                    break;
                }
                case UIAnimation.PopOut:
                {
                    _canvasGroup.blocksRaycasts = false;
                    sequence.Join(_backgroundImage.FlowFade(0f, duration));
                    foreach (var target in _targets)
                    {
                        target.GetComponent<CanvasGroup>().alpha = 1;
                        sequence.Join(target.FlowScale(0.3f, duration).WithEase.InBack());
                    }
                    sequence.AppendCallback(() => completed?.Invoke());
                    break;
                }
                case UIAnimation.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(uiAnimation), uiAnimation, null);
            }
            _previousFlux = sequence;
        }
    }
}
