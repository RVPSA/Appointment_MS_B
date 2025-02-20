namespace User_service.Common;

public static class AppSettings
{
    public static string? JwtSecretKey;
    public static double JwtTokenExpiration;
    
    public static double CookieExpires;
    public static string? CookieDomain;
    public static string? CookiePath;
    public static string? CookieName;
    
    public static string? GatewaySecretKey;

    public static string? TimeStampHeaderKey;
    public static string? SignatureHeaderKey;
    public static string? UserIdKey;
    public static string? UserRoleKey;

    public static string? ClaimsUserId;
    public static string? ClaimUserRole;
    public static string? ClaimUserName;

    public static string? ConnectionString;
}