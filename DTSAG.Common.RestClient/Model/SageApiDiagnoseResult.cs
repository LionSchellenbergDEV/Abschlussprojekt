using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DTSAG.Common.RestClient.Model {
    public class SageApiDiagnoseResult {
        [JsonProperty("$diagnoses", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IEnumerable<SageApiDiagnose> Diagnoses { get; set; } = Array.Empty<SageApiDiagnose>();
    }
}