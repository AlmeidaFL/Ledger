using System.ComponentModel.DataAnnotations;
using SimpleAuth.Api.Services;

namespace SimpleAuth.Api.Dtos;

public record GoogleLoginRequest([Required] string IdToken, [Required] UserAgentInfo UserAgentInfo);