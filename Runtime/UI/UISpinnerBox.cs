using System;
using EasyToolkit.Fluxion;
using EasyToolkit.Fluxion.Core;
using EasyToolkit.Fluxion.Extensions;
using EasyToolkit.Inspector.Attributes;
using TMPro;
using UnityEngine;

namespace EasyGameFramework.Essentials
{
    public class UISpinnerBox : UIPanel
    {
        [Title("Bindings")]
        [SerializeField] private TextMeshProUGUI _percentageText;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        private float _percentage;
        private float _destinationPercentage;
        private IFlow _destinationPercentageFlow;

        public float Percentage
        {
            get => _percentage;
            set
            {
                _percentage = value;
                _percentageText.text = $"{(int)(_percentage * 100f)}%";
            }
        }

        public string Description
        {
            get => _descriptionText.text;
            set => _descriptionText.text = value;
        }

        public Func<string> DescriptionGetter;

        public void SetDestinationPercentage(float percentage, float duration, Action arrived)
        {
            if (_destinationPercentageFlow != null)
            {
                _destinationPercentageFlow.Kill();
                _destinationPercentageFlow = null;
            }

            _destinationPercentageFlow = FluxFactory.To(
                () => Percentage,
                value => Percentage = value,
                percentage, duration)
                .OnKill(() => arrived?.Invoke());
        }

        protected override void OnSafeOpen(object userData)
        {
            base.OnSafeOpen(userData);
            Percentage = 0;
            Description = "";
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (DescriptionGetter != null)
            {
                Description = DescriptionGetter();
            }
        }
    }
}
