namespace Sagi.Sdk.AWS.DynamoDb.Pages;

public class PageResult<TResult> where TResult : class
{
    public IEnumerable<TResult> Items { get; set; } = [];
    public string? PageToken { get; set; }
    public bool HasNextPage => !string.IsNullOrEmpty(PageToken);
}