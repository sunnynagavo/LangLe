namespace LangLe.Shared.DTOs;

public record RegisterRequest(string Email, string Password, string DisplayName);
public record LoginRequest(string Email, string Password);
public record AuthResponse(bool Success, string? Token, string? Error);
