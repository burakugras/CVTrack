using System.Runtime.Serialization;

namespace CVTrack.Domain.Entities;

public enum ApplicationStatus
{
    [EnumMember(Value = "Beklemede")]
    Pending,
    [EnumMember(Value = "Kabul edildi")]
    Accepted,
    [EnumMember(Value = "Reddedildi")]
    Rejected
}
