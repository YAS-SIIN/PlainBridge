
 
using System.ComponentModel.DataAnnotations; 

namespace PlainBridge.Api.Application.Enums;

public enum ResultCodeEnum : int
{
    [Display(Name = "Data not found.")]
    NotFound = 100,
     
    [Display(Name = "Error happened.")]
    Error = 101,  

    [Display(Name = "{0} is repeated.")]
    RepeatedData = 102,
    
    [Display(Name = "{0} is invalid.")]
    InvalidData = 102,
    
    [Display(Name = "{0} is null.")]
    NullData = 103,

    [Display(Name = "This data can't delete.")]
    NotDelete = 107,

    [Display(Name = "Action done successfully.")]
    Success = 0,

}
