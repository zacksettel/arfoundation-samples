﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Visualizes the eye poses for an <see cref="ARFace"/>.
    /// </summary>
    /// <remarks>
    /// Face space is the space where the origin is the transform of an <see cref="ARFace"/>.
    /// </remarks>
    [RequireComponent(typeof(ARFace))]
    public class zLReyesPoseVisulizer : MonoBehaviour
    {
        [SerializeField]
        GameObject m_EyePrefab;

        [SerializeField]
        GameObject leftEyeOSCnodePrefab;

        [SerializeField]
        GameObject rightEyeOSCnodePrefab;

        public GameObject eyePrefab
        {
            get => m_EyePrefab;
            set => m_EyePrefab = value;
        }

        GameObject m_LeftEyeGameObject;
        GameObject m_RightEyeGameObject;

        ARFace m_Face;
        XRFaceSubsystem m_FaceSubsystem;

        void Awake()
        {
            if (!leftEyeOSCnodePrefab || !rightEyeOSCnodePrefab)
            {
                Debug.LogError($"{GetType()}: Missing OSC Node Prefab, aborting");
                Destroy(this);
                return;
            }

            if (!leftEyeOSCnodePrefab.GetComponent<OscJack.OscPropertySender>() || !rightEyeOSCnodePrefab.GetComponent<OscJack.OscPropertySender>())
            {
                Debug.LogError($"{GetType()}: OSC Node Prefab missing OscPropertySender, aborting");
                Destroy(this);
            }

          

            m_Face = GetComponent<ARFace>();
        }

        void CreateEyeGameObjectsIfNecessary()
        {
            if (m_Face.leftEye != null && m_LeftEyeGameObject == null)
            {
                m_LeftEyeGameObject = Instantiate(m_EyePrefab, m_Face.leftEye);
                m_LeftEyeGameObject.SetActive(false);

                GameObject gob = Instantiate(leftEyeOSCnodePrefab, m_LeftEyeGameObject.transform);
            }
            if (m_Face.rightEye != null && m_RightEyeGameObject == null)
            {
                m_RightEyeGameObject = Instantiate(m_EyePrefab, m_Face.rightEye);
                m_RightEyeGameObject.SetActive(false);
                GameObject gob = Instantiate(rightEyeOSCnodePrefab, m_RightEyeGameObject.transform);
            }
        }

        void SetVisible(bool visible)
        {
            if (m_LeftEyeGameObject != null && m_RightEyeGameObject != null)
            {
                m_LeftEyeGameObject.SetActive(visible);
                m_RightEyeGameObject.SetActive(visible);
            }
        }


        void OnEnable()
        {
            var faceManager = FindObjectOfType<ARFaceManager>();
            if (faceManager != null && faceManager.subsystem != null && faceManager.descriptor.supportsEyeTracking)
            {
                m_FaceSubsystem = (XRFaceSubsystem)faceManager.subsystem;
                SetVisible((m_Face.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready));
                m_Face.updated += OnUpdated;
            }
            else
            {
                enabled = false;
            }
        }

        void OnDisable()
        {
            m_Face.updated -= OnUpdated;
            SetVisible(false);
        }

        void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
        {
            CreateEyeGameObjectsIfNecessary();
            SetVisible((m_Face.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready));
        }
    }
}