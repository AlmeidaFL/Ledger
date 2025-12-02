using System.ComponentModel.DataAnnotations;
using SimpleAuth.Api.Services;

namespace SimpleAuth.Api.Dtos;

public record RefreshRequest([Required] string RefreshToken, [Required] UserAgentInfo UserAgentInfo);