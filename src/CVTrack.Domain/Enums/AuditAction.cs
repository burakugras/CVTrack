using System.Runtime.Serialization;

namespace CVTrack.Domain.Entities;

public enum AuditAction
{
    [EnumMember(Value = "Downloaded")]
    Downloaded
    // ileride Uploaded, Deleted, vs. eklenebilir
}
