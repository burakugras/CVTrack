using CVTrack.Domain.Entities;

namespace CVTrack.Application.CVs.Queries;

public class GetAllCvsQuery
{
    public Guid? UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }

}