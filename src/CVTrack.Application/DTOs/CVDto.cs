namespace CVTrack.Application.DTOs;

public class CVDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FileName { get; set; } = null!;
    public DateTime UploadDate { get; set; }
}