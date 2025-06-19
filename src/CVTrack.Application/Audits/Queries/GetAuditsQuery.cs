namespace CVTrack.Application.Audits.Queries;

public class GetAuditsQuery
{
    public Guid? CvId { get; set; }   // Opsiyonel filtre
    public Guid? UserId { get; set; }  // Opsiyonel filtre
}

