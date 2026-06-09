
 
using System.ComponentModel.DataAnnotations; 

namespace PlainBridge.SharedApplication.Enums;

public enum ResultCodeEnum : int
{
    [Display(Name = "Data not found.")]
    NotFound = 100,
     
    [Display(Name = "Error happened.")]
    Error = 101,  

    [Display(Name = "{0} is duplicated.")]
    DuplicatedData = 102,
    
    [Display(Name = "{0} is invalid.")]
    InvalidData = 103,
    
    [Display(Name = "{0} is null.")]
    NullData = 104,

    [Display(Name = "This data can't delete.")]
    NotDelete = 105,

    [Display(Name = "Action done successfully.")]
    Success = 0,

}
