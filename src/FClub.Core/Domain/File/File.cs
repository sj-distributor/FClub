using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FClub.Messages.Enums;

namespace FClub.Core.Domain.File;

[Table("file")]
public class File : IEntity, IHasCreatedFields
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("url")]
    public string? Url { get; set; }

    [Column("task_id")]
    public Guid? TaskId { get; set; }

    [Column("type")]
    public FileType Type { get; set; }

    [Column("upload_setting_id")]
    public int UploadSettingId { get; set; }

    [Column("created_date")]
    public DateTimeOffset CreatedDate { get; set; }
}