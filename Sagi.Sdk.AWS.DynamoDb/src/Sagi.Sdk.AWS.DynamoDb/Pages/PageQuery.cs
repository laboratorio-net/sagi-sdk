using Sagi.Sdk.Results;

namespace Sagi.Sdk.AWS.DynamoDb.Pages;

public class PageQuery
{
    public const int MAX_PAGE_SIZE = 100;
    public const int MIN_PAGE_SIZE = 1;

    public int PageSize { get; set; } = MIN_PAGE_SIZE;
    public string? PageToken { get; set; }

    public bool IsValid => PageSize >= 1 && PageSize <= MAX_PAGE_SIZE;
    public bool IsInvalid => !IsValid;
    public static Error InvalidPageSize => new($"INVALID_PAGE_SIZE",
        $"The PageSize field must have a value between {MIN_PAGE_SIZE} and {MAX_PAGE_SIZE}.");
}


