namespace Web.Models;

public class PaginationModel
{
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public string? BaseUrl { get; set; }
    public string PageParam { get; set; } = "page";

    public static PaginationModel Create(int totalItems, int pageSize, int currentPage, string baseUrl, string pageParam = "page")
    {
        return new PaginationModel
        {
            CurrentPage = currentPage,
            TotalPages  = (int)Math.Ceiling((double)totalItems / pageSize),
            BaseUrl     = baseUrl,
            PageParam   = pageParam
        };
    }
}
