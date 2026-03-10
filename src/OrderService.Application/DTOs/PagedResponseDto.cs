namespace OrderService.Application.DTOs;

public class PagedResponseDto<T>
{
    public List<T> Items { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public PagedResponseDto(List<T> ıtems, int pageNumber, int count, int pageSize)
    {
        Items = ıtems;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = count;
    }
}