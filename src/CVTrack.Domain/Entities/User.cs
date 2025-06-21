namespace CVTrack.Domain.Entities;
public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public UserRole Role { get; set; } = UserRole.User;

    //Navigation properties
    public ICollection<CV> CVs { get; set; } = new List<CV>();
    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}