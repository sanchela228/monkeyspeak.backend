using Core.Database.Models;
using RegistrationModel = Core.Database.Models.Registration;
using Context = Core.Database.Context;


namespace Core.Services;

public class Registration(Context db)
{
    public RegistrationModel AddRegistration()
    {
        var registration = new RegistrationModel()
        {
            
        };
        
        return db.Registrations.Add();
    }
}