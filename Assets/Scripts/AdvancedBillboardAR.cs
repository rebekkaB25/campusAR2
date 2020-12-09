using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;


namespace Scripts
{
    public class AdvancedBillboardAR : AlignToCamera
    {
        [SerializeField] private bool _fadeOutOverDistance = true;

        private float _zeroAlphaDistanceFar = 3f;
        private float _zeroAlphaDistanceClose = 0.5f;
        private float _fullAlphaDistanceFar = 2f;
        private float _fullAlphaDistanceClose = 0.75f;

        private SpriteRenderer[] _spriteRenderers;
        private TMP_Text m_TextComponent;
        private BillboardLine _billboardLine;

        void Awake()
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            m_TextComponent = GetComponentInChildren<TMP_Text>();
            _billboardLine = GetComponent<BillboardLine>();
        }

        void Start()
        {
            // Need to force the text object to be generated so we have valid data to work with right from the start.
            m_TextComponent.ForceMeshUpdate();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            //var camera = Camera.main; //cameratests

            if (!_fadeOutOverDistance) return;

            var cameraDistance = Vector3.Distance(Vector3.zero, transform.position);
            float alpha;
            if (cameraDistance > _zeroAlphaDistanceFar)
                alpha = 0f;
            else if (cameraDistance < _zeroAlphaDistanceClose)
                alpha = 0f;
            else if (cameraDistance < _fullAlphaDistanceClose)
                alpha = 1f - (cameraDistance - _fullAlphaDistanceClose) / (_zeroAlphaDistanceClose - _fullAlphaDistanceClose);
            else if (cameraDistance < _fullAlphaDistanceFar)
                alpha = 1f;
            else
                alpha = 1f - (cameraDistance - _fullAlphaDistanceFar) / (_zeroAlphaDistanceFar - _fullAlphaDistanceFar);

            foreach (var spriteRenderer in _spriteRenderers)
            {
                if (spriteRenderer.enabled)
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            }

            if (m_TextComponent)
                m_TextComponent.alpha = alpha;

            if (_billboardLine)
                _billboardLine.SetAlpha(alpha);
        }
    }
}