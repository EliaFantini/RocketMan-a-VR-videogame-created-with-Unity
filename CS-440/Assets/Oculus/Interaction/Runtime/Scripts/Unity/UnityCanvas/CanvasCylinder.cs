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
using UnityEditor;
using UnityEngine;

namespace Oculus.Interaction.UnityCanvas
{
    public class CanvasCylinder : CanvasRenderTextureMesh
    {
        [Serializable]
        public struct MeshGenerationSettings
        {
            [Delayed]
            public float VerticesPerDegree;

            [Delayed]
            public int MaxHorizontalResolution;

            [Delayed]
            public int MaxVerticalResolution;
        }

        public const int MIN_RESOLUTION = 2;

        [Tooltip("The radius of the cylinder that the Canvas texture is projected onto.")]
        [Delayed]
        [SerializeField]
        private float _curveRadius = 1;

        [SerializeField]
        private MeshGenerationSettings _meshGeneration = new MeshGenerationSettings()
        {
            VerticesPerDegree = 1.4f,
            MaxHorizontalResolution = 128,
            MaxVerticalResolution = 32
        };

        protected override OVROverlay.OverlayShape OverlayShape => OVROverlay.OverlayShape.Cylinder;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            _curveRadius = Mathf.Max(0.01f, _curveRadius);
            _meshGeneration.MaxHorizontalResolution = Mathf.Max(MIN_RESOLUTION,
                _meshGeneration.MaxHorizontalResolution);
            _meshGeneration.MaxVerticalResolution = Mathf.Max(MIN_RESOLUTION,
                _meshGeneration.MaxVerticalResolution);
            _meshGeneration.VerticesPerDegree = Mathf.Max(0, _meshGeneration.VerticesPerDegree);

            if (Application.isPlaying && _started)
            {
                EditorApplication.delayCall += () =>
                {
                    UpdateImposter();
                };
            }
        }
#endif
        public float CurveRadius
        {
            get
            {
                return _curveRadius;
            }
            set
            {
                if (_curveRadius == value)
                {
                    return;
                }

                _curveRadius = value;

                if (isActiveAndEnabled && Application.isPlaying)
                {
                    UpdateImposter();
                }
            }
        }

        protected override void UpdateOverlayPositionAndScale()
        {
            if (_overlay == null)
            {
                return;
            }

            var resolution = _canvasRenderTexture.GetBaseResolutionToUse();
            _overlay.transform.localPosition = new Vector3(0, 0, -_curveRadius) - _runtimeOffset;
            _overlay.transform.localScale = new Vector3(_canvasRenderTexture.PixelsToUnits(resolution.x),
                                                        _canvasRenderTexture.PixelsToUnits(resolution.y),
                                                        _curveRadius);
        }

        protected override Vector3 MeshInverseTransform(Vector3 localPosition)
        {
            float angle = Mathf.Atan2(localPosition.x, localPosition.z + _curveRadius);
            float x = angle * _curveRadius;
            float y = localPosition.y;
            return new Vector3(x, y);
        }

        protected override void GenerateMesh(out List<Vector3> verts,
                                             out List<int> tris,
                                             out List<Vector2> uvs)
        {
            verts = new List<Vector3>();
            tris = new List<int>();
            uvs = new List<Vector2>();

            var resolution = _canvasRenderTexture.GetBaseResolutionToUse();

            float xPos = _canvasRenderTexture.PixelsToUnits(Mathf.RoundToInt(resolution.x)) * 0.5f;
            float xNeg = -xPos;

            float yPos = _canvasRenderTexture.PixelsToUnits(Mathf.RoundToInt(resolution.y)) * 0.5f;
            float yNeg = -yPos;

            int horizontalResolution = Mathf.Max(2,
                Mathf.RoundToInt(_meshGeneration.VerticesPerDegree * Mathf.Rad2Deg * xPos /
                                 _curveRadius));
            int verticalResolution =
                Mathf.Max(2, Mathf.RoundToInt(horizontalResolution * yPos / xPos));

            horizontalResolution = Mathf.Clamp(horizontalResolution, 2,
                _meshGeneration.MaxHorizontalResolution);
            verticalResolution = Mathf.Clamp(verticalResolution, 2,
                _meshGeneration.MaxVerticalResolution);

            Vector3 getCurvedPoint(float u, float v)
            {
                float x = Mathf.Lerp(xNeg, xPos, u);
                float y = Mathf.Lerp(yNeg, yPos, v);

                float angle = x / _curveRadius;
                Vector3 point;
                point.x = Mathf.Sin(angle) * _curveRadius;
                point.y = y;
                point.z = Mathf.Cos(angle) * _curveRadius - _curveRadius;
                return point;
            }

            for (int y = 0; y < verticalResolution; y++)
            {
                for (int x = 0; x < horizontalResolution; x++)
                {
                    float u = x / (horizontalResolution - 1.0f);
                    float v = y / (verticalResolution - 1.0f);

                    verts.Add(getCurvedPoint(u, v));
                    uvs.Add(new Vector2(u, v));
                }
            }

            for (int y = 0; y < verticalResolution - 1; y++)
            {
                for (int x = 0; x < horizontalResolution - 1; x++)
                {
                    int v00 = x + y * horizontalResolution;
                    int v10 = v00 + 1;
                    int v01 = v00 + horizontalResolution;
                    int v11 = v00 + 1 + horizontalResolution;

                    tris.Add(v00);
                    tris.Add(v11);
                    tris.Add(v10);

                    tris.Add(v00);
                    tris.Add(v01);
                    tris.Add(v11);
                }
            }
        }

        #region Inject

        public void InjectAllCanvasCylinder(CanvasRenderTexture canvasRenderTexture,
                                            float curveRadius)
        {
            InjectAllCanvasRenderTextureMesh(canvasRenderTexture);
            InjectCurveRadius(curveRadius);
        }

        public void InjectCurveRadius(float curveRadius)
        {
            _curveRadius = curveRadius;
        }

        #endregion
    }
}
