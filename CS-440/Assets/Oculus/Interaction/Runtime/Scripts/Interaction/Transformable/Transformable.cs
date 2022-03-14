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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    public enum TransformableEvent
    {
        Add,
        Update,
        Remove,
        Transfer
    }

    public struct TransformableArgs
    {
        public int GrabIdentifier { get; }
        public TransformableEvent TransformableEvent { get; }
        public TransformableArgs(int grabIdentifier, TransformableEvent transformableEvent)
        {
            this.GrabIdentifier = grabIdentifier;
            this.TransformableEvent = transformableEvent;
        }
    }

    /// <summary>
    /// Handles a list of IPointables and converts their events into transform
    /// changes on the GameObject this Transformable is attached to.
    /// </summary>
    public class Transformable : MonoBehaviour, ITransformable
    {
        [SerializeField]
        private bool _transferHandOnSecondGrab;

        [SerializeField]
        private int _maxGrabPoints = -1;

        [SerializeField, Interface(typeof(ITransformer)), Optional]
        private MonoBehaviour _oneGrabTransformer = null;

        [SerializeField, Interface(typeof(ITransformer)), Optional]
        private MonoBehaviour _twoGrabTransformer = null;

        [SerializeField, Interface(typeof(IPointable)), Optional]
        private List<MonoBehaviour> _pointables;

        public bool TransferHandOnSecondGrab
        {
            get
            {
                return _transferHandOnSecondGrab;
            }
            set
            {
                _transferHandOnSecondGrab = value;
            }
        }

        public event Action<TransformableArgs> WhenTransformableUpdated = delegate { };

        public List<GrabPoint> GrabPoints => _grabPoints;
        public int GrabPointsCount => _grabPoints.Count;
        public Transform Transform => transform;

        private List<IPointable> Pointables;

        protected Dictionary<int, GrabPoint> _grabPointMap;
        protected List<GrabPoint> _grabPoints;

        private ITransformer _activeTransformer = null;
        private ITransformer OneGrabTransformer;
        private ITransformer TwoGrabTransformer;

        protected bool _started = false;

        protected virtual void Awake()
        {
            OneGrabTransformer = _oneGrabTransformer as ITransformer;
            TwoGrabTransformer = _twoGrabTransformer as ITransformer;
            Pointables = _pointables.ConvertAll(mono => mono as IPointable);
        }
        protected virtual void Start()
        {
            this.BeginStart(ref _started);

            foreach (IPointable pointable in Pointables)
            {
                Assert.IsNotNull(pointable);
            }

            _grabPointMap = new Dictionary<int, GrabPoint>();
            _grabPoints = new List<GrabPoint>();
            if (OneGrabTransformer != null)
            {
                Assert.IsNotNull(OneGrabTransformer);
                OneGrabTransformer.Initialize(this);
            }

            if (TwoGrabTransformer != null)
            {
                Assert.IsNotNull(TwoGrabTransformer);
                TwoGrabTransformer.Initialize(this);
            }

            // Create a default if no transformers assigned
            if (OneGrabTransformer == null &&
                TwoGrabTransformer == null)
            {
                OneHandFreeTransformer defaultTransformer = gameObject.AddComponent<OneHandFreeTransformer>();
                _oneGrabTransformer = defaultTransformer;
                OneGrabTransformer = defaultTransformer;
                OneGrabTransformer.Initialize(this);
            }

            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                foreach (IPointable pointable in Pointables)
                {
                    pointable.OnPointerEvent += HandlePointerEvent;
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                foreach (IPointable pointable in Pointables)
                {
                    pointable.OnPointerEvent -= HandlePointerEvent;
                }
            }
        }

        public void AddPointable(IPointable pointable)
        {
            _pointables.Add(pointable as MonoBehaviour);
            Pointables.Add(pointable);
            if (_started && enabled)
            {
                pointable.OnPointerEvent += HandlePointerEvent;
            }
        }

        public void RemovePointable(IPointable pointable)
        {
            int index = Pointables.IndexOf(pointable);
            if (index == -1)
            {
                return;
            }
            _pointables.RemoveAt(index);
            Pointables.RemoveAt(index);
            if (_started && enabled)
            {
                pointable.OnPointerEvent -= HandlePointerEvent;
            }
        }

        private void HandlePointerEvent(PointerArgs args)
        {
            switch (args.PointerEvent)
            {
                case PointerEvent.Select:
                    AddGrabPoint(args);
                    break;
                case PointerEvent.Move:
                    UpdateGrabPoint(args);
                    break;
                case PointerEvent.Unselect:
                    RemoveGrabPoint(args);
                    break;
            }
        }

        private void AddGrabPoint(PointerArgs args)
        {
            // If the transfer hand on second grab flag is on, we ignore any subsequent
            // Move and Unhover events from the first identifier.
            if (_grabPoints.Count == 1 && _transferHandOnSecondGrab)
            {
                WhenTransformableUpdated(new TransformableArgs(_grabPoints[0].Id,
                    TransformableEvent.Transfer));
                _grabPoints.Clear();
                _grabPointMap.Clear();
            }

            // If we have already reached our max grab points, we ignore any subsequent
            // Move and Unhover events with this identifier in favor of earlier identifiers.
            if (_maxGrabPoints != -1 && _grabPoints.Count == _maxGrabPoints)
            {
                return;
            }

            GrabPoint grabPoint = new GrabPoint(args.Identifier, args.Position, args.Rotation);
            _grabPoints.Add(grabPoint);
            _grabPointMap.Add(args.Identifier, grabPoint);

            WhenTransformableUpdated(new TransformableArgs(args.Identifier,
                TransformableEvent.Add));

            BeginTransform();
        }

        private void UpdateGrabPoint(PointerArgs args)
        {
            if (!_grabPointMap.ContainsKey(args.Identifier))
            {
                return;
            }
            GrabPoint grabPoint = _grabPointMap[args.Identifier];
            grabPoint.UpdateGrab(args.Position, args.Rotation);
            UpdateTransform();

            // Once the update has been processed, clear the delta for this
            // GrabPoint so that it is not processed again. This is critical for
            // transformers acting on more than one GrabPoint because Move updates
            // and subsequently UpdateTransform calls can happen in any order.
            grabPoint.UpdateGrab(grabPoint.GrabPosition,
                                 grabPoint.GrabRotation);

            WhenTransformableUpdated(new TransformableArgs(args.Identifier,
                TransformableEvent.Update));
        }

        private void RemoveGrabPoint(PointerArgs args)
        {
            if (!_grabPointMap.ContainsKey(args.Identifier))
            {
                return;
            }

            GrabPoint grabPoint = _grabPointMap[args.Identifier];
            _grabPointMap.Remove(args.Identifier);
            _grabPoints.Remove(grabPoint);

            WhenTransformableUpdated(new TransformableArgs(args.Identifier,
                TransformableEvent.Remove));

            BeginTransform();
        }

        // Whenever we change the number of grab points, we save the
        // current transform data
        private void BeginTransform()
        {
            // End the transform on any existing transformer before we
            // begin the new one
            if (_activeTransformer != null)
            {
                _activeTransformer.EndTransform();
            }

            switch (_grabPoints.Count)
            {
                case 1:
                    _activeTransformer = OneGrabTransformer;
                    break;
                case 2:
                    _activeTransformer = TwoGrabTransformer;
                    break;
                default:
                    _activeTransformer = null;
                    break;
            }

            if (_activeTransformer == null)
            {
                return;
            }

            _activeTransformer.BeginTransform();
        }

        private void UpdateTransform()
        {
            if (_activeTransformer == null)
            {
                return;
            }

            _activeTransformer.UpdateTransform();

            // Any collider on the Transformable may need updating for
            // future collision checks this frame
            Physics.SyncTransforms();
        }

        #region Inject

        public void InjectOptionalOneGrabTransformer(ITransformer transformer)
        {
            _oneGrabTransformer = transformer as MonoBehaviour;
            OneGrabTransformer = transformer;
        }

        public void InjectOptionalTwoGrabTransformer(ITransformer transformer)
        {
            _twoGrabTransformer = transformer as MonoBehaviour;
            TwoGrabTransformer = transformer;
        }

        public void InjectOptionalPointables(List<IPointable> pointables)
        {
            _pointables = pointables.ConvertAll(pointable => pointable as MonoBehaviour);
            Pointables = pointables;
        }

        public void InjectOptionalMaxGrabPoints(int maxGrabPoints)
        {
            _maxGrabPoints = maxGrabPoints;
        }

        #endregion
    }
}
