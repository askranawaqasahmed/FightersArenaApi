using Dapper;
using Ideageek.FightersArena.Core.Dtos;
using Ideageek.FightersArena.Core.Handlers;
using System.Data;
using System.Text;

namespace Ideageek.FightersArena.Core.Helpers
{
    public static class QueryHelper
    {
        public static DynamicParameters BuildWhereClause<T>(PaginationDto request)
        {
            var parameters = new DynamicParameters();
            var whereBuilder = new StringBuilder("1=1");

            var allowedColumns = GetAllowedFilterColumns<T>();

            foreach (var filter in request.Filters)
            {
                if (filter != null && !string.IsNullOrWhiteSpace(filter.Column) && !string.IsNullOrWhiteSpace(filter.Value)
                    && allowedColumns.Contains(filter.Column))
                {
                    var safeColumn = filter.Column.Trim();
                    var safeValue = filter.Value.Trim().Replace("'", "''"); // Escape single quotes

                    if (filter.FilterType.Equals("LIKE", StringComparison.OrdinalIgnoreCase))
                    {
                        whereBuilder.Append($" AND {safeColumn} LIKE '%{safeValue}%'");
                    }
                    else
                    {
                        whereBuilder.Append($" AND {safeColumn} = '{safeValue}'");
                    }
                }
            }

            if (request.PageNumber == 0)
                request.PageNumber = 1;
            if (request.PageSize == 0)
                request.PageSize = 10;

            parameters.Add("@Filters", whereBuilder.ToString(), DbType.String);
            parameters.Add("@SortColumn", request.SortColumn, DbType.String);
            parameters.Add("@SortOrder", request.SortOrder, DbType.String);
            parameters.Add("@PageNumber", request.PageNumber, DbType.Int32);
            parameters.Add("@PageSize", request.PageSize, DbType.Int32);

            return parameters;
        }

        public static List<string> GetAllowedFilterColumns<T>()
        {
            return typeof(T)
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(FilterableAttribute)))
                .Select(p => p.Name)
                .ToList();
        }

        public static DynamicParameters ParameterBuilder<T>(T dto)
        {
            var parameters = new DynamicParameters();
            if (dto == null) return parameters;

            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                // Always include CreatedBy, even if marked with ParameterIgnore
                if (prop.Name != "CreatedBy")
                {
                    var hasParameterIgnore = Attribute.IsDefined(prop, typeof(ParameterIgnoreAttribute));
                    if (hasParameterIgnore)
                        continue;
                }

                var value = prop.GetValue(dto);
                parameters.Add("@" + prop.Name, value);
            }

            return parameters;
        }
    }
}
