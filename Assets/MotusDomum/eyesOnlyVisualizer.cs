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

        [SerializeField]
        private string ipAddress = "localhost";
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


        [SerializeField]
        float m_CoefficientScale = 100.0f;

        public float coefficientScale
        {
            get { return m_CoefficientScale; }
            set { m_CoefficientScale = value; }
        }

        [SerializeField]
        SkinnedMeshRenderer m_SkinnedMeshRenderer;

        public SkinnedMeshRenderer skinnedMeshRenderer
        {
            get
            {
                return m_SkinnedMeshRenderer;
            }
            set
            {
                m_SkinnedMeshRenderer = value;
                CreateFeatureBlendMapping();
            }
        }

#if UNITY_IOS && !UNITY_EDITOR
        ARKitFaceSubsystem m_ARKitFaceSubsystem;

        Dictionary<ARKitBlendShapeLocation, int> m_FaceArkitBlendShapeIndexMap;
#endif


        private string deviceName; 

        void Awake()
        {
            string rawstring;
            m_Face = GetComponent<ARFace>();

            CreateFeatureBlendMapping();

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

            Debug.Log($"{GetType()}:{name}: connectToOscServer(): Connecting with: ip:{ipAddress}:{udpPort}");

            if (_client != null)
            {
                _client.Dispose();
            }

            _client = OscMaster.GetSharedClient(ipAddress, udpPort);

            if (_client == null) Debug.LogError($"{GetType()}:{name}: connectToOscServer(): NULL CLIENT RETURNED");

        }



   
        void CreateFeatureBlendMapping()
        {
            if (skinnedMeshRenderer == null || skinnedMeshRenderer.sharedMesh == null)
            {
                return;
            }

#if UNITY_IOS && !UNITY_EDITOR
            const string strPrefix = "blendShape2.";
            m_FaceArkitBlendShapeIndexMap = new Dictionary<ARKitBlendShapeLocation, int>();

            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.BrowDownLeft        ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "browDown_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.BrowDownRight       ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "browDown_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.BrowInnerUp         ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "browInnerUp");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.BrowOuterUpLeft     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "browOuterUp_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.BrowOuterUpRight    ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "browOuterUp_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.CheekPuff           ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "cheekPuff");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.CheekSquintLeft     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "cheekSquint_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.CheekSquintRight    ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "cheekSquint_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.EyeBlinkLeft        ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeBlink_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.EyeBlinkRight       ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeBlink_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.EyeLookDownLeft     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookDown_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.EyeLookDownRight    ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookDown_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.EyeLookInLeft       ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookIn_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.EyeLookInRight      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookIn_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.EyeLookOutLeft      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookOut_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.EyeLookOutRight     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookOut_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.EyeLookUpLeft       ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookUp_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.EyeLookUpRight      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookUp_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.EyeSquintLeft       ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeSquint_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.EyeSquintRight      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeSquint_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.EyeWideLeft         ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeWide_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.EyeWideRight        ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeWide_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.JawForward          ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "jawForward");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.JawLeft             ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "jawLeft");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.JawOpen             ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "jawOpen");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.JawRight            ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "jawRight");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthClose          ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthClose");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthDimpleLeft     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthDimple_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthDimpleRight    ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthDimple_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthFrownLeft      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthFrown_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthFrownRight     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthFrown_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthFunnel         ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthFunnel");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthLeft           ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthLeft");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthLowerDownLeft  ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthLowerDown_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthLowerDownRight ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthLowerDown_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthPressLeft      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthPress_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthPressRight     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthPress_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthPucker         ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthPucker");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthRight          ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthRight");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthRollLower      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthRollLower");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthRollUpper      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthRollUpper");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthShrugLower     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthShrugLower");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthShrugUpper     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthShrugUpper");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthSmileLeft      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthSmile_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthSmileRight     ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthSmile_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthStretchLeft    ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthStretch_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthStretchRight   ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthStretch_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthUpperUpLeft    ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthUpperUp_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.MouthUpperUpRight   ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthUpperUp_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.NoseSneerLeft       ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "noseSneer_L");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.NoseSneerRight      ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "noseSneer_R");
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.TongueOut           ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "tongueOut");
#endif
        }



        float lastTXtime = 0f;
        private void Update()
        {
            if (_client == null) return;

            if (Time.realtimeSinceStartup - lastTXtime < oscTXdelta) return;

            lastTXtime = Time.realtimeSinceStartup;

            string oscAddr = "/tracking/"+ deviceName+"/face";

            if (m_Face)
            {
                //Vector3 cameraPos = Camera.current.transform.position;
                //Vector3 pos = cameraPos - m_Face.transform.position;

                //Quaternion rotDiff = Quaternion.Inverse(Camera.current.transform.rotation) * m_Face.transform.rotation;

                //Vector3 eulers = rotDiff.eulerAngles;

                Vector3 pos = m_Face.fixationPoint.position;

                _client.Send(oscAddr + "/eyes/lookAtPos", pos.x, pos.y, pos.z);
                //_client.Send(oscAddr + "/eulers", eulers.x, eulers.y, eulers.z);
            }

            if (m_LeftEyeGameObject)
            {
                Vector3 pos = m_LeftEyeGameObject.transform.position;
                Vector3 eulers = m_LeftEyeGameObject.transform.eulerAngles;
                _client.Send(oscAddr + "/leftEye/position", pos.x, pos.y, pos.z);
                _client.Send(oscAddr + "/leftEye/eulers", eulers.x, eulers.y, eulers.z);
            }

            if (m_RightEyeGameObject)
            {
                Vector3 pos = m_RightEyeGameObject.transform.position;
                Vector3 eulers = m_RightEyeGameObject.transform.eulerAngles;
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
           // if (eventArgs.Equals(ARFaceManager))
        }
    }
}