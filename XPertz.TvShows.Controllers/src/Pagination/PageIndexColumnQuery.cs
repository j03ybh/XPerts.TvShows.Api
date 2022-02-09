using TechMinimalists.Database.Core;

namespace XPertz.TvShows.Controllers
{
    public class PageIndexColumnQuery
    {
        public PageIndexColumnQuery(string columnName, long startIndex, long endIndex)
        {
            Queries = new ColumnQuery[]
            {
                new ColumnQuery(columnName, startIndex, QueryCondition.GreaterThanOrEqualTo),
                new ColumnQuery(columnName, endIndex, QueryCondition.LessThanOrEqualTo)
            };
        }

        public static implicit operator ColumnQuery[](PageIndexColumnQuery pageIndexColumnQuery)
        {
            return pageIndexColumnQuery.Queries;
        }

        public ColumnQuery[] Queries { get; }
    }
}