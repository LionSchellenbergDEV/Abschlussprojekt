using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DTSAG.Common.RestClient.Model;

namespace DTSAG.Common.RestClient {
    public class SageRestClient {
        protected ISageClientConfig Config { get; private set; }

        protected HttpClient HttpClient { get; private set; }

        public SageRestClient(ISageClientConfig config) {
            this.Config = config;
            var httpClientHandler = new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => {
                    // HACK: Allen SSL-Zertifikaten vertrauen...
                    return true;
                }
            };
            this.HttpClient = new HttpClient(httpClientHandler);
        }

        public async Task<IEnumerable<T>> GetDataSourceAsync<T>(string api, string endpoint, string where = null) {
            var dataset = await this.Config.GetDataSetAsync();
            var url = $"{this.Config.GetBaseURL()}/sdata/ol/{api}/{dataset}/{endpoint}";
            if (!string.IsNullOrEmpty(where)) {
                if (!url.Contains("?")) {
                    url = url + "?";
                } else {
                    url = url + "&";
                }
                url = url + $"where={where}";
            }
            using (var request = await GenerateRequest(HttpMethod.Get, url)) {
                var response = await this.HttpClient.SendAsync(request);
                await HandleErrorResponseAsync(response);
                var contentString = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<SageApiResult<T>>(contentString);
                IEnumerable<T> result = apiResult?.Resources ?? Array.Empty<T>();
                return result;
            }
        }

        public async Task<IEnumerable<T>> GetListAsync<T>(string api, string endpoint, string where = null, int? startIndex = null, int? count = null) where T : SageApiResourceBase<T>, new() {
            var dataset = await this.Config.GetDataSetAsync();
            var url = $"{this.Config.GetBaseURL()}/sdata/ol/{api}/{dataset}/{endpoint}";
            if (!string.IsNullOrEmpty(where)) {
                url = AppendQueryParameter(url, "where", where);
            }
            if (startIndex != null) {
                url = AppendQueryParameter(url, "startIndex", startIndex.ToString());
            }
            if (count != null) {
                url = AppendQueryParameter(url, "count", count.ToString());
            }
            using (var request = await GenerateRequest(HttpMethod.Get, url)) {
                var response = await this.HttpClient.SendAsync(request);
                await HandleErrorResponseAsync(response);
                var contentString = await response.Content.ReadAsStringAsync();
                var apiResult = JsonConvert.DeserializeObject<SageApiResult<T>>(contentString);
                IEnumerable<T> result = apiResult?.Resources ?? Array.Empty<T>();
                return result;
            }
        }

        public async Task<T> GetAsync<T>(string api, string endpoint, string key, string include = null) where T : SageApiResourceBase<T>, new() {
            var dataset = await this.Config.GetDataSetAsync();
            var url = $"{this.Config.GetBaseURL()}/sdata/ol/{api}/{dataset}/{endpoint}('{key}')";
            if (!string.IsNullOrEmpty(include)) {
                if (!url.Contains("?")) {
                    url = url + "?";
                } else {
                    url = url + "&";
                }
                url = url + $"include={include}";
            }
            using (var request = await GenerateRequest(HttpMethod.Get, url)) {
                var response = await this.HttpClient.SendAsync(request);
                await HandleErrorResponseAsync(response);
                var contentString = await response.Content.ReadAsStringAsync();
                var singleResult = JsonConvert.DeserializeObject<T>(contentString);
                return singleResult;
            }
        }

        public async Task<T> GetTemplateAsync<T>(string api, string endpoint) {
            var dataset = await this.Config.GetDataSetAsync();
            var url = $"{this.Config.GetBaseURL()}/sdata/ol/{api}/{dataset}/{endpoint}/$template";
            using (var request = await GenerateRequest(HttpMethod.Get, url)) {
                var response = await this.HttpClient.SendAsync(request);
                await HandleErrorResponseAsync(response);
                var contentString = await response.Content.ReadAsStringAsync();
                var singleResult = JsonConvert.DeserializeObject<T>(contentString);
                return singleResult;
            }
        }

