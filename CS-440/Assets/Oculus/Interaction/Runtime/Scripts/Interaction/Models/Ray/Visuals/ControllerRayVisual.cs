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

namespace Oculus.Interaction
{
    public class ControllerRayVisual : MonoBehaviour
    {
        [SerializeField]
        private RayInteractor _rayInteractor;

        [SerializeField]
        private MaterialPropertyBlockEditor _materialPropertyBlockEditor;

        [SerializeField]
        private float _maxRayVisualLength = 0.5f;

        [SerializeField]
        private Color _hoverColor0 = Color.white;

        [SerializeField]
        private Color _hoverColor1 = Color.white;

        [SerializeField]
        private Color _selectColor0 = Color.blue;

        [SerializeField]
        private Color _selectColor1 = Color.blue;

        #region Properties

        public float MaxRayVisualLength
        {
            get
            {
                return _maxRayVisualLength;
            }

            set
            {
                _maxRayVisualLength = value;
            }
        }

        public Color HoverColor0
        {
            get
            {
                return _hoverColor0;
            }

            set
            {
                _hoverColor0 = value;
            }
        }

        public Color HoverColor1
        {
            get
            {
                return _hoverColor1;
            }

            set
            {
                _hoverColor1 = value;
            }
        }

        public Color SelectColor0
        {
            get
            {
                return _selectColor0;
            }

            set
            {
                _selectColor0 = value;
            }
        }

        public Color SelectColor1
        {
            get
            {
                return _selectColor1;
            }

            set
            {
                _selectColor1 = value;
            }
        }

        #endregion

        private int _shaderColor0 = Shader.PropertyToID("_Color0");
        private int _shaderColor1 = Shader.PropertyToID("_Color1");

        private bool _started;

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(_rayInteractor);
            Assert.IsNotNull(_materialPropertyBlockEditor);
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                _rayInteractor.WhenInteractorUpdated += UpdateVisual;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                _rayInteractor.WhenInteractorUpdated -= UpdateVisual;
            }
        }

        private void UpdateVisual()
        {
            transform.SetPositionAndRotation(_rayInteractor.Origin, _rayInteractor.Rotation);

            transform.localScale = new Vector3(
                transform.localScale.x,
                transform.localScale.y,
                Mathf.Min(_maxRayVisualLength, (_rayInteractor.End - transform.position).magnitude));

            _materialPropertyBlockEditor.MaterialPropertyBlock.SetColor(_shaderColor0, _rayInteractor.State == InteractorState.Select ? _selectColor0 : _hoverColor0);
            _materialPropertyBlockEditor.MaterialPropertyBlock.SetColor(_shaderColor1, _rayInteractor.State == InteractorState.Select ? _selectColor1 : _hoverColor1);
        }

        #region Inject

        public void InjectAllControllerRayVisual(RayInteractor rayInteractor, MaterialPropertyBlockEditor materialPropertyBlockEditor)
        {
            InjectRayInteractor(rayInteractor);
            InjectMaterialPropertyBlockEditor(materialPropertyBlockEditor);
        }

        public void InjectRayInteractor(RayInteractor rayInteractor)
        {
            _rayInteractor = rayInteractor;
        }

        public void InjectMaterialPropertyBlockEditor(
            MaterialPropertyBlockEditor materialPropertyBlockEditor)
        {
            _materialPropertyBlockEditor = materialPropertyBlockEditor;
        }

        #endregion
    }
}
