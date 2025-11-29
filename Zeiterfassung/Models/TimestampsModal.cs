namespace Zeiterfassung.Models
{
    /// <summary>
    /// Repräsentiert ein Datenmodell für die Anzeige von Zeiteinträgen (Timestamps) in der Benutzeroberfläche.
    /// </summary>
    public class TimestampsModal
    {
        public int PosID { get; set; }
        public int ProjektPosID { get; set; }
        public int ProjektID { get; set; }
        public DateTime Datum { get; set; }
        public double Stunden { get; set; }
        public string Vorgang { get; set; }
        public string Bearbeiter { get; set; }
        public string Ausfuehrer { get; set; }
        public string Status { get; set; }

    }
}
