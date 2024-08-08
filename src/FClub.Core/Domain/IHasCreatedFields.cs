namespace FClub.Core.Domain;

public interface IHasCreatedFields
{
    DateTimeOffset CreatedDate { get; set; }
}