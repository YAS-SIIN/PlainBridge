
 
using System.ComponentModel.DataAnnotations; 

namespace PlainBridge.Api.Application.Enums;

public enum ResultCodeEnum : int
{
    [Display(Name = "Data not found.")]
    NotFound = 100,

    [Display(Name = "Done.")]
    Done = 101,

    [Display(Name = "Data is repeated.")]
    RepeatedData = 102,
    
    [Display(Name = "Data is invalid.")]
    InvalidData = 102,

    [Display(Name = "This data can't delete.")]
    NotDelete = 107,

    [Display(Name = "Done.")]
    Success = 200,

    [Display(Name = "Error happened.")]
    Error = 400,  
}
