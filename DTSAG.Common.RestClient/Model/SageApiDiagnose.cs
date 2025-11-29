using Newtonsoft.Json;

namespace DTSAG.Common.RestClient.Model {
    public class SageApiDiagnose {
        [JsonProperty("severity", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Severity { get; set; } = null;

        [JsonProperty("sdataCode", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string SdataCode { get; set; } = null;

        [JsonProperty("applicationCode", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string ApplicationCode { get; set; } = null;

        [JsonProperty("message", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; } = null;

        [JsonProperty("stackTrace", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string StackTrace { get; set; } = null;

        [JsonProperty("payloadPath", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string PayloadPath { get; set; } = null;
    }
}