namespace CVTrack.Domain.Entities;

public class CV
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FileName { get; set; } = null!;
    public DateTime UploadDate { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}