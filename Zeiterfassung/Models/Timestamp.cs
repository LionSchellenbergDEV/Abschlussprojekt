using DTSAG.Common.RestClient.Model;

namespace Zeiterfassung.Models
{
    /// <summary>
    /// Repräsentiert einen einzelnen Zeiteintrag (Timestamp) oder eine Leistung, 
    /// die über die Sage 100 API erfasst wird.
    /// Erbt von SageApiResourceBase zur Kompatibilität mit dem SData REST Client.
    /// </summary>
    public class Timestamp : SageApiResourceBase<Timestamp>
    {
        public int PosID { get; set; }
        public int ProjektPosID { get; set; }
        public int ProjektID { get; set; }
        public DateTime Datum { get; set; } = DateTime.Today;
        public decimal Stunden { get; set; }
        public string Vorgang { get; set; } = string.Empty;
        public string Bearbeiter { get; set; } = string.Empty;
        public string Ausfuehrer { get; set; } = string.Empty;
    }
}
