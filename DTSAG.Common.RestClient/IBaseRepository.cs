using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DTSAG.Common.RestClient.Model;


namespace DTSAG.Common.RestClient {
    /// <summary>
    /// Basis-Interface für ein Repository.
    /// </summary>
    /// <remarks>
    /// Link zu SData-Query:<br/>
    /// https://sage.github.io/SData-2.0/pages/core/0212/
    /// </remarks>
    /// <typeparam name="T">Typ des Daten-Objekts.</typeparam>
    public interface IBaseRepository<T> where T : SageApiResourceBase<T>, new() {
        /// <summary>
        /// Name der Lösung, in welcher die API definiert ist.
        /// </summary>
        /// <remarks>
        /// Aufbau: &lt;Partner-ID&gt;.&lt;Solution-Name&gt;
        /// </remarks>
        public string Solution { get; }

        /// <summary>
        /// Name der API.
        /// </summary>
        public string Api { get; }

        /// <summary>
        /// Name des auszuführenden Endpoints innerhalb der API.
        /// </summary>
        public string Endpoint { get; }

        /// <summary>
        /// Liefert eine Liste von Objekten.
        /// </summary>
        /// <param name="where">Optional: Filter für die Liste (als SData-Query).</param>
        public Task<IEnumerable<T>> GetListAsync(string? where = null);

        /// <summary>
        /// Liefert einen Teil der gewünschten Objekte für eine Seiten-Basierte Abfrage.
        /// </summary>
        /// <param name="page">Seite (&gt;=1).</param>
        /// <param name="take">Anzahl der Objekte je Seite.</param>
        /// <param name="where">Optional: Filter für die Liste (als SData-Query).</param>
        public Task<IEnumerable<T>> GetPagedListAsync(int page, int take, string? where = null);

        /// <summary>
        /// Liefert ein Objekt (anhand der Abfrage). Wird kein Objekt gefunden, wird NULL geliefert.
        /// </summary>
        /// <param name="where">Optional: Filter für die Liste (als SData-Query).</param>
        public Task<T?> GetItemAsync(string? where = null);

        /// <summary>
        /// Liefert ein Objekt anhand des Schlüssels. Wird das Objekt nicht gefunden, wird ein Fehler ausgelöst.
        /// </summary>
        /// <param name="key">Schlüssel des Objekts (<see cref="SageApiResourceBase{TImplementingType}.SageKey"/>).</param>
        public Task<T> GetItemByKeyAsync(string key);

        /// <summary>
        /// Liefert ein Template-Objekt (mit standardmäßig initialisierten Werten).
        /// </summary>
        public Task<T> GetTemplateAsync();

        /// <summary>
        /// Fügt das angegebene Objekt hinzu (HTTP-POST) und gibt es zurück.
        /// </summary>
        /// <param name="item">Hinzuzufügendes Objekt.</param>
        public Task<T> AddItemAsync(T item);

        /// <summary>
        /// Aktualisiert das angegebene Objekt (HTTP-PUT) und gibt es zurück.
        /// </summary>
        /// <param name="item">Zu aktualisierendes Objekt.</param>
        public Task<T> UpdateItemAsync(T item);

        /// <summary>
        /// Löscht das angegebene Objekt (HTTP-DELETE).
        /// </summary>
        /// <param name="item">Zu löschendes Objekt.</param>
        public Task DeleteItemAsync(T item);
    }
}