namespace LangLe.Shared.DTOs;

public record WordBankEntryDto(
    int Id,
    string English,
    string Spanish,
    string Telugu,
    string? ImageUrl,
    DateTime LearnedAt);
