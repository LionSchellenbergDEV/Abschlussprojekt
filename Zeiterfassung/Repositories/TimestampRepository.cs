using DTSAG.Common.RestClient;
using Zeiterfassung.Models;

namespace Zeiterfassung.Repositories
{
    /// <summary>
    /// Repository-Klasse für den Zugriff auf Zeiterfassungsdaten (<see cref="Timestamp"/>) in der Sage 100 API.
    /// </summary>
    public class TimestampRepository : BaseRepository<Timestamp>
    {
        public TimestampRepository(SageRestClient restClient) : base(restClient)
        {
        }

        public override string Solution { get; } = "100000014.DTSAG_Ticketverwaltung";

        public override string Api { get; } = "apiSageTickets";

        public override string Endpoint { get; } = "eptUploadTest";
    }
}

