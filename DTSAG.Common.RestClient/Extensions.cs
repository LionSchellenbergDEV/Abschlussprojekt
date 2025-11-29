using System.Collections.Generic;
using DTSAG.Common.RestClient.Model;

namespace DTSAG.Common.RestClient {
    public static class ConversionExtensions {
        /// <summary>
        /// Konvertiert das <see cref="SageNameValuePair"/> in ein <see cref="KeyValuePair"/>.
        /// </summary>
        /// <param name="nameValuePair">Zu konvertierendes <see cref="SageNameValuePair"/>.</param>
        public static KeyValuePair<string, object> ToKeyValuePair(this SageNameValuePair nameValuePair) {
            return new KeyValuePair<string, object>(nameValuePair.Name, nameValuePair.Value);
        }

        /// <summary>
        /// Konvertiert das <see cref="KeyValuePair"/> in ein <see cref="SageNameValuePair"/>.
        /// </summary>
        /// <param name="keyValuePair">Zu konvertierendes <see cref="KeyValuePair"/>.</param>
        public static SageNameValuePair ToSageNameValuePair(this KeyValuePair<string, object> keyValuePair) {
            return new SageNameValuePair(keyValuePair.Key, keyValuePair.Value);
        }

        /// <summary>
        /// Konvertiert die <see cref="SageNameValuePair"/>-Auflistung in eine <see cref="KeyValuePair"/>-Auflistung.
        /// </summary>
        /// <remarks>
        /// Der unterliegende Typ ist eine <see cref="List{T}"/>.
        /// </remarks>
        /// <param name="list">Zu konvertierende <see cref="SageNameValuePair"/>-Auflistung.</param>
        public static IEnumerable<KeyValuePair<string, object>> ToKeyValuePairList(this IEnumerable<SageNameValuePair> list) {
            var results = new List<KeyValuePair<string, object>>();
            if (list != null) {
                foreach (var item in list) {
                    results.Add(item.ToKeyValuePair());
                }
            }
            return results;
        }

        /// <summary>
        /// Konvertiert die <see cref="KeyValuePair"/>-Auflistung in eine <see cref="SageNameValuePair"/>-Auflistung.
        /// </summary>
        /// <remarks>
        /// Der unterliegende Typ ist eine <see cref="List{T}"/>.
        /// </remarks>
        /// <param name="list">Zu konvertierende <see cref="KeyValuePair"/>-Auflistung.</param>
        public static IEnumerable<SageNameValuePair> ToSageNameValuePairList(this IEnumerable<KeyValuePair<string, object>> list) {
            var results = new List<SageNameValuePair>();
            if (list != null) {
                foreach (var item in list) {
                    results.Add(item.ToSageNameValuePair());
                }
            }
            return results;
        }
    }
}