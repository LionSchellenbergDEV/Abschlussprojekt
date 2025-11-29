using System.Threading.Tasks;

namespace DTSAG.Common.RestClient {
    public interface ISageClientConfig {
        string GetBaseURL();

        bool IsLocalAPI();

        Task<string> GetTokenAsync();

        Task<string> GetDataSetAsync();
    }
}