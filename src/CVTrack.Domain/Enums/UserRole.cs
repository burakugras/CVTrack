using System.Runtime.Serialization;

namespace CVTrack.Domain.Entities;

public enum UserRole
{
    [EnumMember(Value = "User")]
    User,   // Standart kullanıcı
    [EnumMember(Value = "Admin")]
    Admin   // Yönetici
}
