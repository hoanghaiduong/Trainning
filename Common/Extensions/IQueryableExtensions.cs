

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Trainning.Models;

namespace Trainning.Common.Extensions
{
    public static class IQueryableExtensions
    {
        public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(
            this IQueryable<T> query, PaginationModel paginationModel
        )
        {
            var totalCount = await query.CountAsync();
            var items = await query.Skip((paginationModel.PageNumber - 1) * paginationModel.PageSize).Take(paginationModel.PageSize).ToListAsync();
            return new PaginatedResult<T>(items, totalCount, paginationModel.PageSize, paginationModel.PageNumber);
        }
    }
}