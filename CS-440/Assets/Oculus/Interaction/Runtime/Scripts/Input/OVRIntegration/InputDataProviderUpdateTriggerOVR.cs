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

namespace Oculus.Interaction.Input
{
    // When it is desired for HandDataAsset to drive OVRSkeleton, the DataSources:
    // Must execute after OVRHands (so that the modifier stack can read the latest IsTracked value)
    // Must execute before OVRSkeleton (so that the created IOVRSkeletonDataProvider injected into
    // OVRSkeleton can read from the updated HandDataAsset)
    //   By deriving from InputDataProviderUpdateTrigger, we ensure that the InputData will be
    //   invalidated at the right time (at priority -85), so that it is recalculated just-in-time
    //   in the skeleton data callbacks (at priority -80)
    [DefaultExecutionOrder(-85)]
    public class InputDataProviderUpdateTriggerOVR : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IDataSource))]
        private MonoBehaviour _dataSource;
        private IDataSource DataSource;

        [SerializeField]
        private bool _enableUpdate = true;
        [SerializeField]
        private bool _enableFixedUpdate = true;

        [SerializeField, Interface(typeof(IOVRCameraRigRef)), Optional]
        private MonoBehaviour _cameraRigRef;

        protected bool _started = false;

        public IOVRCameraRigRef CameraRigRef { get; private set; } = null;

        protected virtual void Awake()
        {
            DataSource = _dataSource as IDataSource;
            CameraRigRef = _cameraRigRef as IOVRCameraRigRef;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(DataSource);
            if (_cameraRigRef != null)
            {
                Assert.IsNotNull(CameraRigRef);
            }
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                if (CameraRigRef != null)
                {
                    CameraRigRef.OnAnchorsUpdated += MarkRequiresUpdate;
                }
            }
        }

        private void MarkRequiresUpdate()
        {
            DataSource.MarkInputDataRequiresUpdate();
        }

        protected virtual void Update()
        {
            if (_enableUpdate)
            {
                MarkRequiresUpdate();
            }
        }

        protected virtual void FixedUpdate()
        {
            if (_enableFixedUpdate)
            {
                MarkRequiresUpdate();
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                if (CameraRigRef != null)
                {
                    CameraRigRef.OnAnchorsUpdated -= MarkRequiresUpdate;
                }
            }
        }

        #region Inject

        public void InjectAllInputDataProviderUpdateTriggerOVR(IDataSource dataSource, bool enableUpdate, bool enableFixedUpdate)
        {
            InjectDataSource(dataSource);
            InjectEnableUpdate(enableUpdate);
            InjectEnableFixedUpdate(enableFixedUpdate);
        }

        public void InjectDataSource(IDataSource dataSource)
        {
            _dataSource = dataSource as MonoBehaviour;
            DataSource = dataSource;
        }

        public void InjectEnableUpdate(bool enableUpdate)
        {
            _enableUpdate = enableUpdate;
        }

        public void InjectEnableFixedUpdate(bool enableFixedUpdate)
        {
            _enableFixedUpdate = enableFixedUpdate;
        }

        public void InjectOptionalCameraRigRef(IOVRCameraRigRef cameraRigRef)
        {
            _cameraRigRef = cameraRigRef as MonoBehaviour;
            CameraRigRef = cameraRigRef;
        }

        #endregion
    }
}
