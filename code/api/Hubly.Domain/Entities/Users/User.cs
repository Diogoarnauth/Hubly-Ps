using System.ComponentModel.DataAnnotations.Schema;

namespace Hubly.api.Domain.Entities;

[Table("users", Schema = "dbo")]
public class User
{
    [Column("id")]
    public int Id { get; set; }    
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("password_validation")]
    public string PasswordValidation { get; set; } = string.Empty;

    //verificar com o prof
    public PasswordValidationInfo GetValidationInfo() => new PasswordValidationInfo(PasswordValidation);

    [Column("created_at")]
    public long CreatedAt { get; set; }

    public virtual Creator? Creator { get; set; }
    public virtual Company? Company { get; set; }
}