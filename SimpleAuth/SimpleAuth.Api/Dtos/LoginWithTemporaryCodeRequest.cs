using System.ComponentModel.DataAnnotations;
using SimpleAuth.Api.Services;

namespace SimpleAuth.Api.Dtos;

public class LoginWithTemporaryCodeRequest
{ 
        [Required]
        public string Email { get; set; } = "";
        [Required]
        public string Code { get; set; } = "";
        [Required]
        public UserAgentInfo UserAgentInfo { get; set; }
}