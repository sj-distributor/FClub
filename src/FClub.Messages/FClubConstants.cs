namespace FClub.Messages;

public static class FClubConstants
{
    public const string Picture = "picture";
    public const string ThirdPartyId = "thirdpartyid";
    public const string ThirdPartyFrom = "thirdpartyfrom";
    
    public static readonly string[] CorrelationIdHeaders = { "CorrelationId", "X-Correlation-ID", "x-correlation-id" };
}