using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Enumerations.Enums;

public enum ClaimsEnum
{
    [Description("USER-create")]
    [Display(Description = "Create User")]
    CreateUser = 1,
    [Description("USER-update")]
    [Display(Description = "Update User")]
    UpdateUser = 2,
    [Description("USER-delete")]
    [Display(Description = "Delete User")]
    DeleteUser = 3,
    [Description("USER-read")]
    [Display(Description = "Read User")]
    ReadUser = 4,
    
    [Description("ROLE-create")]
    [Display(Description = "Create Role")]
    CreateRole = 5,
    [Description("ROLE-update")]
    [Display(Description = "Update Role")]
    UpdateRole = 6,
    [Description("ROLE-delete")]
    [Display(Description = "Delete Role")]
    DeleteRole = 7,
    [Description("ROLE-read")]
    [Display(Description = "Read Role")]
    ReadRole = 8,
    
    [Description("CLAIM-read")]
    [Display(Description = "Read Claim")]
    ReadClaim = 9,
}