using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Voltage
{
    public class EmissionFlicker : MonoBehaviour
    {
        [SerializeField] private bool m_FlickerWhileEnabled = true;
        [SerializeField] private Material m_Material = null;
        [SerializeField] private Color m_BaseColor = Color.white;
        [SerializeField] private Color m_EmissionColor = Color.white;
        [SerializeField] private float m_MinIntensity = 0;
        [SerializeField] private float m_MaxIndensity = 1;
        [SerializeField] private AnimationCurve m_IndensityCurve = null;
        [SerializeField] private float m_FlilckerSpeed = 1;
        [SerializeField] private List<Renderer> m_RendererList = null;
        
        private Material _material = null;
        private LocalKeyword _kwEmission;
        private bool _isFlickering = false;
        private float _startTime = 0;
        
        private void Awake()
        {
            _material = Instantiate(m_Material);
            _material.SetColor("_Color", m_BaseColor);
            _material.SetColor("_EmissionColor", m_EmissionColor * m_MinIntensity);
            
            _kwEmission = new LocalKeyword(_material.shader, "_EMISSION");
            
            foreach (Renderer renderer in m_RendererList)
            {
                renderer.material = _material;
            }
        }
        
        private void OnEnable()
        {
            if (m_FlickerWhileEnabled) StartFlicker();
        }
        
        private void OnDisable()
        {
            if (m_FlickerWhileEnabled) StopFlicker();
        }
        
        private void Update()
        {
            if (_isFlickering)
            {
                float time = (Time.time - _startTime) * m_FlilckerSpeed;
                float value = m_IndensityCurve.Evaluate(time);
                float indensity = m_MinIntensity + (m_MaxIndensity - m_MinIntensity) * value;
                _material.SetColor("_EmissionColor", m_EmissionColor * indensity);
            }
        }
        
        private void OnDestroy()
        {
            Destroy(_material);
        }
        
        public void StartFlicker()
        {
            _isFlickering = true;
            _material.SetKeyword(_kwEmission, true);
            _material.SetColor("_EmissionColor", m_EmissionColor * m_MinIntensity);
            _startTime = Time.time;
        }
        
        public void StopFlicker()
        {
            _isFlickering = false;
            _material.SetKeyword(_kwEmission, false);
            _material.SetColor("_EmissionColor", m_EmissionColor * m_MinIntensity);
        }
    }
}