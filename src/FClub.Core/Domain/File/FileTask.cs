using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FClub.Messages.Enums;

namespace FClub.Core.Domain.File;

[Table("file_task")]
public class FileTask : IEntity, IHasCreatedFields
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("status")]
    public FileTaskStatus Status { get; set; }
    
    [Column("created_date")]
    public DateTimeOffset CreatedDate { get; set; }
}