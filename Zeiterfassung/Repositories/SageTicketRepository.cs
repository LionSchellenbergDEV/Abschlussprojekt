using DTSAG.Common.RestClient;
using Zeiterfassung.Models;

namespace Zeiterfassung.Repositories
{
    /// <summary>
    /// Repository-Klasse für den Zugriff auf Ticket-Daten in der Sage 100 API.
    /// </summary>
    public class SageTicketRepository : BaseRepository<SageTicket>
    {
        public SageTicketRepository(SageRestClient restClient) : base(restClient)
        {
        }

        public override string Solution { get; } = "100000014.DTSAG_Ticketverwaltung";

        public override string Api { get; } = "apiSageTickets";

        public override string Endpoint { get; } = "eptSageTicket";
    }
}

