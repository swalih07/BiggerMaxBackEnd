namespace Application.DTOs;

public class ProductQueryParams
{
    public  int? CategoryId { get; set; }
    public string Category { get; set; }= string.Empty;
    public string Search { get; set; }=string.Empty;

    public string? SortBy { get; set; } = "price";
    public bool Desc { get; set; } = false;

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
