namespace Application.DTOs;

public class ProductQueryParams
{
    public string? Category { get; set; }
    public string? Search { get; set; }

    public string? SortBy { get; set; } = "price";
    public bool Desc { get; set; } = false;

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
