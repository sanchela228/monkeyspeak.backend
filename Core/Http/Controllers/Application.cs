using Core.Database;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Core.Http.Controllers;

public class Application(Context db) : ControllerBase
{
    private readonly Context _db = db;
    
    [HttpPost]
    [Route("/api/applications/register")]
    public async Task CreateApplication([FromBody] string id)
    {
        await _db.Applications.AddAsync(new Core.Database.Models.Application()
        {
            ApplicationId = id
        });

        await _db.SaveChangesAsync();
    }
}