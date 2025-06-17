namespace CVTrack.Application.DTOs;

public class AdminCvDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFirstName { get; set; } = null!;
    public string UserLastName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;

    public string FileName { get; set; } = null!;
    public DateTime UploadDate { get; set; }
}
