using TMPro;
using UnityEngine;
//using Vuforia;

namespace Scripts
{
    public class AdvancedBillboard : AlignToCamera
    {
       [SerializeField] private bool _fadeOutOverDistance = false;

        private float _zeroAlphaDistanceFar = 5.5f;
        private float _zeroAlphaDistanceClose = 1.0f;
        private float _fullAlphaDistanceFar = 4.5f;
        private float _fullAlphaDistanceClose = 1.5f;

        private Camera _mainCamera;
        private Vector3 _cameraPosition;
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
            _mainCamera = Camera.main;
            // Need to force the text object to be generated so we have valid data to work with right from the start.
            m_TextComponent.ForceMeshUpdate();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (!_fadeOutOverDistance) return;

            if (_mainCamera.transform.position != _cameraPosition)
            {
                var cameraDistance = Vector3.Distance(_mainCamera.transform.position, transform.position);
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
}