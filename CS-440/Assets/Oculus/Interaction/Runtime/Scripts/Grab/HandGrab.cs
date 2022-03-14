/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Your use of this SDK or tool is subject to the Oculus SDK License Agreement, available at
https://developer.oculus.com/licenses/oculussdk/

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.Input;

namespace Oculus.Interaction.Grab
{
    public enum GrabType
    {
        PinchGrab = 1 << 0,
        PalmGrab = 1 << 1
    }

    public interface IHandGrabInteractor
    {
        HandGrabAPI HandGrabApi { get; }
        GrabTypeFlags SupportedGrabTypes { get; }
        IHandGrabInteractable TargetInteractable { get; }
    }

    public interface IHandGrabInteractable
    {
        GrabTypeFlags SupportedGrabTypes { get; }
        GrabbingRule PinchGrabRules { get; }
        GrabbingRule PalmGrabRules { get; }
    }

    public class HandGrabInteractableData : IHandGrabInteractable
    {
        public GrabTypeFlags SupportedGrabTypes { get; set; } = GrabTypeFlags.All;
        public GrabbingRule PinchGrabRules { get; set; } = GrabbingRule.DefaultPinchRule;
        public GrabbingRule PalmGrabRules { get; set; } = GrabbingRule.DefaultPalmRule;
    }

    public static class HandGrab
    {
        public static void StoreGrabData(IHandGrabInteractor interactor,
            IHandGrabInteractable interactable, ref HandGrabInteractableData cache)
        {
            HandGrabAPI api = interactor.HandGrabApi;

            cache.SupportedGrabTypes = GrabTypeFlags.None;

            if (SupportsPinch(interactor, interactable))
            {
                HandFingerFlags pinchFingers = api.HandPinchingFinger();
                if (api.IsSustainingGrab(interactable.PinchGrabRules, pinchFingers))
                {
                    cache.SupportedGrabTypes |= GrabTypeFlags.Pinch;
                    cache.PinchGrabRules = new GrabbingRule(pinchFingers, interactable.PinchGrabRules);
                }
            }
            if (SupportsPalm(interactor, interactable))
            {
                HandFingerFlags palmFingers = api.HandPalmGrabbingFingers();
                if(api.IsSustainingGrab(interactable.PalmGrabRules, palmFingers))
                {
                    cache.SupportedGrabTypes |= GrabTypeFlags.Palm;
                    cache.PalmGrabRules = new GrabbingRule(palmFingers, interactable.PalmGrabRules);
                }
            }
        }

        public static float ComputeHoverStrength(IHandGrabInteractor interactor,
            IHandGrabInteractable interactable, out GrabTypeFlags hoveringGrabTypes)
        {
            HandGrabAPI api = interactor.HandGrabApi;
            hoveringGrabTypes = GrabTypeFlags.None;
            float hoverStrength = 0f;

            if (SupportsPinch(interactor, interactable))
            {
                float pinchStrength = api.GetHandPinchStrength(interactable.PinchGrabRules, false);
                if (pinchStrength > hoverStrength)
                {
                    hoverStrength = pinchStrength;
                    hoveringGrabTypes = GrabTypeFlags.Pinch;
                }
            }

            if (SupportsPalm(interactor, interactable))
            {
                float palmStrength = api.GetHandPalmStrength(interactable.PalmGrabRules, false);
                if (palmStrength > hoverStrength)
                {
                    hoverStrength = palmStrength;
                    hoveringGrabTypes = GrabTypeFlags.Palm;
                }
            }

            return hoverStrength;
        }

