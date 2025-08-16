
using System.Text.Json.Serialization;

namespace PlainBridge.SharedApplication.DTOs;
public record UserProfileViewDto
{
    [JsonPropertyName("preferred_username")]
    public string Username { get; set; }
    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("given_name")]
    public string Name { get; set; }
    [JsonPropertyName("family_name")]
    public string Family { get; set; }
}
