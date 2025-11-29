// MailCacheService.cs

using MimeKit;

/// <summary>
/// Dient als temporärer Cache, um MimeMessages zwischen Index- und Detailansicht zu teilen
/// </summary>
public class MailCacheService
{
  
    private MimeMessage? _cachedMessage;


    private int _cachedIndex;

    /// <summary>
    /// Speichert eine Nachricht im Cache.
    /// </summary>
    /// <param name="index">Der eindeutige IMAP-Index der Nachricht.</param>
    /// <param name="message">Die vollständige MimeMessage.</param>
    public void CacheMessage(int index, MimeMessage message)
    {
        _cachedIndex = index;
        _cachedMessage = message;
    }

    /// <summary>
    /// Ruft eine Nachricht aus dem Cache ab, falls der Index übereinstimmt.
    /// </summary>
    /// <param name="index">Der angeforderte IMAP-Index.</param>
    /// <returns>Die MimeMessage oder null, wenn nicht gecacht oder Index falsch.</returns>
    public MimeMessage? GetCachedMessage(int index)
    {
        // Cache Hit: Index stimmt überein
        if (_cachedIndex == index)
        {
            return _cachedMessage;
        }

        // Cache Miss
        return null;
    }
}