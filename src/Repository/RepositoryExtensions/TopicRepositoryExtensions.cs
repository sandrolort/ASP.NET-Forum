using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using Common.Enums;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository.RepositoryExtensions;

public static class TopicRepositoryExtensions
{
    public static IQueryable<Topic> FilterConfirmed(this IQueryable<Topic> query) => query.Where(t => t.State == State.Show);

    public static IQueryable<Topic> Search(this IQueryable<Topic> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

        return query.Where(t => t.Title.ToLower().Contains(lowerCaseSearchTerm));
    }
    
    public static IQueryable<Topic> Sort(this IQueryable<Topic> topics, string? orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
            return topics.OrderByDescending(e => e.Id);
        var orderParams = orderByQueryString.Trim().Split(',');
        var propertyInfos = typeof(Topic).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        var orderQueryBuilder = new StringBuilder();
        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
                continue;
            var propertyFromQueryName = param.Trim().Split(" ")[0];
            var objectProperty = propertyInfos.FirstOrDefault(pi =>
                pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
            if (objectProperty == null)
                continue;
            var direction = param.EndsWith(" desc") ? "descending" : "ascending";
            orderQueryBuilder.Append($"{objectProperty.Name} {direction},");
        }
        var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
        return string.IsNullOrWhiteSpace(orderQuery) ? 
                topics.OrderBy(e => e.Id) : 
                topics.OrderBy(orderQuery);
    }
    
    public static IQueryable<Topic> FilterByAge(this IQueryable<Topic> query, int archiveAfterDays) =>
        query.Include(t => t.Comments)
            .Where(t => t.Status == Status.Active)
            .Where(t => t.Comments == null && t.ModifiedDate < DateTime.Now.AddDays(archiveAfterDays) 
                        ||
                        t.Comments!=null && 
                        t.Comments.Any() && 
                        t.Comments.MaxBy(c => c.CreationDate)!
                            .CreationDate.AddDays(archiveAfterDays) < DateTime.Now);
}