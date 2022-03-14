/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using static OVRMesh;
using static OVRMeshRenderer;
using static OVRSkeleton;
using static OVRSkeletonRenderer;

namespace Oculus.Interaction.Input
{
    public delegate HandDataAsset HandInputDataReadFunction();

    internal class OVRInputHandComponents
    {
        public OVRInputHandComponents(Transform handAnchor)
        {
            Anchor = handAnchor;
            OvrHand = handAnchor.GetComponentInChildren<OVRHand>(true);
            if (OvrHand != null)
            {
                OvrMesh = OvrHand.GetComponent<OVRMesh>();
                OvrMeshRenderer = OvrHand.GetComponent<OVRMeshRenderer>();
                OvrSkeleton = OvrHand.GetComponent<OVRSkeleton>();
                OvrSkeletonRenderer = OvrHand.GetComponent<OVRSkeletonRenderer>();
            }
        }

        public Transform Anchor { get; }
        public OVRMesh OvrMesh { get; }
        public OVRMeshRenderer OvrMeshRenderer { get; }
        public OVRSkeleton OvrSkeleton { get; }
        public OVRSkeletonRenderer OvrSkeletonRenderer { get; }
        public OVRHand OvrHand { get; }
    }

    public enum HandRenderPoseOriginBehavior
    {
        /// <summary>
        /// Show hand if it's PoseOrigin is RawTrackedPose or FilteredTrackedPose, regardless of the
        /// tracking confidence
        /// </summary>
        ShowTrackedOnly,

        /// <summary>
        /// Show hand if it's PoseOrigin is RawTrackedPose or FilteredTrackedPose, and also has high
        /// tracking confidence
        /// </summary>
        ShowHighConfidenceTrackedOnly,

        /// <summary>
        /// Show hand if it's PoseOrigin is any value other than None, and it is connected.
        /// </summary>
        ShowConnectedOnly,
    }

    public class BaseOVRDataProvider
    {
        private readonly HandRenderPoseOriginBehavior _poseOriginBehavior;

        protected BaseOVRDataProvider(HandRenderPoseOriginBehavior poseOriginBehavior)
        {
            this._poseOriginBehavior = poseOriginBehavior;
        }

        public bool ToOvrConfidence(HandDataAsset handDataAsset)
        {

            bool isRootPoseTracked =
                handDataAsset.RootPoseOrigin == PoseOrigin.RawTrackedPose ||
                handDataAsset.RootPoseOrigin == PoseOrigin.FilteredTrackedPose;
            switch (_poseOriginBehavior)
            {
                case HandRenderPoseOriginBehavior.ShowConnectedOnly:
                    {
                        bool isSyntheticPose =
                            handDataAsset.RootPoseOrigin == PoseOrigin.SyntheticPose;
                        return isRootPoseTracked || isSyntheticPose;
                    }
                case HandRenderPoseOriginBehavior.ShowTrackedOnly:
                    return isRootPoseTracked;
                case HandRenderPoseOriginBehavior.ShowHighConfidenceTrackedOnly:
                    return isRootPoseTracked && handDataAsset.IsHighConfidence;
                default:
                    // Should not reach this.
                    return false;
            }
        }

    }

    public class FromDataAssetOVRMeshRendererDataProvider : BaseOVRDataProvider, IOVRMeshRendererDataProvider
    {
        public IOVRMeshRendererDataProvider originalProvider;
        public HandInputDataReadFunction handInputDataProvider;

        public FromDataAssetOVRMeshRendererDataProvider(
            HandRenderPoseOriginBehavior poseOriginBehavior) : base(poseOriginBehavior)
        {
        }

        public MeshRendererData GetMeshRendererData()
        {
            var data = originalProvider.GetMeshRendererData();

            var handInputData = handInputDataProvider.Invoke();
            if (handInputData == null)
            {
                return data;
            }

            if (!data.IsDataValid)
            {
                data.ShouldUseSystemGestureMaterial = false;
            }

            data.IsDataValid = handInputData.IsDataValid && handInputData.IsConnected;
            if (!data.IsDataValid)
            {
                return data;
            }

            data.IsDataHighConfidence = ToOvrConfidence(handInputData);

            return data;
        }
    }

