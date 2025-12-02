using System.ComponentModel.DataAnnotations;

namespace SimpleAuth.Api.Dtos;

public record LogoutRequest([Required] string RefreshToken);