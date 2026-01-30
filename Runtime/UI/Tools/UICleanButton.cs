using EasyToolkit.Fluxion.Core;
using EasyToolkit.Fluxion.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EasyGameFramework.Essentials
{
    [AddComponentMenu("UI/Clean Button")]
    public class UICleanButton : Button
    {
        [SerializeField] private float _fadeTime = 0.3f;
        [SerializeField] private float _onHoverAlpha = 0.6f;
        [SerializeField] private float _onClickAlpha = 0.4f;

        private CanvasGroup _canvasGroup;
        private IFlow _previousFlow;

        protected override void Awake()
        {
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            _previousFlow?.Kill();
            _previousFlow = _canvasGroup.FlowFade(_onHoverAlpha, _fadeTime);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            _previousFlow?.Kill();
            _previousFlow = _canvasGroup.FlowFade(1.0f, _fadeTime);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _canvasGroup.alpha = _onClickAlpha;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            _canvasGroup.alpha = 1.0f;
        }
    }
}
