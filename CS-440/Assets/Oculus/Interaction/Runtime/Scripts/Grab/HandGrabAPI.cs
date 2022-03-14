/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction.GrabAPI
{
    public class HandGrabAPI : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _hand;

        public IHand Hand { get; private set; }

        private IFingerAPI _fingerPinchAPI = new FingerPinchAPI();
        private IFingerAPI _fingerPalmAPI = new FingerGrabAPI();

        private bool _started;

        protected virtual void Awake()
        {
            Hand = _hand as IHand;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(Hand);
            Assert.IsNotNull(_fingerPinchAPI);
            Assert.IsNotNull(_fingerPalmAPI);
            this.EndStart(ref _started);
        }

        protected virtual void Update()
        {
            _fingerPinchAPI.Update(Hand);
            _fingerPalmAPI.Update(Hand);
        }

        public HandFingerFlags HandPinchingFinger()
        {
            return HandGrabbingFingers(_fingerPinchAPI);
        }

        public HandFingerFlags HandPalmGrabbingFingers()
        {
            return HandGrabbingFingers(_fingerPalmAPI);
        }

        private HandFingerFlags HandGrabbingFingers(IFingerAPI fingerAPI)
        {
            HandFingerFlags grabbingFingers = HandFingerFlags.None;

            for (int i = 0; i < Constants.NUM_FINGERS; i++)
            {
                HandFinger finger = (HandFinger)i;

                bool isGrabbing = fingerAPI.GetFingerIsGrabbing(finger);
                if (isGrabbing)
                {
                    grabbingFingers |= (HandFingerFlags)(1 << i);
                }
            }

            return grabbingFingers;
        }

        public bool IsHandPinchGrabbing(in GrabbingRule fingers)
        {
            HandFingerFlags pinchFingers = HandPinchingFinger();
            return IsSustainingGrab(fingers, pinchFingers);
        }

        public bool IsHandPalmGrabbing(in GrabbingRule fingers)
        {
            HandFingerFlags palmFingers = HandPalmGrabbingFingers();
            return IsSustainingGrab(fingers, palmFingers);
        }

        public bool IsSustainingGrab(in GrabbingRule fingers, HandFingerFlags grabbingFingers)
        {
            for (int i = 0; i < Constants.NUM_FINGERS; i++)
            {
                HandFinger finger = (HandFinger)i;
                HandFingerFlags fingerFlag = (HandFingerFlags)(1 << i);

                bool isFingerGrabbing = (grabbingFingers & fingerFlag) != 0;
                if (fingers[finger] == FingerRequirement.Required)
                {
                    if (fingers.UnselectMode == FingerUnselectMode.AnyReleased
                        && !isFingerGrabbing)
                    {
                        return false;
                    }

                    if(fingers.UnselectMode == FingerUnselectMode.AllReleased
                        && isFingerGrabbing)
                    {
                        return true;
                    }
                }
                else if (fingers[finger] == FingerRequirement.Optional)
                {
                    if (fingers.UnselectMode == FingerUnselectMode.AnyReleased
                        && !isFingerGrabbing)
                    {
                        return false;
                    }

                    if (fingers.UnselectMode == FingerUnselectMode.AllReleased
                        && isFingerGrabbing)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determine whether the state of any of the finger pinches have changed this frame to
        /// the target pinching state (on/off).
        /// </summary>
        /// <param name="fingers">Finger rules to check.</param>
        public bool IsHandSelectPinchFingersChanged(in GrabbingRule fingers)
        {
            return IsHandSelectFingersChanged(fingers, _fingerPinchAPI);
        }

        /// <summary>
        /// Determine whether the state of any of the finger grabs have changed this frame to
        /// the target grabbing state (on/off).
        /// </summary>
        /// <param name="fingers">Finger rules to check.</param>
        public bool IsHandSelectPalmFingersChanged(in GrabbingRule fingers)
        {
            return IsHandSelectFingersChanged(fingers, _fingerPalmAPI);
        }

        public bool IsHandUnselectPinchFingersChanged(in GrabbingRule fingers)
        {
            return IsHandUnselectFingersChanged(fingers, _fingerPinchAPI);
        }

        public bool IsHandUnselectPalmFingersChanged(in GrabbingRule fingers)
        {
            return IsHandUnselectFingersChanged(fingers, _fingerPalmAPI);
        }

        private bool IsHandSelectFingersChanged(in GrabbingRule fingers, IFingerAPI fingerAPI)
        {
            bool selectsWithOptionals = fingers.SelectsWithOptionals;
            bool aFingerGrabbed = false;

            for (int i = 0; i < Constants.NUM_FINGERS; i++)
            {
                HandFinger finger = (HandFinger)i;
                if (fingers[finger] == FingerRequirement.Required)
                {
                    if (!fingerAPI.GetFingerIsGrabbing(finger))
                    {
                        return false;
                    }

                    if (fingerAPI.GetFingerIsGrabbingChanged(finger, true))
                    {
                        aFingerGrabbed = true;
                    }
                }
                else if (selectsWithOptionals
                    && fingers[finger] == FingerRequirement.Optional)
                {
                    if (fingerAPI.GetFingerIsGrabbingChanged(finger, true))
                    {
                        return true;
                    }
                }
            }

            return aFingerGrabbed;
        }

        private bool IsHandUnselectFingersChanged(in GrabbingRule fingers, IFingerAPI fingerAPI)
        {
            bool isAnyFingerGrabbing = false;
            bool aFingerUngrabbed = false;
            bool selectsWithOptionals = fingers.SelectsWithOptionals;
            for (int i = 0; i < Constants.NUM_FINGERS; i++)
            {
                HandFinger finger = (HandFinger)i;
                if (fingers[finger] == FingerRequirement.Ignored)
                {
                    continue;
                }

                isAnyFingerGrabbing |= fingerAPI.GetFingerIsGrabbing(finger);
                if (fingers[finger] == FingerRequirement.Required)
                {
                    if (fingerAPI.GetFingerIsGrabbingChanged(finger, false))
                    {
                        aFingerUngrabbed = true;
                        if (fingers.UnselectMode == FingerUnselectMode.AnyReleased)
                        {
                            return true;
                        }
                    }
                }

                if (fingers[finger] == FingerRequirement.Optional)
                {
                    if (fingerAPI.GetFingerIsGrabbingChanged(finger, false))
                    {
                        aFingerUngrabbed = true;
                        if (fingers.UnselectMode == FingerUnselectMode.AnyReleased
                            && selectsWithOptionals)
                        {
                            return true;
                        }
                    }
                }
            }

            return !isAnyFingerGrabbing && aFingerUngrabbed;
        }

        public Vector3 GetPinchCenter()
        {
            return WristOffsetToWorldPoint(_fingerPinchAPI.GetCenterOffset());
        }

        public Vector3 GetPalmCenter()
        {
            return WristOffsetToWorldPoint(_fingerPalmAPI.GetCenterOffset());
        }

        private Vector3 WristOffsetToWorldPoint(Vector3 offset)
        {
            if (!Hand.GetJointPose(HandJointId.HandWristRoot, out Pose wristPose))
            {
                return offset;
            }

            return wristPose.position + wristPose.rotation * offset;
        }

        public float GetHandPinchStrength(in GrabbingRule fingers,
            bool includePinching = true)
        {
            return GetHandStrength(fingers, includePinching, _fingerPinchAPI);
        }

        public float GetHandPalmStrength(in GrabbingRule fingers,
            bool includeGrabbing = true)
        {
            return GetHandStrength(fingers, includeGrabbing, _fingerPalmAPI);
        }

        public float GetFingerPinchStrength(HandFinger finger)
        {
            return _fingerPinchAPI.GetFingerGrabStrength(finger);
        }

        public float GetFingerPalmStrength(HandFinger finger)
        {
            return _fingerPalmAPI.GetFingerGrabStrength(finger);
        }

        private float GetHandStrength(in GrabbingRule fingers,
            bool includeGrabbing, IFingerAPI fingerAPI)
        {
            float requiredMin = 1.0f;
            float optionalMax = 0f;
            for (int i = 0; i < Constants.NUM_FINGERS; i++)
            {
                HandFinger finger = (HandFinger)i;
                if (!includeGrabbing && fingerAPI.GetFingerIsGrabbing((HandFinger)i))
                {
                    continue;
                }

                if (fingers[finger] == FingerRequirement.Ignored)
                {
                    continue;
                }

                if (fingers[finger] == FingerRequirement.Optional)
                {
                    optionalMax = Mathf.Max(optionalMax, fingerAPI.GetFingerGrabStrength((HandFinger)i));
                }

                if (fingers[finger] == FingerRequirement.Required)
                {
                    requiredMin = Mathf.Min(requiredMin, fingerAPI.GetFingerGrabStrength((HandFinger)i));
                }

            }
            return Mathf.Min(requiredMin, optionalMax);
        }

        #region Inject

        public void InjectAllHandGrabAPI(IHand hand)
        {
            InjectHand(hand);
        }

        public void InjectHand(IHand hand)
        {
            _hand = hand as MonoBehaviour;
            Hand = hand;
        }

        public void InjectOptionalFingerPinchAPI(IFingerAPI fingerPinchAPI)
        {
            _fingerPinchAPI = fingerPinchAPI;
        }

        public void InjectOptionalFingerGrabAPI(IFingerAPI fingerGrabAPI)
        {
            _fingerPalmAPI = fingerGrabAPI;
        }

        #endregion
    }
}
