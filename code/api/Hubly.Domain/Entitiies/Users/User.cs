using System.ComponentModel.DataAnnotations.Schema;

namespace Hubly.Domain.Entities;

[Table("users", Schema = "dbo")]
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [Column("password_validation")]
    public string PasswordValidation { get; set; } = string.Empty;

    //verificar com o prof
    public PasswordValidationInfo GetValidationInfo() => new PasswordValidationInfo(PasswordValidation);

    [Column("created_at")]
    public long CreatedAt { get; set; }
}