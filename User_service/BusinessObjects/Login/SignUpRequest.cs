namespace User_service.BusinessObjects.Login;

public class SignUpRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string SureName { get; set; }
    public required string Address { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ContactNumber { get; set; }
    public required DateOnly Dob { get; set; }
    public required string Gender { get; set; }
    public required string NicNumber { get; set; }
}