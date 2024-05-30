namespace Web.Models;

public record PagedModel <T>
(
    IEnumerable<T> Items,
    int Page,
    int ItemsPerPage,
    string? Search,
    string? OrderBy
) where T : class;