        public async Task<IEnumerable<T>> GetDataReferenceAsync<T>(string api, string endpoint, string endpointForDataReference, string where = "", int? startIndex = null, int? count = null) {
            var dataset = await this.Config.GetDataSetAsync();
            string url = $"{this.Config.GetBaseURL()}/sdata/ol/{api}/{dataset}/{endpoint}";
            if (!string.IsNullOrEmpty(where)) {
                url = AppendQueryParameter(url, "where", where);
            }
            if (startIndex != null) {
                url = AppendQueryParameter(url, "startIndex", startIndex.ToString());
            }
            if (count != null) {
                url = AppendQueryParameter(url, "count", count.ToString());
            }
            using (HttpRequestMessage request = await GenerateRequest(HttpMethod.Get, url, endpointForDataReference)) {
                var response = await this.HttpClient.SendAsync(request);
                await HandleErrorResponseAsync(response);
                // Ergebnis des API-Calls auswerten...
                // ACHTUNG! response.Content.ReadFromJsonAsync() funktioniert NICHT!
                var contentString = await response.Content.ReadAsStringAsync();
                // Ergebnis je nach Anfrage parsen...
                IEnumerable<T> result;
                // Result-List wird zurückgegeben.
                var apiResult = JsonConvert.DeserializeObject<SageApiResult<T>>(contentString);
                result = apiResult.Resources ?? Array.Empty<T>();
                // Elemente zurückgeben.
                return result;
            }
        }

        public async Task<T> AddAsync<T>(string api, string endpoint, T data) where T : SageApiResourceBase<T>, new() {
            var dataset = await this.Config.GetDataSetAsync();
            var url = $"{this.Config.GetBaseURL()}/sdata/ol/{api}/{dataset}/{endpoint}";
            using (var request = await GenerateRequest(HttpMethod.Post, url)) {
                var postString = JsonConvert.SerializeObject(data);
                request.Content = new StringContent(postString, Encoding.UTF8, "application/json");
                var response = await this.HttpClient.SendAsync(request);
                await HandleErrorResponseAsync(response);
                var contentString = await response.Content.ReadAsStringAsync();
                var singleResult = JsonConvert.DeserializeObject<T>(contentString);
                return singleResult;
            }
        }
        public async Task<T> UpdateFullAsync<T>(string api, string endpoint, T data) where T : SageApiResourceBase<T>, new() {
            var dataset = await this.Config.GetDataSetAsync();
            if (string.IsNullOrWhiteSpace(data.SageKey)) throw new ArgumentNullException(nameof(data.SageKey));
            if (string.IsNullOrWhiteSpace(data.SageEtag)) throw new ArgumentNullException(nameof(data.SageEtag));
            var sageKey = data.SageKey;
            var sageEtag = data.SageEtag;
            var url = $"{this.Config.GetBaseURL()}/sdata/ol/{api}/{dataset}/{endpoint}('{sageKey}')";
            using (var request = await GenerateRequest(HttpMethod.Put, url, ifMatch: sageEtag)) {
                request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await this.HttpClient.SendAsync(request);
                await HandleErrorResponseAsync(response);
                var contentString = await response.Content.ReadAsStringAsync();
                var singleResult = JsonConvert.DeserializeObject<T>(contentString);
                return singleResult;
            }
        }

        public async Task<T> UpdatePartAsync<T>(string api, string endpoint, T data) where T : SageApiResourceBase<T>, new() {
            var dataset = await this.Config.GetDataSetAsync();
            if (string.IsNullOrWhiteSpace(data.SageKey)) throw new ArgumentNullException(nameof(data.SageKey));
            if (string.IsNullOrWhiteSpace(data.SageEtag)) throw new ArgumentNullException(nameof(data.SageEtag));
            var sageKey = data.SageKey;
            var sageEtag = data.SageEtag;
            var url = $"{this.Config.GetBaseURL()}/sdata/ol/{api}/{dataset}/{endpoint}('{sageKey}')";
            using (var request = await GenerateRequest(HttpMethod.Patch, url, ifMatch: sageEtag)) {
                request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await this.HttpClient.SendAsync(request);
                await HandleErrorResponseAsync(response);
                var contentString = await response.Content.ReadAsStringAsync();
                var singleResult = JsonConvert.DeserializeObject<T>(contentString);
                return singleResult;
            }
        }

