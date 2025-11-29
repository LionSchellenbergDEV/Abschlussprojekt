using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTSAG.Common.RestClient.Model;

namespace DTSAG.Common.RestClient {
    /// <summary>
    /// Basis-Implementierung eines Repositorys.
    /// </summary>
    /// <remarks>
    /// Zur Verwendung müssen lediglich die Properties <see cref="Solution"/>, <see cref="Api"/> und
    /// <see cref="Endpoint"/> definiert werden.
    /// </remarks>
    /// <typeparam name="T">Typ des Daten-Objekts.</typeparam>
    public abstract class BaseRepository<T> : IBaseRepository<T> where T : SageApiResourceBase<T>, new() {
        public abstract string Solution { get; }

        public abstract string Api { get; }

        public abstract string Endpoint { get; }

        /// <summary>
        /// Verweis auf den zu verwendenden Rest-Client.
        /// </summary>
        protected SageRestClient RestClient { get; }

        /// <summary>
        /// Liefert die <see cref="Api"/> mit der <see cref="Solution"/> verknüpft zur Verwendung in einem Rest-Call.
        /// </summary>
        protected virtual string ApiWithSolution => $"{this.Api}.{this.Solution}";

        /// <summary>
        /// Liefert den <see cref="Endpoint"/> mit der <see cref="Solution"/> verknüpft zur Verwendung in einem Rest-Call.
        /// </summary>
        protected virtual string EndpointWithSolution => $"{this.Endpoint}.{this.Solution}";

        /// <summary>
        /// Initialisiert das Repository.
        /// </summary>
        /// <param name="restClient">Zu verwendender Rest-Client.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BaseRepository(SageRestClient restClient) {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            this.RestClient = restClient;
        }

        public virtual async Task<IEnumerable<T>> GetListAsync(string? where = null) {
            var result = await this.RestClient.GetListAsync<T>(this.ApiWithSolution, this.EndpointWithSolution, where);
            return result;
        }

        public virtual async Task<IEnumerable<T>> GetPagedListAsync(int page, int take, string? where = null) {
            if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));
            if (take < 1) throw new ArgumentOutOfRangeException(nameof(take));
            var startIndex = (((page - 1) * take) + 1);
            var result = await this.RestClient.GetListAsync<T>(this.ApiWithSolution, this.EndpointWithSolution, where, startIndex, take);
            return result;
        }

        public virtual async Task<T?> GetItemAsync(string? where = null) {
            var results = await this.GetListAsync(where);
            var result = results.FirstOrDefault();
            return result;
        }

        public virtual async Task<T> GetItemByKeyAsync(string key) {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            var result = await this.RestClient.GetAsync<T>(this.ApiWithSolution, this.EndpointWithSolution, key);
            return result;
        }

        public virtual async Task<T> GetTemplateAsync() {
            var result = await this.RestClient.GetTemplateAsync<T>(this.ApiWithSolution, this.EndpointWithSolution);
            return result;
        }

        public virtual async Task<T> AddItemAsync(T item) {
            var result = await this.RestClient.AddAsync(this.ApiWithSolution, this.EndpointWithSolution, item);
            return result;
        }

        public virtual async Task<T> UpdateItemAsync(T item) {
            var result = await this.RestClient.UpdateFullAsync(this.ApiWithSolution, this.EndpointWithSolution, item);
            return result;
        }

        public virtual async Task DeleteItemAsync(T item) {
            await this.RestClient.DeleteAsync(this.ApiWithSolution, this.EndpointWithSolution, item.SageKey);
        }
    }
}