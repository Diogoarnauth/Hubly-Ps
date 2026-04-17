namespace Hubly.api.DTOs;
public class FullUserProfileOutputModel
{
    public int Id { get; set; }
    public string Name { get; set; }        
    public string Email {get; set;}
    public bool IsOwner { get; set; } 
    public GetCreatorOutputModel? Creator { get; set; }
}
