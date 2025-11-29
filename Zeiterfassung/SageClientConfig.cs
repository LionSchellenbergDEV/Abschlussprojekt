using DTSAG.Common.RestClient;

namespace Zeiterfassung
{
    /// <summary>
    /// Implementiert ISageClientConfig zur Bereitstellung der Konfigurationsdetails,
    /// die der SageRestClient für die Verbindung mit der SData API benötigt.
    /// </summary>
    public class SageClientConfig : ISageClientConfig
    {
        public string GetBaseURL()
        {
            return "https://WIN-SWE:5493";
        }

        /// <summary>
        /// Liefert das Sage-Daten-Set oder die Datenbank-Kennung.
        /// Dies ist der Teil des SData-Pfades, der die Mandanten- und Datenbankauswahl regelt.
        /// </summary>
        /// <returns>Ein Task, der den Daten-Set-String enthält.</returns>
        public Task<string> GetDataSetAsync()
        {
            string dataSet = "OlDemoReweAbfD;123";

            return Task.FromResult(dataSet);
        }

        private const string ApiUsername = "Sage";
        private const string ApiPassword = "";

        /// <summary>
        /// Erzeugt den Base64-kodierten String für die Authentifizierung (Basic Auth).
        /// </summary>
        /// <returns>Ein Task, der den Base64-kodierten "Benutzername:Passwort"-String enthält.</returns>
        public Task<string> GetTokenAsync()
        {
            var authString = $"{ApiUsername}:{ApiPassword}";

            // Kodierung in Base64
            var encodedAuth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authString));

            // Der SageRestClient verwendet dies dann im Header:
            // Basic {encodedAuth} 

            return Task.FromResult(encodedAuth);
        }
        /// <summary>
        /// Gibt an, ob es sich um eine lokale API-Instanz handelt.
        /// Diese Information kann intern vom SageRestClient für spezifische Protokollanpassungen genutzt werden.
        /// </summary>
        /// <returns>Immer true, basierend auf der Implementierung.</returns>
        public bool IsLocalAPI()
        {
            return true;
        }
    }
}
