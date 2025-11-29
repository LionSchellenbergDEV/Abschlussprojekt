using System.Reflection.Emit;
using System.Resources;
using DTSAG.Common.RestClient;
using DTSAG.Common.RestClient.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Zeiterfassung.Components.Models;

namespace Zeiterfassung.Models
{
    /// <summary>
    /// Repräsentiert ein Ticket-Objekt aus der Sage 100 API (SData), das für die Zeiterfassung relevant ist.
    /// Erbt von SageApiResourceBase, um die SData-Metadatenbehandlung zu ermöglichen.
    /// </summary>
    public class SageTicket: SageApiResourceBase<SageTicket>
    {
        public int ProjektPosID { get; set; }
        public int ProjektID { get; set; }

        public string Ansprechpartner { get; set; }

        public string Betreff { get; set; }
        public DateTime Erfassungsdatum { get; set; }
        public string Mandant {  get; set; }
        public string Positionstyp { get; set; }
        public string Bearbeiter {  get; set; }
        public string Ausfuehrer { get; set; }
        public int Status { get; set; }



    }
}