    public class FromDataAssetOVRSkeletonDataProvider : BaseOVRDataProvider, IOVRSkeletonDataProvider
    {
        public IOVRSkeletonDataProvider originalProvider;
        public HandInputDataReadFunction handInputDataProvider;
        public SkeletonType skeletonType;

        private readonly OVRPlugin.Quatf[] _boneRotations =
            new OVRPlugin.Quatf[OVRSkeletonData.LeftSkeleton.NumBones];

        private SkeletonPoseData _poseData;

        public FromDataAssetOVRSkeletonDataProvider(
            HandRenderPoseOriginBehavior poseOriginBehavior) : base(poseOriginBehavior)
        {
        }

        public static int InitialSkeletonChangeCount => 1;

        public SkeletonPoseData GetSkeletonPoseData()
        {
            return _poseData;
        }

        public void UpdateSkeletonPoseData()
        {
            _poseData = originalProvider.GetSkeletonPoseData();

            // If the hand is not connected, treat it as if there is no valid data.
            var handInputData = handInputDataProvider.Invoke();

            bool isValid = handInputData.IsDataValid && handInputData.IsConnected;
            _poseData.IsDataValid = isValid;
            if (!isValid)
            {
                return;
            }

            for (int i = 0; i < _boneRotations.Length; i++)
            {
                _boneRotations[i] = handInputData.Joints[i].ToFlippedXQuatf();
            }

            if (handInputData.HandScale <= 0.0f)
            {
                // If handScale is zero it will cause rendering artifacts on the hand meshes.
                handInputData.HandScale = 1.0f;
            }

            _poseData.IsDataHighConfidence = ToOvrConfidence(handInputData);
            _poseData.BoneRotations = _boneRotations;
            _poseData.RootPose = new OVRPlugin.Posef()
            {
                Orientation = handInputData.Root.rotation.ToFlippedZQuatf(),
                Position = handInputData.Root.position.ToFlippedZVector3f()
            };
            _poseData.RootScale = handInputData.HandScale;
        }

        public SkeletonType GetSkeletonType()
        {
            return skeletonType;
        }
    }

    public class FromDataAssetOVRSkeletonRendererDataProvider : BaseOVRDataProvider, IOVRSkeletonRendererDataProvider
    {
        public HandInputDataReadFunction handInputDataProvider;
        public IOVRSkeletonRendererDataProvider originalProvider;

        public FromDataAssetOVRSkeletonRendererDataProvider(
            HandRenderPoseOriginBehavior poseOriginBehavior) : base(poseOriginBehavior)
        {
        }

        public SkeletonRendererData GetSkeletonRendererData()
        {
            var data = new SkeletonRendererData();
            var handInputData = handInputDataProvider.Invoke();
            if (handInputData == null)
            {
                return originalProvider.GetSkeletonRendererData();
            }

            data.IsDataValid = handInputData.RootPoseOrigin != PoseOrigin.None;
            data.IsDataHighConfidence = ToOvrConfidence(handInputData);
            data.RootScale = handInputData.HandScale;
            data.ShouldUseSystemGestureMaterial = false;
            return data;
        }
    }

