namespace CVTrack.Application.CVs.Commands;

public class CreateCvCommand
{
    public Guid UserId { get; set; }
    public string FileName { get; set; } = null!;
    // ileride byte[] Content ekleyip Infrastructure ile dosya kaydetme yapacağız
}