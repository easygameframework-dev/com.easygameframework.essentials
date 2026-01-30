using System;
using UnityEngine;

namespace EasyGameFramework.Essentials
{
    public class UIRotateAround : MonoBehaviour
    {
        [SerializeField] private RectTransform _target;
        [SerializeField] private RectTransform _center;

        [SerializeField] private float _duration = 2f;

        private float _angle;
        private float _radius;

        private void Start()
        {
            if (_target != null && _center != null)
            {
                var offset = _target.anchoredPosition - _center.anchoredPosition;
                _radius = offset.magnitude;

                _angle = Mathf.Atan2(offset.y, offset.x);
            }
        }

        private void Update()
        {
            if (_target == null || _center == null || _duration <= 0f)
                return;

            _angle += Time.deltaTime * Mathf.PI * 2f / _duration;

            if (_angle > Mathf.PI * 2f)
                _angle -= Mathf.PI * 2f;

            var offset = new Vector2(Mathf.Cos(_angle), Mathf.Sin(_angle)) * _radius;
            _target.anchoredPosition = _center.anchoredPosition + offset;
        }
    }
}
