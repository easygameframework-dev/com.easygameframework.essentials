using EasyToolkit.Core.Mathematics;
using EasyToolkit.Fluxion;
using EasyToolkit.Fluxion.Core;
using EasyToolkit.Fluxion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace EasyGameFramework.Essentials
{
    public class UIProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _barImage;

        private IFlow _previousFlow;

        public float Amount => _barImage.fillAmount;

        public void SetAmount(float amount, float easeDuration = 0f)
        {
            if (_previousFlow != null && _previousFlow.IsActive())
            {
                _previousFlow.Kill();
            }

            if (easeDuration.IsApproximatelyOf(0f))
            {
                _barImage.fillAmount = amount;
                return;
            }

            _previousFlow = FluxFactory.To(
                () => _barImage.fillAmount,
                newValue => _barImage.fillAmount = newValue,
                amount,
                easeDuration);
        }
    }
}
