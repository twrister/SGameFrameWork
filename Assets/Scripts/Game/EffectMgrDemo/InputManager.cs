using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace SthGame.EffectMgrDemo
{
    /// <summary>
    /// A simple Input Manager for a Runner game.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        /// <summary>
        /// Returns the InputManager.
        /// </summary>
        public static InputManager Instance => s_Instance;
        static InputManager s_Instance;

        [SerializeField]
        float m_InputSensitivity = 1.5f;

        bool m_HasInput;
        Vector3 m_InputPosition;
        Vector3 m_PreviousInputPosition;

        void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;
        }

        void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }

        void Update()
        {
            // if (PlayerController.Instance == null)
            // {
            //     return;
            // }

#if UNITY_EDITOR
            m_InputPosition = Mouse.current.position.ReadValue();

            if (Mouse.current.leftButton.isPressed)
            {
                if (!m_HasInput)
                {
                    m_PreviousInputPosition = m_InputPosition;
                }
                m_HasInput = true;
            }
            else
            {
                m_HasInput = false;
            }
#else
            if (Touch.activeTouches.Count > 0)
            {
                m_InputPosition = Touch.activeTouches[0].screenPosition;

                if (!m_HasInput)
                {
                    m_PreviousInputPosition = m_InputPosition;
                }
                
                m_HasInput = true;
            }
            else
            {
                m_HasInput = false;
            }
#endif

            if (m_HasInput && Time.time > m_LastClickTime + m_ClickInterval)
            {
                float normalizedDeltaPosition = (m_InputPosition.x - m_PreviousInputPosition.x) / Screen.width * m_InputSensitivity;
                
                // Logger.Log($"m_InputPosition = {m_InputPosition}");
                // PlayerController.Instance.SetDeltaPosition(normalizedDeltaPosition);
                
                ray.origin = Camera.main.transform.position;
                ray.direction = Camera.main.ScreenPointToRay(m_InputPosition).direction;

                if (Physics.RaycastNonAlloc(ray, m_HitInfos) > 0)
                {
                    Vector3 clickPoint = m_HitInfos[0].point;
                    

                    GameObject effect = m_EffectList[m_EffectIndex];
                    EffectMgr.Instance.Play(effect, clickPoint, Quaternion.identity);
                    m_EffectIndex = (m_EffectIndex + 1) % m_EffectList.Count;
                    Debug.Log($"clickPoint = {clickPoint}, effect = {effect.name}");
                }
                m_LastClickTime = Time.time;
            }

            m_PreviousInputPosition = m_InputPosition;
        }

        #region 点地板
        
        private RaycastHit[] m_HitInfos = new RaycastHit[4];
        private Ray ray;

        public List<GameObject> m_EffectList;
        private int m_EffectIndex;

        private float m_ClickInterval = 0.5f;
        private float m_LastClickTime = 0f;

        #endregion
    }
}

