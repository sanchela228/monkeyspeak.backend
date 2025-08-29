namespace Core.Database.Models;

public class Registration
{
    public int ID { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public bool AdditionalData { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}