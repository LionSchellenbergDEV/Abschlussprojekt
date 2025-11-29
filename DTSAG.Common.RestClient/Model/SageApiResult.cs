using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DTSAG.Common.RestClient.Model {
    public sealed class SageApiResult<T> {
        [JsonProperty("$resources", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<T> Resources { get; set; } = Array.Empty<T>();
    }
}