        public async Task DeleteAsync(string api, string endpoint, string key) {
            var dataset = await this.Config.GetDataSetAsync();
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            var url = $"{this.Config.GetBaseURL()}/sdata/ol/{api}/{dataset}/{endpoint}('{key}')";
            using (var request = await GenerateRequest(HttpMethod.Delete, url)) {
                var response = await this.HttpClient.SendAsync(request);
                await HandleErrorResponseAsync(response);
            }
        }

        protected string AppendQueryParameter(string url, string parameterName, string parameterValue) {
            string tempUrl = url;
            if (!tempUrl.Contains("?")) {
                tempUrl += "?";
            } else {
                tempUrl += "&";
            }
            tempUrl += $"{Uri.EscapeDataString(parameterName)}={Uri.EscapeDataString(parameterValue)}";
            return tempUrl;
        }

        /// <summary>
        /// Erzeugt einen Standard-Request für API-Anfragen.
        /// Fehler werden nicht behandelt.
        /// </summary>
        /// <param name="method">HTTP-Methode.</param>
        /// <param name="uri">Aufzurufende URI.</param>
        /// <param name="apiEndpointForDataReference">Optional: Wird eine Datenreferenz angefordert, muss hier der "übergeordnete" API-Endpunkt angegeben werden.</param>
        /// <param name="ifMatch">If-Match-Header.</param>
        protected async Task<HttpRequestMessage> GenerateRequest(HttpMethod method, string uri, string apiEndpointForDataReference = null, string ifMatch = null) {
            var request = new HttpRequestMessage(method, uri);
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("prefer", "compact");
            request.Headers.Add("X-Sage-ConnectivityVersion", "1.3");
            if (!string.IsNullOrWhiteSpace(apiEndpointForDataReference)) {
                request.Headers.Add("X-ApiEndpoint", apiEndpointForDataReference);
            }
            string token = await this.Config.GetTokenAsync();
            if (this.Config.IsLocalAPI()) {
                request.Headers.Add("Authorization", $"Basic {token}");
            } else {
                request.Headers.Add("Authorization", $"Bearer {token}");
            }
            if (!string.IsNullOrWhiteSpace(ifMatch)) {
                request.Headers.TryAddWithoutValidation("if-match", ifMatch);
            }
            return request;
        }

        protected virtual async Task HandleErrorResponseAsync(HttpResponseMessage response) {
            try {
                // Prüfen, ob der Status OK ist...
                response.EnsureSuccessStatusCode();
            } catch {
                // Versuchen, ein Diagnose-Objekt zu ermitteln...
                IEnumerable<SageApiDiagnose> diagnoses = Array.Empty<SageApiDiagnose>();
                try {
                    var contentString = await response.Content.ReadAsStringAsync();
                    var diagnoseContainer = JsonConvert.DeserializeObject<SageApiDiagnoseResult>(contentString);
                    if (diagnoseContainer?.Diagnoses != null) {
                        diagnoses = diagnoseContainer.Diagnoses;
                    }
                } catch { /* Fehler ignorieren! */ }
                // Fehler aufbauen...
                Exception detailException = null;
                if (diagnoses.Any()) {
                    foreach (var diag in diagnoses) {
                        // Meldung aus der Diagnose ermitteln...
                        var message = diag.Message;
                        if (string.IsNullOrWhiteSpace(message)) {
                            message = diag.ApplicationCode;
                        }
                        // Gibt es keinerlei Meldung, wird die Diagnose übersprungen.
                        if (string.IsNullOrWhiteSpace(message)) continue;
                        // Exception "verschachteln"...
                        detailException = new Exception(message, detailException);
                        detailException.Source = diag.SdataCode;
                    }
                } else if (response.StatusCode == HttpStatusCode.PreconditionFailed) {
                    // HTTP 412, aber keine Diagnose-Meldung:
                    detailException = new Exception("Der Datensatz wurde zwischenzeitlich verändert. Bitte laden Sie den Datensatz neu, bevor Sie ihn bearbeiten.");
                } else if (response.StatusCode == HttpStatusCode.Gone) {
                    // HTTP 410, aber keine Diagnose-Meldung:
                    detailException = new Exception("Der Datensatz wurde nicht gefunden.");
                }
                // Fehler auslösen...
                if (detailException == null) {
                    throw;
                } else {
                    throw detailException;
                }
            }
        }
    }
}