    /// <summary>
    /// Reads hand & HMD pose data from the DataSources, and copies them onto the
    /// given OVRSkeleton. Can also provide more sophisticated control of the mesh rendering,
    /// to keep hand meshes visible when tracking is lost.
    ///
    /// This class is not required if you are using the HandSkeletonVisual to render the hand.
    /// </summary>
    public class OVRSkeletonDataProviders : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IOVRCameraRigRef))]
        private MonoBehaviour _cameraRigRef;

        public IOVRCameraRigRef CameraRigRef { get; private set; }

        [Header("Update CameraRig Transforms")]
        [SerializeField, Interface(typeof(IDataSource<HmdDataAsset, HmdDataSourceConfig>))]
        private MonoBehaviour _hmdData;
        private IDataSource<HmdDataAsset, HmdDataSourceConfig> _hmdDataSource;

        [SerializeField]
        private Hand _leftHand;

        [SerializeField]
        private Hand _rightHand;

        [SerializeField]
        private bool _modifyCameraRigAnchorTransforms = true;

        [SerializeField]
        private bool _modifyHandTransformsLeft = true;

        [SerializeField]
        private bool _modifyHandTransformsRight = true;

        [Header("Hand Meshes")]
        [Tooltip("If true, use the following hand meshes in place of those provided by OVRPlugin")]
        [SerializeField]
        private bool _replaceHandMeshRendererProviders = true;

        [SerializeField, Optional]
        private Mesh _leftHandMesh;

        [SerializeField, Optional]
        private Mesh _rightHandMesh;

        [SerializeField]
        private HandRenderPoseOriginBehavior _handRenderBehavior =
            HandRenderPoseOriginBehavior.ShowConnectedOnly;

        OVRInputHandComponents _leftHandComponents;
        OVRInputHandComponents _rightHandComponents;
        private FromDataAssetOVRSkeletonDataProvider _leftHandOvrSkeletonDataProvider;
        private FromDataAssetOVRSkeletonDataProvider _rightHandOvrSkeletonDataProvider;

        private const System.Reflection.BindingFlags PrivateInstanceFlags =
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

        private const System.Reflection.BindingFlags InstanceFlags =
            PrivateInstanceFlags | System.Reflection.BindingFlags.Public;

        public void SetModifyCameraRigAndHandTransformState(bool modifyCameraRig,
            bool modifyLeftHand, bool modifyRightHand)
        {
            _modifyCameraRigAnchorTransforms = modifyCameraRig;
            _modifyHandTransformsLeft = modifyLeftHand;
            _modifyHandTransformsRight = modifyRightHand;
        }

        protected bool _started = false;

        protected virtual void Awake()
        {
            CameraRigRef = _cameraRigRef as IOVRCameraRigRef;
            _hmdDataSource = _hmdData as IDataSource<HmdDataAsset, HmdDataSourceConfig>;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(CameraRigRef);
            Assert.IsNotNull(_hmdDataSource);
            Assert.IsNotNull(_leftHand);
            Assert.IsNotNull(_rightHand);

            OVRCameraRig cameraRig = CameraRigRef.CameraRig;
            Assert.IsNotNull(cameraRig);
            Assert.IsNotNull(cameraRig.leftHandAnchor);
            Assert.IsNotNull(cameraRig.rightHandAnchor);
            Assert.IsNotNull(cameraRig.centerEyeAnchor);

            _leftHandComponents = new OVRInputHandComponents(cameraRig.leftHandAnchor);
            _rightHandComponents = new OVRInputHandComponents(cameraRig.rightHandAnchor);

            HandDataAsset LeftHandProvider() =>
                _leftHand.isActiveAndEnabled
                    ? _leftHand.GetData()
                    : null;

            HandDataAsset RightHandProvider() =>
                _rightHand.isActiveAndEnabled
                    ? _rightHand.GetData()
                    : null;

            if (_replaceHandMeshRendererProviders && _leftHandComponents.OvrHand != null &&
                _rightHandComponents.OvrHand != null)
            {
                InitializeBakedMeshes();
            }

            // Inject a custom DataProvider to provide confidence levels to the
            // OVRMeshRenderer
            InitializeMeshRendererDataProvider(_leftHandComponents, LeftHandProvider);
            InitializeMeshRendererDataProvider(_rightHandComponents, RightHandProvider);

            // Inject a custom DataProvider to provide finger joints to the OVRSkeleton,
            // which are surfaced though the `Bones` property.
            // We update this providers state in this components Update method, so that we can
            // catch skeleton changes and notify the DataSources.
            _leftHandOvrSkeletonDataProvider = CreateHandSkeletonPoseDataProvider(
                _leftHandComponents, LeftHandProvider);
            _rightHandOvrSkeletonDataProvider = CreateHandSkeletonPoseDataProvider(
                _rightHandComponents, RightHandProvider);
            InitializeHandSkeletonPoseDataProvider(
                _leftHandComponents, _leftHandOvrSkeletonDataProvider);
            InitializeHandSkeletonPoseDataProvider(
                _rightHandComponents, _rightHandOvrSkeletonDataProvider);

            // Inject a custom DataProvider to provide poses to the OVRSkeletonRenderer
            // (debug skeleton joint visualization)
            InitializeHandSkeletonRendererPoseDataProvider(_leftHandComponents, LeftHandProvider);
            InitializeHandSkeletonRendererPoseDataProvider(_rightHandComponents, RightHandProvider);
            this.EndStart(ref _started);
        }

        private void InitializeBakedMeshes()
        {
            /*
            Q: Why all the reflection to update OVRXYZ classes?
            A: Since OVRMesh, OVRSkeleton etc directly call OVRPlugin.XYZ methods, without any hooks,
               hooks, I can't easily get past the fact that on Oculus Link, when hand tracking is
               not active, OVRHands/OVRSkeleton do not work. (OVRPlugin.GetMesh returns nothing)
               The goal of this code below is to provide baked skeleton and mesh data to the OVR classes,
               classes,  rather than them retrieving it from OVRPlugin. This allows OVRHands to
               render in the Editor using poses sourced from GRVF files,  without needing to
               call the OVRPlugin API's.

            Q: Why are the hand fingers all messed up!
            A: You might be using the mesh provided by the Oculus Integration (OculusHand_L or
               OculusHand_R). These meshes are not compatible with OVRSkeleton by themselves; they
               are intended for use with OVRCustomSkeleton. To fix this, use the provided meshes
               (HandLeft, HandRight).
            */
            ReplaceSkeletonData(_leftHandComponents.OvrSkeleton, OVRSkeletonData.LeftSkeleton);
            ReplaceSkeletonData(_rightHandComponents.OvrSkeleton, OVRSkeletonData.RightSkeleton);

            if (_leftHandMesh != null)
            {
                ReplaceHandMeshes(_leftHandComponents, _leftHandMesh);
            }

            if (_rightHandMesh != null)
            {
                ReplaceHandMeshes(_rightHandComponents, _rightHandMesh);
            }
            // Inject a custom DataProvider to provide finger joints to the OVRSkeleton,
            // Inject a custom DataProvider to provide poses to the OVRSkeletonRenderer
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                _leftHand.HandUpdated += OnLeftHandDataAvailable;
                _rightHand.HandUpdated += OnRightHandDataAvailable;

                // The root poses of the Hand and HMD must be updated just after OVRCameraRig, which is at
                //   priority 0. Otherwise the changes will be overwritten in that update. To solve this, root
                //   poses are updated in the OVRCameraRig.UpdatedAnchors callback.
                CameraRigRef.CameraRig.UpdatedAnchors += OnCameraRigUpdatedAnchors;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                CameraRigRef.CameraRig.UpdatedAnchors -= OnCameraRigUpdatedAnchors;
                _leftHand.HandUpdated -= OnLeftHandDataAvailable;
                _rightHand.HandUpdated -= OnRightHandDataAvailable;
            }
        }

        protected virtual void Update()
        {
            // Handle the case where OVRManager is not initialized (usually due to an HMD not being
            // plugged in, when using LINK.
            if (!OVRManager.OVRManagerinitialized && !CameraRigRef.CameraRig.useFixedUpdateForTracking)
            {
                CameraRigRef.CameraRig.EnsureGameObjectIntegrity();
                OverwriteHandTransforms();
            }
        }

        private void FixedUpdate()
        {
            // Handle the case where OVRManager is not initialized (usually due to an HMD not being
            // plugged in, when using LINK.
            if (!OVRManager.OVRManagerinitialized && CameraRigRef.CameraRig.useFixedUpdateForTracking)
            {
                CameraRigRef.CameraRig.EnsureGameObjectIntegrity();
                OverwriteHandTransforms();
            }
        }

        private void OnCameraRigUpdatedAnchors(OVRCameraRig obj)
        {
            OverwriteHandTransforms();
        }

        private void OnLeftHandDataAvailable()
        {
            _leftHandOvrSkeletonDataProvider.UpdateSkeletonPoseData();
        }

        private void OnRightHandDataAvailable()
        {
            _rightHandOvrSkeletonDataProvider.UpdateSkeletonPoseData();
        }

        private void OverwriteHandTransforms()
        {
            // Apply modified state back to the camera rig anchors
            if (_modifyCameraRigAnchorTransforms && _hmdData.isActiveAndEnabled)
            {
                var hmdInputData = _hmdDataSource.GetData();
                if (_modifyCameraRigAnchorTransforms && hmdInputData.IsTracked)
                {
                    SetLocalTransform(CameraRigRef.CameraRig.centerEyeAnchor, hmdInputData.Root);
                }
            }

            if (_modifyHandTransformsLeft && _leftHand.isActiveAndEnabled)
            {
                if (_modifyHandTransformsLeft && _leftHand.GetRootPose(out Pose rootPose))
                {
                    SetTransform(_leftHandComponents.Anchor, rootPose);
                }
            }

            if (_modifyHandTransformsRight && _rightHand.isActiveAndEnabled)
            {
                if (_modifyHandTransformsRight && _rightHand.GetRootPose(out Pose rootPose))
                {
                    SetTransform(_rightHandComponents.Anchor, rootPose);
                }
            }
        }

        private static void SetLocalTransform(Transform transform, in Pose root)
        {
            transform.localPosition = root.position;
            transform.localRotation = root.rotation;
        }

        private static void SetTransform(Transform transform, in Pose root)
        {
            transform.position = root.position;
            transform.rotation = root.rotation;
        }

        private void ReplaceHandMeshes(OVRInputHandComponents handComponents, Mesh mesh)
        {
            if (handComponents.OvrMesh == null)
            {
                return;
            }

            // OVRSkeleton and OVRMesh will not initialize the skeleton in editor mode if the hand
            // is not connected. This is a limitation with Link, the skeleton isn't available unless
            // hands are tracked.
            Assert.IsNotNull(mesh);

            // Create a clone of the mesh, so that vertices in the asset file are not modified by
            // changes to sharedMesh.
            mesh = Instantiate(mesh);

            InvokeSetField(handComponents.OvrMesh, "_mesh", mesh);
            InvokeSetProp(handComponents.OvrMesh, "IsInitialized", true);
        }

        private void InitializeMeshRendererDataProvider(OVRInputHandComponents handComponents,
            HandInputDataReadFunction inputDataProvider)
        {
            if (handComponents.OvrMeshRenderer == null)
            {
                return;
            }

            var dataProvider = new FromDataAssetOVRMeshRendererDataProvider(_handRenderBehavior)
            {
                handInputDataProvider = inputDataProvider,
                originalProvider = handComponents.OvrHand
            };

            InvokeSetField(handComponents.OvrMeshRenderer, "_dataProvider", dataProvider);
        }

        private FromDataAssetOVRSkeletonDataProvider CreateHandSkeletonPoseDataProvider(
            OVRInputHandComponents handComponents, HandInputDataReadFunction inputDataProvider)
        {
            var ovrMeshType = handComponents.OvrHand.GetComponent<IOVRMeshDataProvider>()
                .GetMeshType();
            SkeletonType skeletonType =
                ovrMeshType == MeshType.HandLeft ? SkeletonType.HandLeft : SkeletonType.HandRight;
            return new FromDataAssetOVRSkeletonDataProvider(_handRenderBehavior)
            {
                originalProvider = handComponents.OvrHand,
                skeletonType = skeletonType,
                handInputDataProvider = inputDataProvider
            };
        }

        private void InitializeHandSkeletonPoseDataProvider(OVRInputHandComponents handComponents,
            FromDataAssetOVRSkeletonDataProvider dataProvider)
        {
            InvokeSetField(handComponents.OvrSkeleton, "_dataProvider", dataProvider);
        }

        private void InitializeHandSkeletonRendererPoseDataProvider(
            OVRInputHandComponents handComponents, HandInputDataReadFunction inputDataProvider)
        {
            if (handComponents.OvrSkeletonRenderer == null)
            {
                return;
            }

            var dataProvider = new FromDataAssetOVRSkeletonRendererDataProvider(_handRenderBehavior)
            {
                handInputDataProvider = inputDataProvider,
                originalProvider = handComponents.OvrHand
            };

            InvokeSetField(handComponents.OvrSkeletonRenderer, "_dataProvider", dataProvider);
        }

        private void ReplaceSkeletonData(OVRSkeleton ovrLeftHandSkeleton,
            OVRPlugin.Skeleton2 skeletonData)
        {
            var skeletonChangeCount = FromDataAssetOVRSkeletonDataProvider.InitialSkeletonChangeCount;
            var nullParams = new object[] { };

            InvokeSetField(ovrLeftHandSkeleton, "_skeleton", skeletonData);

            InvokeMethod(ovrLeftHandSkeleton, "InitializeBones", nullParams);
            InvokeMethod(ovrLeftHandSkeleton, "InitializeBindPose", nullParams);
            InvokeMethod(ovrLeftHandSkeleton, "InitializeCapsules", nullParams);
            InvokeSetProp(ovrLeftHandSkeleton, "IsInitialized", true);
            InvokeSetProp(ovrLeftHandSkeleton, "SkeletonChangedCount", -1);
        }

        private static void InvokeMethod<T>(T instance, string methodName, object[] args)
        {
            var method = typeof(T).GetMethod(methodName, InstanceFlags);
            Assert.IsNotNull(method, methodName + " method must exist on type: " + nameof(T));
            method.Invoke(instance, args);
        }

        private static void InvokeSetField<T>(T instance, string fieldName, object val)
        {
            var prop = typeof(T).GetField(fieldName, InstanceFlags);
            Assert.IsNotNull(prop, prop + " field must exist on type: " + nameof(T));
            prop.SetValue(instance, val);
        }

        private static void InvokeSetProp<T>(T instance, string propName, object val)
        {
            var prop = typeof(T).GetProperty(propName, InstanceFlags);
            Assert.IsNotNull(prop, prop + " property must exist on type: " + nameof(T));
            prop.SetMethod.Invoke(instance, new[] { val });
        }

        #region Inject

        public void InjectOVRSkeletonDataProviders(IOVRCameraRigRef cameraRigRef,
            IDataSource<HmdDataAsset, HmdDataSourceConfig> hmdData,
            Hand leftHand, Hand rightHand, bool modifyCameraRigAnchorTransforms, bool modifyHandTransformsLeft,
            bool modifyHandTransformsRight, bool replaceHandMeshRendererProviders,
            HandRenderPoseOriginBehavior handRenderBehavior)
        {
            InjectCameraRigRef(cameraRigRef);
            InjectHmdData(hmdData);
            InjectLeftHand(leftHand);
            InjectRightHand(rightHand);
            InjectModifyCameraRigAnchorTransforms(modifyCameraRigAnchorTransforms);
            InjectModifyHandTransformsLeft(modifyHandTransformsLeft);
            InjectModifyHandTransformsRight(modifyHandTransformsRight);
            InjectReplaceHandMeshRendererProviders(replaceHandMeshRendererProviders);
            InjectHandRenderBehavior(handRenderBehavior);
        }

        public void InjectCameraRigRef(IOVRCameraRigRef cameraRigRef)
        {
            _cameraRigRef = cameraRigRef as MonoBehaviour;
            CameraRigRef = cameraRigRef;
        }

        public void InjectHmdData(IDataSource<HmdDataAsset,HmdDataSourceConfig> hmdData)
        {
            _hmdData = hmdData as MonoBehaviour;
            _hmdDataSource = hmdData;
        }

        public void InjectLeftHand(Hand leftHand)
        {
            _leftHand = leftHand;
        }

        public void InjectRightHand(Hand rightHand)
        {
            _rightHand = rightHand;
        }

        public void InjectModifyCameraRigAnchorTransforms(bool modifyCameraRigAnchorTransforms)
        {
            _modifyCameraRigAnchorTransforms = modifyCameraRigAnchorTransforms;
        }

        public void InjectModifyHandTransformsLeft(bool modifyHandTransformsLeft)
        {
            _modifyHandTransformsLeft = modifyHandTransformsLeft;
        }

        public void InjectModifyHandTransformsRight(bool modifyHandTransformsRight)
        {
            _modifyHandTransformsRight = modifyHandTransformsRight;
        }

        public void InjectReplaceHandMeshRendererProviders(bool replaceHandMeshRendererProviders)
        {
            _replaceHandMeshRendererProviders = replaceHandMeshRendererProviders;
        }

        public void InjectHandRenderBehavior(HandRenderPoseOriginBehavior handRenderBehavior)
        {
            _handRenderBehavior = handRenderBehavior;
        }

        public void InjectOptionalLeftHandMesh(Mesh handMesh)
        {
            _leftHandMesh = handMesh;
        }

        public void InjectOptionalRightHandMesh(Mesh handMesh)
        {
            _rightHandMesh = handMesh;
        }

        #endregion
    }
}
