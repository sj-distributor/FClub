using System;
using System.Collections.Generic;
using FClub.Messages.Enums.Account;

namespace FClub.Messages.Dto.Users;

public class UserAccountDto
{
    public int Id { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime ModifiedOn { get; set; }
    
    public Guid Uuid { get; set; }
    
    public string UserName { get; set; }
    
    public bool IsActive { get; set; }
    
    public string ThirdPartyUserId { get; set; }
    
    public UserAccountIssuer Issuer { get; set; }
}