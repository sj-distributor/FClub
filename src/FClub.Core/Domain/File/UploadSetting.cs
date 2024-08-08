using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FClub.Messages.Enums;

namespace FClub.Core.Domain.File;

[Table("upload_setting")]
public class UploadSetting : IEntity, IHasCreatedFields
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("file_path")]
    public string FilePath { get; set; }

    [Column("upload_address_type")]
    public UploadType UploadAddressType { get; set; }

    [Column("created_date")]
    public DateTimeOffset CreatedDate { get; set; }
}