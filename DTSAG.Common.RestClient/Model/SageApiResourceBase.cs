using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace DTSAG.Common.RestClient.Model {
    public abstract class SageApiResourceBase<TImplementingType> : INotifyPropertyChanged where TImplementingType : SageApiResourceBase<TImplementingType>, new() {
        #region INotifyPropertyChanged Implementierung
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        [JsonProperty("$key", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string SageKey { get; set; } = null;

        [JsonProperty("$etag", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string SageEtag { get; set; } = null;

        [JsonProperty("$diagnoses", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IEnumerable<SageApiDiagnose> SageDiagnoses { get; set; } = Array.Empty<SageApiDiagnose>();

        [JsonProperty(nameof(CustomFields), Required = Required.Default, NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IEnumerable<SageNameValuePair> CustomFields { get; set; } = Array.Empty<SageNameValuePair>();

        public virtual TImplementingType Clone() {
            var clone = new TImplementingType() {
                SageKey = SageKey,
                SageEtag = SageEtag
            };
            // CustomFields klonen... Es wird davon ausgegangen, dass die Value immer ein Value-Type ist!
            var clonedCustomFields = new List<SageNameValuePair>();
            foreach (var field in this.CustomFields) {
                var clonedField = new SageNameValuePair(field.Name, field.Value);
                clonedCustomFields.Add(clonedField);
            }
            clone.CustomFields = clonedCustomFields.ToArray();
            return clone;
        }
    }
}