        public static bool ComputeShouldSelect(IHandGrabInteractor interactor,
            IHandGrabInteractable interactable, out GrabTypeFlags selectingGrabTypes)
        {
            if (interactable == null)
            {
                selectingGrabTypes = GrabTypeFlags.None;
                return false;
            }

            HandGrabAPI api = interactor.HandGrabApi;
            selectingGrabTypes = GrabTypeFlags.None;
            if (SupportsPinch(interactor, interactable) &&
                 api.IsHandSelectPinchFingersChanged(interactable.PinchGrabRules))
            {
                selectingGrabTypes |= GrabTypeFlags.Pinch;
            }

            if (SupportsPalm(interactor, interactable) &&
                 api.IsHandSelectPalmFingersChanged(interactable.PalmGrabRules))
            {
                selectingGrabTypes |= GrabTypeFlags.Palm;
            }

            return selectingGrabTypes != GrabTypeFlags.None;
        }

        public static bool ComputeShouldUnselect(IHandGrabInteractor interactor,
            IHandGrabInteractable interactable)
        {
            HandGrabAPI api = interactor.HandGrabApi;
            HandFingerFlags pinchFingers = api.HandPinchingFinger();
            HandFingerFlags palmFingers = api.HandPalmGrabbingFingers();

            if (interactable.SupportedGrabTypes == GrabTypeFlags.None)
            {
                if (!api.IsSustainingGrab(GrabbingRule.FullGrab, pinchFingers) &&
                    !api.IsSustainingGrab(GrabbingRule.FullGrab, palmFingers))
                {
                    return true;
                }
                return false;
            }

            bool pinchHolding = false;
            bool palmHolding = false;
            bool pinchReleased = false;
            bool palmReleased = false;

            if (SupportsPinch(interactor, interactable.SupportedGrabTypes))
            {
                pinchHolding = api.IsSustainingGrab(interactable.PinchGrabRules, pinchFingers);

                if (api.IsHandUnselectPinchFingersChanged(interactable.PinchGrabRules))
                {
                    pinchReleased = true;
                }
            }

            if (SupportsPalm(interactor, interactable.SupportedGrabTypes))
            {
                palmHolding = api.IsSustainingGrab(interactable.PalmGrabRules, palmFingers);
                if (api.IsHandUnselectPalmFingersChanged(interactable.PalmGrabRules))
                {
                    palmReleased = true;
                }
            }

            return !pinchHolding && !palmHolding && (pinchReleased || palmReleased);
        }

        public static HandFingerFlags GrabbingFingers(IHandGrabInteractor interactor,
            IHandGrabInteractable interactable)
        {
            HandGrabAPI api = interactor.HandGrabApi;
            if (interactable == null)
            {
                return HandFingerFlags.None;
            }

            HandFingerFlags fingers = HandFingerFlags.None;

            if (SupportsPinch(interactor, interactable))
            {
                HandFingerFlags pinchingFingers = api.HandPinchingFinger();
                interactable.PinchGrabRules.StripIrrelevant(ref pinchingFingers);
                fingers = fingers | pinchingFingers;
            }

            if (SupportsPalm(interactor, interactable))
            {
                HandFingerFlags grabbingFingers = api.HandPalmGrabbingFingers();
                interactable.PalmGrabRules.StripIrrelevant(ref grabbingFingers);
                fingers = fingers | grabbingFingers;
            }

            return fingers;
        }

        private static bool SupportsPinch(IHandGrabInteractor interactor,
            IHandGrabInteractable interactable)
        {
            return SupportsPinch(interactor, interactable.SupportedGrabTypes);
        }

        private static bool SupportsPalm(IHandGrabInteractor interactor,
            IHandGrabInteractable interactable)
        {
            return SupportsPalm(interactor, interactable.SupportedGrabTypes);
        }

        private static bool SupportsPinch(IHandGrabInteractor interactor,
            GrabTypeFlags grabTypes)
        {
            return (interactor.SupportedGrabTypes & GrabTypeFlags.Pinch) != 0 &&
                (grabTypes & GrabTypeFlags.Pinch) != 0;
        }

        private static bool SupportsPalm(IHandGrabInteractor interactor,
            GrabTypeFlags grabTypes)
        {
            return (interactor.SupportedGrabTypes & GrabTypeFlags.Palm) != 0 &&
                (grabTypes & GrabTypeFlags.Palm) != 0;
        }
    }
}
