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

namespace Oculus.Interaction.Input
{
    public abstract class
        DataModifier<TData, TConfig> : DataSource<TData, TConfig>
        where TData : class, ICopyFrom<TData>, new()
    {
        [Header("Data Modifier")]
        [SerializeField, Interface(nameof(_modifyDataFromSource))]
        protected MonoBehaviour _iModifyDataFromSourceMono;
        private IDataSource<TData, TConfig> _modifyDataFromSource;

        [SerializeField]
        [Tooltip("If this is false, then this modifier will simply pass through " +
                 "data without performing any modification. This saves on memory " +
                 "and computation")]
        private bool _applyModifier = true;

        private static TData InvalidAsset { get; } = new TData();
        private TData _thisDataAsset;
        private TData _currentDataAsset = InvalidAsset;
        private TConfig _configCache;

        protected override TData DataAsset => _currentDataAsset;

        public virtual IDataSource<TData, TConfig> ModifyDataFromSource => _modifyDataFromSource == null
            ? (_modifyDataFromSource = _iModifyDataFromSourceMono as IDataSource<TData, TConfig>)
            : _modifyDataFromSource;

        public override int CurrentDataVersion
        {
            get
            {
                return _applyModifier
                    ? base.CurrentDataVersion
                    : ModifyDataFromSource.CurrentDataVersion;
            }
        }

        public void ResetSources(IDataSource<TData, TConfig> modifyDataFromSource, IDataSource updateAfter, UpdateModeFlags updateMode)
        {
            ResetUpdateAfter(updateAfter, updateMode);
            _modifyDataFromSource = modifyDataFromSource;
            _currentDataAsset = InvalidAsset;
            _configCache = default;
        }

        protected override void UpdateData()
        {
            if (_applyModifier)
            {
                if (_thisDataAsset == null)
                {
                    _thisDataAsset = new TData();
                }

                _thisDataAsset.CopyFrom(ModifyDataFromSource.GetData());
                _currentDataAsset = _thisDataAsset;
                Apply(_currentDataAsset);
            }
            else
            {
                _currentDataAsset = ModifyDataFromSource.GetData();
            }
        }

        protected abstract void Apply(TData data);

        protected override void Start()
        {
            base.Start();
            Assert.IsNotNull(ModifyDataFromSource);
        }

        public override TConfig Config
        {
            get
            {
                return _configCache != null
                    ? _configCache
                    : (_configCache = ModifyDataFromSource.Config);
            }
        }

        #region Inject
        public void InjectAllDataModifier(UpdateModeFlags updateMode, IDataSource updateAfter, IDataSource<TData, TConfig> modifyDataFromSource, bool applyModifier)
        {
            base.InjectAllDataSource(updateMode, updateAfter);
            InjectModifyDataFromSource(modifyDataFromSource);
            InjectApplyModifier(applyModifier);
        }

        public void InjectModifyDataFromSource(IDataSource<TData, TConfig> modifyDataFromSource)
        {
            _modifyDataFromSource = modifyDataFromSource;
        }

        public void InjectApplyModifier(bool applyModifier)
        {
            _applyModifier = applyModifier;
        }
        #endregion
    }
}
