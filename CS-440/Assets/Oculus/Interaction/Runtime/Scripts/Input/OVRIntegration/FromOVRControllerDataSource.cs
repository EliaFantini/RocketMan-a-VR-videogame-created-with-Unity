/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction.Input
{
    struct UsageMapping
    {
        public UsageMapping(ControllerButtonUsage usage, OVRInput.Touch touch)
        {
            Usage = usage;
            Touch = touch;
            Button = OVRInput.Button.None;
        }

        public UsageMapping(ControllerButtonUsage usage, OVRInput.Button button)
        {
            Usage = usage;
            Touch = OVRInput.Touch.None;
            Button = button;
        }

        public bool IsTouch => Touch != OVRInput.Touch.None;
        public bool IsButton => Button != OVRInput.Button.None;
        public ControllerButtonUsage Usage { get; }
        public OVRInput.Touch Touch { get; }
        public OVRInput.Button Button { get; }
    }

    /// <summary>
    /// Returns the Pointer Pose for the active controller model
    /// as found in the official prefabs.
    /// This point is usually located at the front tip of the controller.
    /// </summary>
    struct OVRPointerPoseSelector
    {
        private static readonly Pose[] QUEST1_POINTERS = new Pose[2]
        {
            new Pose(new Vector3(-0.00779999979f,-0.00410000002f,0.0375000015f),
                Quaternion.Euler(359.209534f, 6.45196056f, 6.95544577f)),
            new Pose(new Vector3(0.00779999979f,-0.00410000002f,0.0375000015f),
                Quaternion.Euler(359.209534f, 353.548035f, 353.044556f))
        };

        private static readonly Pose[] QUEST2_POINTERS = new Pose[2]
        {
            new Pose(new Vector3(0.00899999961f, -0.00321028521f, 0.030869998f),
                Quaternion.Euler(359.209534f, 6.45196056f, 6.95544577f)),
            new Pose(new Vector3(-0.00899999961f, -0.00321028521f, 0.030869998f),
                Quaternion.Euler(359.209534f, 353.548035f, 353.044556f))
        };

        public Pose LocalPointerPose { get; private set; }

        public OVRPointerPoseSelector(Handedness handedness)
        {
            OVRPlugin.SystemHeadset headset = OVRPlugin.GetSystemHeadsetType();
            switch (headset)
            {
                case OVRPlugin.SystemHeadset.Oculus_Quest_2:
                case OVRPlugin.SystemHeadset.Oculus_Link_Quest_2:
                    LocalPointerPose =  QUEST2_POINTERS[(int)handedness];
                    break;
                default:
                    LocalPointerPose = QUEST1_POINTERS[(int)handedness];
                    break;
            }
        }
    }

    public class FromOVRControllerDataSource : DataSource<ControllerDataAsset, ControllerDataSourceConfig>
    {
        [Header("OVR Data Source")]
        [SerializeField, Interface(typeof(IOVRCameraRigRef))]
        private MonoBehaviour _cameraRigRef;

        [Header("Shared Configuration")]
        [SerializeField]
        private Handedness _handedness;

        [SerializeField, Interface(typeof(ITrackingToWorldTransformer))]
        private MonoBehaviour _trackingToWorldTransformer;
        private ITrackingToWorldTransformer TrackingToWorldTransformer;

        [SerializeField, Interface(typeof(IDataSource<HmdDataAsset, HmdDataSourceConfig>))]
        private MonoBehaviour _hmdData;
        private IDataSource<HmdDataAsset, HmdDataSourceConfig> HmdData;

        private readonly ControllerDataAsset _controllerDataAsset = new ControllerDataAsset();
        private OVRInput.Controller _ovrController;
        private Transform _ovrControllerAnchor;
        private ControllerDataSourceConfig _config;

        private OVRPointerPoseSelector _pointerPoseSelector;

        public IOVRCameraRigRef CameraRigRef { get; private set; }

        #region OVR Controller Mappings

        // Mappings from Unity XR CommonUsage to Oculus Button/Touch.
        private static readonly UsageMapping[] ControllerUsageMappings =
        {
            new UsageMapping(ControllerButtonUsage.PrimaryButton, OVRInput.Button.One),
            new UsageMapping(ControllerButtonUsage.PrimaryTouch, OVRInput.Touch.One),
            new UsageMapping(ControllerButtonUsage.SecondaryButton, OVRInput.Button.Two),
            new UsageMapping(ControllerButtonUsage.SecondaryTouch, OVRInput.Touch.Two),
            new UsageMapping(ControllerButtonUsage.GripButton,
                OVRInput.Button.PrimaryHandTrigger),
            new UsageMapping(ControllerButtonUsage.TriggerButton,
                OVRInput.Button.PrimaryIndexTrigger),
            new UsageMapping(ControllerButtonUsage.MenuButton, OVRInput.Button.Start),
            new UsageMapping(ControllerButtonUsage.Primary2DAxisClick,
                OVRInput.Button.PrimaryThumbstick),
            new UsageMapping(ControllerButtonUsage.Primary2DAxisTouch,
                OVRInput.Touch.PrimaryThumbstick),
            new UsageMapping(ControllerButtonUsage.Thumbrest, OVRInput.Touch.PrimaryThumbRest)
        };

        #endregion

        protected void Awake()
        {
            TrackingToWorldTransformer = _trackingToWorldTransformer as ITrackingToWorldTransformer;
            HmdData = _hmdData as IDataSource<HmdDataAsset, HmdDataSourceConfig>;
            CameraRigRef = _cameraRigRef as IOVRCameraRigRef;
        }

        protected override void Start()
        {
            base.Start();
            Assert.IsNotNull(CameraRigRef);
            Assert.IsNotNull(TrackingToWorldTransformer);
            Assert.IsNotNull(HmdData);
            if (_handedness == Handedness.Left)
            {
                Assert.IsNotNull(CameraRigRef.LeftController);
                _ovrControllerAnchor = CameraRigRef.LeftController;
                _ovrController = OVRInput.Controller.LTouch;
            }
            else
            {
                Assert.IsNotNull(CameraRigRef.RightController);
                _ovrControllerAnchor = CameraRigRef.RightController;
                _ovrController = OVRInput.Controller.RTouch;
            }
            _pointerPoseSelector = new OVRPointerPoseSelector(_handedness);
        }

        private void InitConfig()
        {
            if (_config != null)
            {
                return;
            }

            _config = new ControllerDataSourceConfig()
            {
                Handedness = _handedness,
                TrackingToWorldTransformer = TrackingToWorldTransformer,
                HmdData = HmdData
            };
        }

        protected override void UpdateData()
        {
            var worldToTrackingSpace = CameraRigRef.CameraRig.transform.worldToLocalMatrix;
            Transform ovrController = _ovrControllerAnchor;

            _controllerDataAsset.IsDataValid = true;
            _controllerDataAsset.IsConnected =
                (OVRInput.GetConnectedControllers() & _ovrController) > 0;
            if (!_controllerDataAsset.IsConnected)
            {
                // revert state fields to their defaults
                _controllerDataAsset.IsTracked = default;
                _controllerDataAsset.ButtonUsageMask = default;
                _controllerDataAsset.RootPoseOrigin = default;
                return;
            }

            _controllerDataAsset.IsTracked = true;

            // Update button usages
            _controllerDataAsset.ButtonUsageMask = ControllerButtonUsage.None;
            OVRInput.Controller controllerMask = _ovrController;
            foreach (UsageMapping mapping in ControllerUsageMappings)
            {
                bool usageActive;
                if (mapping.IsTouch)
                {
                    usageActive = OVRInput.Get(mapping.Touch, controllerMask);
                }
                else
                {
                    Assert.IsTrue(mapping.IsButton);
                    usageActive = OVRInput.Get(mapping.Button, controllerMask);
                }

                if (usageActive)
                {
                    _controllerDataAsset.ButtonUsageMask |= mapping.Usage;
                }
            }

            // Update poses

            // Convert controller pose from world to tracking space.
            Pose worldRoot = new Pose(ovrController.position, ovrController.rotation);
            _controllerDataAsset.RootPose.position = worldToTrackingSpace.MultiplyPoint3x4(worldRoot.position);
            _controllerDataAsset.RootPose.rotation = worldToTrackingSpace.rotation * worldRoot.rotation;
            _controllerDataAsset.RootPoseOrigin = PoseOrigin.RawTrackedPose;


            // Convert controller pointer pose from local to tracking space.
            Pose pointerPose = PoseUtils.Multiply(worldRoot, _pointerPoseSelector.LocalPointerPose);
            _controllerDataAsset.PointerPose.position = worldToTrackingSpace.MultiplyPoint3x4(pointerPose.position);
            _controllerDataAsset.PointerPose.rotation = worldToTrackingSpace.rotation * pointerPose.rotation;
            _controllerDataAsset.PointerPoseOrigin = PoseOrigin.RawTrackedPose;

        }

        protected override ControllerDataAsset DataAsset => _controllerDataAsset;

        // It is important that this creates an object on the fly, as it is possible it is called
        // from other components Awake methods.
        public override ControllerDataSourceConfig Config
        {
            get
            {
                if (_config == null)
                {
                    InitConfig();
                }

                return _config;
            }
        }

        #region Inject

        public void InjectAllFromOVRControllerDataSource(UpdateModeFlags updateMode, IDataSource updateAfter,
            Handedness handedness, ITrackingToWorldTransformer trackingToWorldTransformer,
            IDataSource<HmdDataAsset, HmdDataSourceConfig> hmdData)
        {
            base.InjectAllDataSource(updateMode, updateAfter);
            InjectHandedness(handedness);
            InjectTrackingToWorldTransformer(trackingToWorldTransformer);
            InjectHmdData(hmdData);
        }

        public void InjectHandedness(Handedness handedness)
        {
            _handedness = handedness;
        }

        public void InjectTrackingToWorldTransformer(ITrackingToWorldTransformer trackingToWorldTransformer)
        {
            _trackingToWorldTransformer = trackingToWorldTransformer as MonoBehaviour;
            TrackingToWorldTransformer = trackingToWorldTransformer;
        }

        public void InjectHmdData(IDataSource<HmdDataAsset,HmdDataSourceConfig> hmdData)
        {
            _hmdData = hmdData as MonoBehaviour;
            HmdData = hmdData;
        }

        #endregion
    }
}
