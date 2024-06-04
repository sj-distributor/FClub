using FClub.Messages.Enums;

namespace FClub.Messages.Dto.File;

public class FileTaskDto
{
    public int Id { get; set; }
    
    public FileTaskStatus Status { get; set; }
    
    public DateTimeOffset CreatedDate { get; set; }
}