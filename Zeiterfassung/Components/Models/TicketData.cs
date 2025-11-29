namespace Zeiterfassung.Components.Models
{
    public class ResourceEntry
    {
        public string Ressource { get; set; }
        public string Vorgang { get; set; }
        public string Bereich { get; set; }
        public string DatumVon { get; set; }
        public string DatumBis { get; set; }
        public decimal Stunden { get; set; }
    }

    // Die Klasse für die Gesamtstruktur des Tickets
    public class Ticket
    {
        public string Mandant { get; set; }
        public int ProjektPosID { get; set; }
        public int ProjektID { get; set; }
        public DateTime Erfassungsdatum { get; set; }
        public string Positionstyp { get; set; }
        public string Bearbeiter { get; set; }
        public string Ausfuehrer { get; set; }
        public string Betreff { get; set; }
        public string Ansprechpartner { get; set; }
        public int Status { get; set; }

        public List<ResourceEntry> Resources { get; set; } = new List<ResourceEntry>();
    }

}
