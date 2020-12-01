using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif
using OscJack;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Visualizes the eye poses for an <see cref="ARFace"/>.
    /// </summary>
    /// <remarks>
    /// Face space is the space where the origin is the transform of an <see cref="ARFace"/>.
    /// </remarks>
    [RequireComponent(typeof(ARFace))]
    public class eyesOnlyVisualizer : MonoBehaviour
    {

        [SerializeField] private string ipAddress = "localhost";
        [SerializeField] private int udpPort = 12345;
        [SerializeField] private float oscTXdelta = 0.0333f; // osc TX sending rate   
        private OscClient _client;

        [SerializeField]
        GameObject m_EyePrefab;

        public GameObject eyePrefab
        {
            get => m_EyePrefab;
            set => m_EyePrefab = value;
        }

        GameObject m_LeftEyeGameObject;
        GameObject m_RightEyeGameObject;

        ARFace m_Face;
        XRFaceSubsystem m_FaceSubsystem;

        private string deviceName; 

        void Awake()
        {
            string rawstring;
            m_Face = GetComponent<ARFace>();
            rawstring = SystemInfo.deviceName;
            deviceName = rawstring.Replace(' ', '_');

            guiHelper.onIPconnectDelegate += connectToOscServer;
        }

        private void Start()
        {
            connectToOscServer();
        }



        void connectToOscServer()
        {
            ipAddress = guiHelper.ipAddress;

            Debug.Log($"{GetType()}: connectToOscServer(): Connecting with: ip:{ipAddress}:{udpPort}");

            if (_client != null)
            {
                _client.Dispose();
            }

            _client = OscMaster.GetSharedClient(ipAddress, udpPort);
        }


        float lastTXtime = 0f;


            

        private void Update()
        {
            if (_client == null) return;

            if (Time.realtimeSinceStartup - lastTXtime < oscTXdelta) return;

            lastTXtime = Time.realtimeSinceStartup;

            string oscAddr = "/kinectAzure/"+ deviceName+"/face";

            if (m_Face)
            {
                Vector3 cameraPos = Camera.current.transform.position;
                Vector3 pos = cameraPos - m_Face.transform.position;

                Quaternion rotDiff = Quaternion.Inverse(Camera.current.transform.rotation) * m_Face.transform.rotation ;

                Vector3 eulers = rotDiff.eulerAngles;

                _client.Send(oscAddr + "/position", pos.x, pos.y, pos.z);
                _client.Send(oscAddr + "/eulers", eulers.x, eulers.y, eulers.z);
            }

            if (m_LeftEyeGameObject)
            {
                Vector3 pos = m_LeftEyeGameObject.transform.localPosition;
                Vector3 eulers = m_LeftEyeGameObject.transform.localEulerAngles;
                _client.Send(oscAddr + "/leftEye/position", pos.x, pos.y, pos.z);
                _client.Send(oscAddr + "/leftEye/eulers", eulers.x, eulers.y, eulers.z);
            }

            if (m_RightEyeGameObject)
            {
                Vector3 pos = m_RightEyeGameObject.transform.localPosition;
                Vector3 eulers = m_RightEyeGameObject.transform.localEulerAngles;
                _client.Send(oscAddr + "/rightEye/position", pos.x, pos.y, pos.z);
                _client.Send(oscAddr + "/rightEye/eulers", eulers.x, eulers.y, eulers.z);
                //Debug.Log($"zeye pos: {m_RightEyeGameObject.transform.position} rot: {transform.localRotation.eulerAngles}");
            }
        }

        void CreateEyeGameObjectsIfNecessary()
        {
            if (m_Face.leftEye != null && m_LeftEyeGameObject == null)
            {
                m_LeftEyeGameObject = Instantiate(m_EyePrefab, m_Face.leftEye);
                m_LeftEyeGameObject.SetActive(false);
            }
            if (m_Face.rightEye != null && m_RightEyeGameObject == null)
            {
                m_RightEyeGameObject = Instantiate(m_EyePrefab, m_Face.rightEye);
                m_RightEyeGameObject.SetActive(false);
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