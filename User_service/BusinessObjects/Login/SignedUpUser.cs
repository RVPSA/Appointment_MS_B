namespace User_service.BusinessObjects.Login;

public class SignedUpUser
{
    public required string LastName { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string ContactNumber { get; set; }
    public required string NicNumber { get; set; }
    public required string Role { get; set; }
    public required string PatientNumber { get; set; }
}