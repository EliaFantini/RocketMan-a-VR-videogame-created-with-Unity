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
using UnityEngine.XR;

namespace Oculus.Interaction.Input
{
    public class FromOVRHmdDataSource : DataSource<HmdDataAsset, HmdDataSourceConfig>
    {
        [Header("OVR Data Source")]
        [SerializeField, Interface(typeof(IOVRCameraRigRef))]
        private MonoBehaviour _cameraRigRef;

        public IOVRCameraRigRef CameraRigRef { get; private set; }

        [SerializeField]
        [Tooltip("If true, uses OVRManager.headPoseRelativeOffset rather than sensor data for " +
                 "HMD pose.")]
        private bool _useOvrManagerEmulatedPose = false;

        [Header("Shared Configuration")]
        [SerializeField, Interface(typeof(ITrackingToWorldTransformer))]
        private MonoBehaviour _trackingToWorldTransformer;
        private ITrackingToWorldTransformer TrackingToWorldTransformer;

        private HmdDataAsset _hmdDataAsset = new HmdDataAsset();
        private HmdDataSourceConfig _config;

        protected void Awake()
        {
            CameraRigRef = _cameraRigRef as IOVRCameraRigRef;
            TrackingToWorldTransformer = _trackingToWorldTransformer as ITrackingToWorldTransformer;
        }

        protected override void Start()
        {
            base.Start();
            Assert.IsNotNull(CameraRigRef);
            Assert.IsNotNull(TrackingToWorldTransformer);
            InitConfig();
        }
        private void InitConfig()
        {
            if (_config != null)
            {
                return;
            }
            _config = new HmdDataSourceConfig()
            {
                TrackingToWorldTransformer = TrackingToWorldTransformer
            };
        }

        protected override void UpdateData()
        {
            bool hmdPresent = OVRNodeStateProperties.IsHmdPresent();
            ref var centerEyePose = ref _hmdDataAsset.Root;
            if (_useOvrManagerEmulatedPose)
            {
                Quaternion emulatedRotation = Quaternion.Euler(
                    -OVRManager.instance.headPoseRelativeOffsetRotation.x,
                    -OVRManager.instance.headPoseRelativeOffsetRotation.y,
                    OVRManager.instance.headPoseRelativeOffsetRotation.z);
                centerEyePose.rotation = emulatedRotation;
                centerEyePose.position = OVRManager.instance.headPoseRelativeOffsetTranslation;
                hmdPresent = true;
            }
            else
            {
                var previousEyePose = new Pose();

                if (_hmdDataAsset.IsTracked)
                {
                    previousEyePose = _hmdDataAsset.Root;
                }

                if (hmdPresent)
                {
                    // These are already in Unity's coordinate system (LHS)
                    if (!OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.CenterEye,
                        NodeStatePropertyType.Position, OVRPlugin.Node.EyeCenter,
                        OVRPlugin.Step.Render, out centerEyePose.position))
                    {
                        centerEyePose.position = previousEyePose.position;
                    }

                    if (!OVRNodeStateProperties.GetNodeStatePropertyQuaternion(XRNode.CenterEye,
                        NodeStatePropertyType.Orientation, OVRPlugin.Node.EyeCenter,
                        OVRPlugin.Step.Render, out centerEyePose.rotation))
                    {
                        centerEyePose.rotation = previousEyePose.rotation;
                    }
                }
                else
                {
                    centerEyePose = previousEyePose;
                }
            }

            _hmdDataAsset.IsTracked = hmdPresent;
            _hmdDataAsset.FrameId = Time.frameCount;
        }

        protected override HmdDataAsset DataAsset => _hmdDataAsset;

        // It is important that this creates an object on the fly, as it is possible it is called
        // from other components Awake methods.
        public override HmdDataSourceConfig Config
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

        public void InjectAllFromOVRHmdDataSource(UpdateModeFlags updateMode, IDataSource updateAfter,
            bool useOvrManagerEmulatedPose, ITrackingToWorldTransformer trackingToWorldTransformer)
        {
            base.InjectAllDataSource(updateMode, updateAfter);
            InjectUseOvrManagerEmulatedPose(useOvrManagerEmulatedPose);
            InjectTrackingToWorldTransformer(trackingToWorldTransformer);
        }

        public void InjectUseOvrManagerEmulatedPose(bool useOvrManagerEmulatedPose)
        {
            _useOvrManagerEmulatedPose = useOvrManagerEmulatedPose;
        }

        public void InjectTrackingToWorldTransformer(ITrackingToWorldTransformer trackingToWorldTransformer)
        {
            _trackingToWorldTransformer = trackingToWorldTransformer as MonoBehaviour;
            TrackingToWorldTransformer = trackingToWorldTransformer;
        }

        #endregion
    }
}
