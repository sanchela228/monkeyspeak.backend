using Context = Core.Database.Context;
using SessionModel = Core.Database.Models.Session;
using SessionStatus = Core.Database.Models.SessionStatus;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public class Session
{
    private readonly Context _db;
    
    public Session(Context db) => _db = db;
    
    public async Task<SessionModel> CreateSession( string ApplicationId, string? meta = null )
    {
        if (_db.Applications.Find(ApplicationId) == null) 
            return null;
        
        var session = new SessionModel()
        {
            ApplicationId = ApplicationId,
        };
            
        _db.Sessions.Add(session);
        await _db.SaveChangesAsync();
        return session;
    }
    
    public async Task<SessionModel?> GetSessionByCode(string code)
    {
        return await _db.Sessions
            .FirstOrDefaultAsync(s => s.Code == code && s.Status == SessionStatus.Active);
    }
    
    public async Task CloseSession(Guid webSocketSessionId)
    {
        var session = await _db.Sessions
            .FirstOrDefaultAsync(s => s.WebSocketSessionId == webSocketSessionId);
            
        if (session != null)
        {
            session.Status = SessionStatus.Closed;
            session.ClosedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }
    
    public async Task CleanupExpiredSessions(TimeSpan expirationTime)
    {
        var expiredSessions = await _db.Sessions
            .Where(s => s.Status == SessionStatus.Active && 
                        s.CreatedAt < DateTime.UtcNow.Subtract(expirationTime))
            .ToListAsync();
            
        foreach (var session in expiredSessions)
        {
            session.Status = SessionStatus.Expired;
            session.ClosedAt = DateTime.UtcNow;
        }
        
        await _db.SaveChangesAsync();
    }
}