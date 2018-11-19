using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Service.Entity.Base
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public class BaseEntity
    {
        string _SELECT;
        string _UPDATE;
        string _DELETE;
        string _INSERT;
        string _DATALINK;
        internal string SQL_SELECT
        {
            get
            {
                return _SELECT;
            }
        }
        internal string SQL_UPDATE
        {
            get { return _UPDATE; }
        }
        internal string SQL_DELETE
        {
            get { return _DELETE; }
        }
        internal string SQL_INSERT
        {
            get { return _INSERT; }
        }
        public string DataLink
        {
            get { return _DATALINK; }
        }

        public BaseEntity(string DataLink, string Select, string Insert, string Update, string Delete)
        {
            _SELECT = Select ?? _SELECT;
            _UPDATE = Update ?? _UPDATE;
            _INSERT = Insert ?? _INSERT;
            _DELETE = Delete ?? _DELETE;
            _DATALINK = DataLink ?? _DATALINK;
        }

    }

    /// <summary>
    /// 实体方法扩展  V2.1  2016-10-30 
    /// For .net Core
    /// 增加了SubString方法
    /// </summary>
    public static class ExFunc
    {
        private class ValueParam
        {
            public string Name { get; set; }
            public object Value { get; set; }
            public DbType Type { get; set; }
        }
        private static String NewLine
        {
            get
            {
                return "\r\n";
            }
        }
        private enum MethodType
        {
            None = 0,
            Like = 1,
            In = 2,
            SubString = 3,
        }

        public static bool In<T>(this T obj, T[] array) where T : BaseEntity
        {
            return true;
        }
        public static bool NotIn<T>(this T obj, T[] array) where T : BaseEntity
        {
            return true;
        }
        public static bool Like(this string str, string likeStr)
        {
            return true;
        }
        public static bool NotLike(this string str, string likeStr)
        {
            return true;
        }
        public static bool SubString(this string str, int BeginIndex, int Length, string Value)
        {
            return true;
        }

        public static int Count<T>(this T entity, Expression<Func<T, bool>> func) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_SELECT;
            #region 解析Select字段
            Field = GenricField(StrQuery);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }

            StrQuery = StrQuery.Replace(NewLine, "");
            Regex reg = new Regex(@"^select(.*?)from", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            StrQuery = reg.Replace(StrQuery, "select count(1) from ").ToString().Trim();
            using (var conn = GetConnection(entity.DataLink))
            {
                var Result = conn.Query<int>(StrQuery, GetParameters(param)).FirstOrDefault();
                return Result;
            }
        }

        public static int Count<T>(this T entity, IDbConnection conn, IDbTransaction trans, Expression<Func<T, bool>> func) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_SELECT;
            #region 解析Select字段
            Field = GenricField(StrQuery);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }

            StrQuery = StrQuery.Replace(NewLine, "");
            Regex reg = new Regex(@"^select(.*?)from", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            StrQuery = reg.Replace(StrQuery, "select count(1) from ").ToString().Trim();
            var Result = conn.Query<int>(StrQuery, GetParameters(param), trans).FirstOrDefault();
            return Result;
        }

        public static int Max<T>(this T entity, Expression<Func<T, bool>> func, string FieldName) where T : BaseEntity
        {

            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_SELECT;
            #region 解析Select字段
            Field = GenricField(StrQuery);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }

            StrQuery = StrQuery.Replace(NewLine, "");
            Regex reg = new Regex(@"^select(.*?)from", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            StrQuery = reg.Replace(StrQuery, "select max(" + FieldName + ") from ").ToString().Trim();
            using (var conn = GetConnection(entity.DataLink))
            {
                var Result = conn.ExecuteScalar<int>(StrQuery, GetParameters(param));
                return Result;
            }


        }
        public static IEnumerable<T> Where<T>(this T entity, Expression<Func<T, bool>> func) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_SELECT;
            #region 解析Select字段
            Field = GenricField(StrQuery);
            #endregion

            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }
            using (var conn = GetConnection(entity.DataLink))
            {
                var Result = conn.Query<T>(StrQuery, GetParameters(param));
                return Result;
            }

        }
        public static IEnumerable<T> Where<T>(this T entity, IDbConnection conn, IDbTransaction trans, Expression<Func<T, bool>> func) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_SELECT;
            #region 解析Select字段
            Field = GenricField(StrQuery);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }
            return conn.Query<T>(StrQuery, GetParameters(param), trans);
        }
        public static IEnumerable<T> WhereSP<T>(this T entity, Expression<Func<T, bool>> func) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            string StrQuery = entity.SQL_SELECT;
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    BinarExpressionProvider(be.Left, be.Right, be.NodeType, false, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    BinarExpressionProvider(be, null, be.NodeType, false, param, Field);
                }
            }

            using (var conn = GetConnection(entity.DataLink))
            {
                var Result = conn.Query<T>(StrQuery, GetParameters(param), commandType: CommandType.StoredProcedure);
                return Result;
            }

        }
        public static IEnumerable<T> Where<T>(this T entity, Expression<Func<T, bool>> func, string WhereSql) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_SELECT;
            #region 解析Select字段
            Field = GenricField(StrQuery);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }
            if (!string.IsNullOrWhiteSpace(WhereSql))
                StrQuery += " and (" + WhereSql + ")";
            using (var conn = GetConnection(entity.DataLink))
            {
                var Result = conn.Query<T>(StrQuery, GetParameters(param));
                return Result;
            }

        }
        public static IEnumerable<T> WhereSQL<T>(this T entity, Expression<Func<T, bool>> func, string Sql) where T : BaseEntity
        {

            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = Sql;
            #region 解析Select字段
            Field = GenricField(StrQuery);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = Sql + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = Sql + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }

            using (var conn = GetConnection(entity.DataLink))
            {
                var Result = conn.Query<T>(StrQuery, GetParameters(param));
                return Result;
            }

        }

        public static IEnumerable<M> WhereSP<T, M>(this T entity, Expression<Func<T, bool>> func) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            string StrQuery = entity.SQL_SELECT;
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    BinarExpressionProvider(be.Left, be.Right, be.NodeType, false, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    BinarExpressionProvider(be, null, be.NodeType, false, param, Field);
                }
            }
            using (var conn = GetConnection(entity.DataLink))
            {
                var Result = conn.Query<M>(StrQuery, GetParameters(param), commandType: CommandType.StoredProcedure);
                return Result;
            }

        }
        public static IEnumerable<M> Where<T, M>(this T entity, Expression<Func<T, bool>> func) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            List<FieldAtrribute> OrgField = new List<FieldAtrribute>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_SELECT;
            #region 解析Select字段
            Field = GenricField(StrQuery, ref OrgField);
            string _SQL_SELECT = GenricSelect<M>(StrQuery, OrgField);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }

            using (var conn = GetConnection(entity.DataLink))
            {
                try
                {
                    var Result = conn.Query<M>(StrQuery, GetParameters(param));
                    return Result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

        }

        public static IEnumerable<M> Where<T, M>(this T entity, Expression<Func<T, bool>> func, string WhereSql) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            List<FieldAtrribute> OrgField = new List<FieldAtrribute>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_SELECT;
            #region 解析Select字段
            Field = GenricField(StrQuery, ref OrgField);
            string _SQL_SELECT = GenricSelect<M>(StrQuery, OrgField);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }
            if (!string.IsNullOrWhiteSpace(WhereSql))
                StrQuery += " and (" + WhereSql + ")";
            using (var conn = GetConnection(entity.DataLink))
            {
                var Result = conn.Query<M>(StrQuery, GetParameters(param));
                return Result;
            }

        }
        public static IEnumerable<M> WhereSQL<T, M>(this T entity, Expression<Func<T, bool>> func, string Sql) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            List<FieldAtrribute> OrgField = new List<FieldAtrribute>();
            bool IsIncludeAlias = true;
            string StrQuery = Sql;
            #region 解析Select字段
            Field = GenricField(StrQuery, ref OrgField);
            string _SQL_SELECT = GenricSelect<M>(StrQuery, OrgField);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }

            using (var conn = GetConnection(entity.DataLink))
            {
                var Result = conn.Query<M>(StrQuery, GetParameters(param));
                return Result;
            }

        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="func">查询条件表达式</param>
        /// <param name="KeyColName">分页排序原则</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="PageCount">总页数</param>
        /// <param name="ItemCount">总条目数</param>
        /// <param name="IsCount">是否计算总页数</param>
        /// <returns></returns>
        public static IEnumerable<T> WherePage<T>(this T entity, Expression<Func<T, bool>> func, string KeyColName, int PageIndex, int PageSize, ref int PageCount, ref int ItemCount, bool IsCount) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_SELECT;

            #region 解析Select字段
            Field = GenricField(StrQuery);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }

            }

            using (var conn = GetConnection(entity.DataLink))
            {
                var Param = GetParameters(param);
                StrQuery = CreatePageQuery(conn, StrQuery, KeyColName, PageSize, PageIndex, IsCount, ref ItemCount, ref PageCount, Param);
                var Result = conn.Query<T>(StrQuery, Param);
                return Result;
            }

        }

        public static IEnumerable<T> WherePage<T>(this T entity, Expression<Func<T, bool>> func, string WhereSql, string KeyColName, int PageIndex, int PageSize, ref int PageCount, ref int ItemCount, bool IsCount) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_SELECT;

            #region 解析Select字段
            Field = GenricField(StrQuery);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = entity.SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }
            if (!string.IsNullOrWhiteSpace(WhereSql))
                StrQuery += " and (" + WhereSql + ")";

            using (var conn = GetConnection(entity.DataLink))
            {
                var Param = GetParameters(param);
                StrQuery = CreatePageQuery(conn, StrQuery, KeyColName, PageSize, PageIndex, IsCount, ref ItemCount, ref PageCount, Param);
                var Result = conn.Query<T>(StrQuery, Param);
                return Result;
            }

        }

        public static IEnumerable<T> WherePageSQL<T>(this T entity, Expression<Func<T, bool>> func, string Sql, string KeyColName, int PageIndex, int PageSize, ref int PageCount, ref int ItemCount, bool IsCount) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = Sql;

            #region 解析Select字段
            Field = GenricField(StrQuery);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = StrQuery + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = StrQuery + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }


            using (var conn = GetConnection(entity.DataLink))
            {
                var Param = GetParameters(param);
                StrQuery = CreatePageQuery(conn, StrQuery, KeyColName, PageSize, PageIndex, IsCount, ref ItemCount, ref PageCount, Param);
                var Result = conn.Query<T>(StrQuery, Param);
                return Result;
            }

        }

        public static IEnumerable<M> WherePage<T, M>(this T entity, Expression<Func<T, bool>> func, string KeyColName, int PageIndex, int PageSize, ref int PageCount, ref int ItemCount, bool IsCount) where T : BaseEntity
        {

            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            List<FieldAtrribute> OrgField = new List<FieldAtrribute>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_SELECT;

            #region 解析Select字段
            Field = GenricField(StrQuery, ref OrgField);
            string _SQL_SELECT = GenricSelect<M>(StrQuery, OrgField);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }

            }

            using (var conn = GetConnection(entity.DataLink))
            {
                var Param = GetParameters(param);
                StrQuery = CreatePageQuery(conn, StrQuery, KeyColName, PageSize, PageIndex, IsCount, ref ItemCount, ref PageCount, Param);
                try
                {
                    var Result = conn.Query<M>(StrQuery, Param);
                    return Result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

        }
        public static PageList<M> WherePage<T, M>(this T entity, Expression<Func<T, bool>> func, string KeyColName, int PageIndex, int PageSize, int PageCount, int ItemCount) where T : BaseEntity
        {

            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            List<FieldAtrribute> OrgField = new List<FieldAtrribute>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_SELECT;

            #region 解析Select字段
            Field = GenricField(StrQuery, ref OrgField);
            string _SQL_SELECT = GenricSelect<M>(StrQuery, OrgField);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }

            }
            bool IsCount = PageIndex == 1 ? true : false;
            using (var conn = GetConnection(entity.DataLink))
            {
                var Param = GetParameters(param);
                StrQuery = CreatePageQuery(conn, StrQuery, KeyColName, PageSize, PageIndex, IsCount, ref ItemCount, ref PageCount, Param);
                var Result = conn.Query<M>(StrQuery, Param);
                return new PageList<M>()
                {
                    CurrentPageIndex = PageIndex,
                    PageSize = PageSize,
                    TotalPageCount = PageCount,
                    TotalItemCount = ItemCount,
                    Data = Result
                };
            }

        }
        public static PageList<M> WherePage<T, M>(this T entity, Expression<Func<T, bool>> func, string WhereSql, string KeyColName, int PageIndex, int PageSize, int PageCount, int ItemCount) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            List<FieldAtrribute> OrgField = new List<FieldAtrribute>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_SELECT;

            #region 解析Select字段
            Field = GenricField(StrQuery, ref OrgField);
            string _SQL_SELECT = GenricSelect<M>(StrQuery, OrgField);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }
            if (!string.IsNullOrWhiteSpace(WhereSql))
                StrQuery += " and (" + WhereSql + ")";
            bool IsCount = PageIndex == 1 ? true : false;
            using (var conn = GetConnection(entity.DataLink))
            {
                var Param = GetParameters(param);
                StrQuery = CreatePageQuery(conn, StrQuery, KeyColName, PageSize, PageIndex, IsCount, ref ItemCount, ref PageCount, Param);
                var Result = conn.Query<M>(StrQuery, Param);
                return new PageList<M>()
                {
                    CurrentPageIndex = PageIndex,
                    PageSize = PageSize,
                    TotalPageCount = PageCount,
                    TotalItemCount = ItemCount,
                    Data = Result
                };
            }

        }
        public static IEnumerable<M> WherePage<T, M>(this T entity, Expression<Func<T, bool>> func, string WhereSql, string KeyColName, int PageIndex, int PageSize, ref int PageCount, ref int ItemCount, bool IsCount) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            List<FieldAtrribute> OrgField = new List<FieldAtrribute>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_SELECT;

            #region 解析Select字段
            Field = GenricField(StrQuery, ref OrgField);
            string _SQL_SELECT = GenricSelect<M>(StrQuery, OrgField);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }
            if (!string.IsNullOrWhiteSpace(WhereSql))
                StrQuery += " and (" + WhereSql + ")";

            using (var conn = GetConnection(entity.DataLink))
            {
                try
                {
                    var Param = GetParameters(param);
                    StrQuery = CreatePageQuery(conn, StrQuery, KeyColName, PageSize, PageIndex, IsCount, ref ItemCount, ref PageCount, Param);
                    var Result = conn.Query<M>(StrQuery, Param);
                    return Result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public static IEnumerable<M> WherePageSQL<T, M>(this T entity, Expression<Func<T, bool>> func, string Sql, string KeyColName, int PageIndex, int PageSize, ref int PageCount, ref int ItemCount, bool IsCount) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            List<FieldAtrribute> OrgField = new List<FieldAtrribute>();
            bool IsIncludeAlias = true;
            string StrQuery = Sql;

            #region 解析Select字段
            Field = GenricField(StrQuery, ref OrgField);
            string _SQL_SELECT = GenricSelect<M>(StrQuery, OrgField);
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = _SQL_SELECT + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }


            using (var conn = GetConnection(entity.DataLink))
            {
                var Param = GetParameters(param);
                StrQuery = CreatePageQuery(conn, StrQuery, KeyColName, PageSize, PageIndex, IsCount, ref ItemCount, ref PageCount, Param);
                var Result = conn.Query<M>(StrQuery, Param);
                return Result;
            }

        }


        public static int Delete<T>(this T entity, Expression<Func<T, bool>> func) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_DELETE;
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = entity.SQL_DELETE + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = entity.SQL_DELETE + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }

            using (var conn = GetConnection(entity.DataLink))
            {
                try
                {
                    var Result = conn.Execute(StrQuery, GetParameters(SetParameters(param, entity)));
                    return Result;
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }
        public static int Delete<T>(this T entity, Expression<Func<T, bool>> func, string WhereSql) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_DELETE;
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = entity.SQL_DELETE + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = entity.SQL_DELETE + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }
            if (!string.IsNullOrWhiteSpace(WhereSql))
                StrQuery += " and (" + WhereSql + ")";
            using (var conn = GetConnection(entity.DataLink))
            {
                try
                {
                    var Result = conn.Execute(StrQuery, GetParameters(SetParameters(param, entity)));
                    return Result;
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }
        public static int Delete<T>(this T entity, IDbConnection conn, IDbTransaction trans, Expression<Func<T, bool>> func) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_DELETE;
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = entity.SQL_DELETE + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = entity.SQL_DELETE + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }

            var Result = conn.Execute(StrQuery, GetParameters(SetParameters(param, entity)), trans);
            return Result;
        }

        public static int Update<T>(this T entity, Expression<Func<T, bool>> func) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_UPDATE;
            #region 解析字段
            Regex reg = new Regex(@"(set)(.+)(where)", RegexOptions.IgnoreCase);
            var match = reg.Match(StrQuery).ToString().Trim();
            reg = new Regex(@"set", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");
            reg = new Regex(@"where", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");
            string[] array = match.Split(',');
            foreach (var Item in array)
            {
                string Value = Item.Trim();
                string[] FieldArray = Value.Split('=');
                if (FieldArray[1].Trim().Substring(0, 1).Equals("@"))
                    if (!param.Any(a => a.Name == (FieldArray[1].Trim().ToLower())))
                        param.Add(new ValueParam
                        {
                            Name = FieldArray[1].Trim(),
                            Value = FieldArray[1].Trim(),
                            Type = DbType.String
                        });

            }
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = entity.SQL_UPDATE + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = entity.SQL_UPDATE + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }
            using (var conn = GetConnection(entity.DataLink))
            {
                try
                {
                    var Result = conn.Execute(StrQuery, GetParameters(SetParameters(param, entity)));
                    return Result;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }
        public static int Update<T>(this T entity, Expression<Func<T, bool>> func, string WhereSql) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_UPDATE;
            #region 解析字段
            Regex reg = new Regex(@"(set)(.+)(where)", RegexOptions.IgnoreCase);
            var match = reg.Match(StrQuery).ToString().Trim();
            reg = new Regex(@"set", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");
            reg = new Regex(@"where", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");
            string[] array = match.Split(',');
            foreach (var Item in array)
            {
                string Value = Item.Trim();
                string[] FieldArray = Value.Split('=');
                if (FieldArray[1].Trim().Substring(0, 1).Equals("@"))
                    if (!param.Any(a => a.Name == (FieldArray[1].Trim().ToLower())))
                        param.Add(new ValueParam
                        {
                            Name = FieldArray[1].Trim(),
                            Value = FieldArray[1].Trim(),
                            Type = DbType.String
                        });

            }
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = entity.SQL_UPDATE + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = entity.SQL_UPDATE + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }
            using (var conn = GetConnection(entity.DataLink))
            {
                try
                {
                    StrQuery += WhereSql;
                    var Result = conn.Execute(StrQuery, GetParameters(SetParameters(param, entity)));
                    return Result;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }
        public static int Update<T>(this T entity, IDbConnection conn, IDbTransaction trans, Expression<Func<T, bool>> func) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_UPDATE;
            #region 解析字段
            Regex reg = new Regex(@"(set)(.+)(where)", RegexOptions.IgnoreCase);
            var match = reg.Match(StrQuery).ToString().Trim();
            reg = new Regex(@"set", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");
            reg = new Regex(@"where", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");
            string[] array = match.Split(',');
            foreach (var Item in array)
            {
                string Value = Item.Trim();
                string[] FieldArray = Value.Split('=');
                if (FieldArray[1].Trim().Substring(0, 1).Equals("@"))
                    if (!param.Any(a => a.Name == FieldArray[1].Trim().ToLower()))
                        param.Add(new ValueParam
                        {
                            Name = FieldArray[1].Trim(),
                            Value = FieldArray[1].Trim(),
                            Type = DbType.String
                        });
            }
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = entity.SQL_UPDATE + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = entity.SQL_UPDATE + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }

            try
            {
                var Result = conn.Execute(StrQuery, GetParameters(SetParameters(param, entity)), trans);
                return Result;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        public static int Update<T>(this T entity, Expression<Func<T, bool>> func, IEnumerable<MemberExpression> Member) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_UPDATE;
            #region 解析字段
            var strUpdate = "";
            if (Member.Count() > 0)
            {

                Member.ToList().ForEach(e =>
                {
                    strUpdate += string.Format(",{0}={1}", e.Member.Name, "@" + e.Member.Name);
                    if (!param.Any(a => a.Name == ("@" + e.Member.Name)))
                        param.Add(new ValueParam
                        {
                            Name = "@" + e.Member.Name,
                            Value = "@" + e.Member.Name,
                            Type = DbType.String
                        });
                });
                if (strUpdate.Length > 0)
                    strUpdate = strUpdate.Substring(1);
            }

            Regex reg = new Regex(@"(set)(.+)(where)", RegexOptions.IgnoreCase);
            strUpdate = reg.Replace(StrQuery, @" set " + strUpdate + @" where ");
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = strUpdate + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = strUpdate + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }
            else
            {
                StrQuery = strUpdate;
            }
            using (var conn = GetConnection(entity.DataLink))
            {
                var Result = conn.Execute(StrQuery, GetParameters(SetParameters(param, entity)));
                return Result;
            }
        }

        public static int Update<T>(this T entity, Expression<Func<T, bool>> func, string WhereSql, IEnumerable<MemberExpression> Member) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_UPDATE;
            #region 解析字段
            var strUpdate = "";
            if (Member.Count() > 0)
            {

                Member.ToList().ForEach(e =>
                {
                    strUpdate += string.Format(",{0}={1}", e.Member.Name, "@" + e.Member.Name);
                    if (!param.Any(a => a.Name == ("@" + e.Member.Name)))
                        param.Add(new ValueParam
                        {
                            Name = "@" + e.Member.Name,
                            Value = "@" + e.Member.Name,
                            Type = DbType.String
                        });
                });
                if (strUpdate.Length > 0)
                    strUpdate = strUpdate.Substring(1);
            }

            Regex reg = new Regex(@"(set)(.+)(where)", RegexOptions.IgnoreCase);
            strUpdate = reg.Replace(StrQuery, @" set " + strUpdate + @" where ");
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = strUpdate + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = strUpdate + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }
            else
            {
                StrQuery = strUpdate;
            }
            using (var conn = GetConnection(entity.DataLink))
            {
                if (!string.IsNullOrWhiteSpace(WhereSql))
                    StrQuery += " and " + WhereSql;
                var Result = conn.Execute(StrQuery, GetParameters(SetParameters(param, entity)));
                return Result;
            }
        }
        public static int Update<T>(this T entity, IDbConnection conn, IDbTransaction trans, Expression<Func<T, bool>> func, IEnumerable<MemberExpression> Member) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            IDictionary<string, string> Field = new Dictionary<string, string>();
            bool IsIncludeAlias = true;
            string StrQuery = entity.SQL_UPDATE;
            #region 解析字段
            var strUpdate = "";
            if (Member.Count() > 0)
            {

                Member.ToList().ForEach(e =>
                {
                    strUpdate += string.Format(",{0}={1}", e.Member.Name, "@" + e.Member.Name);
                    if (!param.Any(a => a.Name == ("@" + e.Member.Name)))
                        param.Add(new ValueParam
                        {
                            Name = "@" + e.Member.Name,
                            Value = "@" + e.Member.Name,
                            Type = DbType.String
                        });
                });
                if (strUpdate.Length > 0)
                    strUpdate = strUpdate.Substring(1);
            }

            Regex reg = new Regex(@"(set)(.+)(where)", RegexOptions.IgnoreCase);
            strUpdate = reg.Replace(StrQuery, @" set " + strUpdate + @" where ");
            #endregion
            if (func != null)
            {
                if (func.Body is BinaryExpression)
                {
                    BinaryExpression be = ((BinaryExpression)func.Body);
                    StrQuery = strUpdate + " and " + BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
                }
                else if (func.Body is Expression)
                {
                    Expression be = ((Expression)func.Body);
                    StrQuery = strUpdate + " and " + BinarExpressionProvider(be, null, be.NodeType, IsIncludeAlias, param, Field);
                }
            }
            else
            {
                StrQuery = strUpdate;
            }

            var Result = conn.Execute(StrQuery, GetParameters(SetParameters(param, entity)), trans);
            return Result;

        }


        public static int Insert<T>(this T entity) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            string StrQuery = entity.SQL_INSERT;
            #region 解析Insert字段
            Regex reg = new Regex(@"(Values)(.+)(\))", RegexOptions.IgnoreCase);
            var match = reg.Match(StrQuery).ToString().Trim();


            reg = new Regex(@"(\()(.+)(\))", RegexOptions.IgnoreCase);
            match = reg.Match(match).ToString().Trim();
            reg = new Regex(@"\(", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");

            reg = new Regex(@"\)", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");
            string[] array = match.Split(',');
            foreach (var Item in array)
            {
                string Value = Item.Trim();
                if (!param.Any(a => a.Name == Value))
                    param.Add(new ValueParam
                    {
                        Name = Value,
                        Value = Value,
                        Type = DbType.String
                    });


            }
            #endregion
            StrQuery = entity.SQL_INSERT;


            using (var conn = GetConnection(entity.DataLink))
            {

                try
                {
                    var Result = conn.Execute(StrQuery, GetParameters(SetParameters(param, entity)));
                    return Result;
                }
                catch (Exception ex)
                {
                    return 0;

                }

            }

        }
        public static int InsertNoParam<T>(this T entity) where T : BaseEntity
        {
            string StrQuery = entity.SQL_INSERT;
            using (var conn = GetConnection(entity.DataLink))
            {

                try
                {
                    var Result = conn.Execute(StrQuery);
                    return Result;
                }
                catch (Exception ex)
                {
                    return 0;

                }

            }

        }

        /// <summary>
        /// 插入自增主键表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>返回自增ID的值</returns>
        public static int InsertIdentity<T>(this T entity) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            string StrQuery = entity.SQL_INSERT;
            #region 解析Insert字段
            Regex reg = new Regex(@"(Values)(.+)(\))", RegexOptions.IgnoreCase);
            var match = reg.Match(StrQuery).ToString().Trim();


            reg = new Regex(@"(\()(.+)(\))", RegexOptions.IgnoreCase);
            match = reg.Match(match).ToString().Trim();
            reg = new Regex(@"\(", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");

            reg = new Regex(@"\)", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");
            string[] array = match.Split(',');
            foreach (var Item in array)
            {
                string Value = Item.Trim();
                if (!param.Any(a => a.Name == Value))
                    param.Add(new ValueParam
                    {
                        Name = Value,
                        Value = Value,
                        Type = DbType.String
                    });


            }
            #endregion
            StrQuery = entity.SQL_INSERT;
            using (var conn = GetConnection(entity.DataLink))
            {
                try
                {
                    StrQuery += @" ;SELECT @id=SCOPE_IDENTITY()";
                    var Parm = GetParameters(SetParameters(param, entity));
                    Parm.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    var Result = conn.Execute(StrQuery, Parm);
                    var ID = Parm.Get<int>("@id");
                    return ID;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }
        public static int Insert<T>(this T entity, IDbConnection conn, IDbTransaction trans) where T : BaseEntity
        {
            IList<ValueParam> param = new List<ValueParam>();
            string StrQuery = entity.SQL_INSERT;
            #region 解析Insert字段
            Regex reg = new Regex(@"(Values)(.+)(\))", RegexOptions.IgnoreCase);
            var match = reg.Match(StrQuery).ToString().Trim();


            reg = new Regex(@"(\()(.+)(\))", RegexOptions.IgnoreCase);
            match = reg.Match(match).ToString().Trim();
            reg = new Regex(@"\(", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");

            reg = new Regex(@"\)", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");
            string[] array = match.Split(',');
            foreach (var Item in array)
            {
                string Value = Item.Trim();
                param.Add(new ValueParam
                {
                    Name = Value,
                    Value = Value,
                    Type = DbType.String
                });


            }
            #endregion
            StrQuery = entity.SQL_INSERT;
            var Result = conn.Execute(StrQuery, GetParameters(SetParameters(param, entity)), trans);
            return Result;
        }


        #region 非公用方法

        /// <summary>
        /// 注：只方法只能用于FlowMain可控制的Model
        /// 是一个有具体使用场景的方法，请不要随意使用
        /// 谢谢合作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="Organization">机构ID</param>
        /// <param name="OrderID">单据ID</param>
        /// <param name="FlowID">流程号</param>
        /// <param name="StepIndex">当前步骤号</param>
        /// <returns>1成功  0失败</returns>
        public static int RunFlowProcedure<T>(this T entity, string Organization, Guid ItemID, string FlowID, int StepIndex) where T : BaseEntity
        {
            using (var conn = GetConnection(entity.DataLink))
            {
                try
                {
                    string Sql = "select Operat from Flow_Main where FlowID=@FlowID and StepIndex=@StepIndex and Organization=@Organization ";
                    string SpName = conn.Query<string>(Sql, new { FlowID = FlowID, StepIndex = StepIndex, Organization = Organization }).FirstOrDefault();
                    if (!string.IsNullOrEmpty(SpName))
                    {
                        var param = new DynamicParameters();
                        param.Add("@ItemID", ItemID);
                        param.Add("@Result", 0, DbType.Int32, ParameterDirection.Output, null);
                        conn.Execute(SpName, param, null, null, CommandType.StoredProcedure);
                        int Result = param.Get<int>("@Result");
                        return Result;
                    }
                    else
                    {
                        return 1;
                    }
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        public static int RunProcedure<T>(this T entity, string SpName) where T : BaseEntity
        {
            using (var conn = GetConnection(entity.DataLink))
            {
                try
                {

                    if (!string.IsNullOrEmpty(SpName))
                    {
                        var param = new DynamicParameters();
                        param.Add("@Result", 0, DbType.Int32, ParameterDirection.Output, null);
                        conn.Execute(SpName, param, null, null, CommandType.StoredProcedure);
                        int Result = param.Get<int>("@Result");
                        return Result;
                    }
                    else
                    {
                        return 1;
                    }
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        #endregion

        #region 私有方法
        public class FieldAtrribute
        {
            /// <summary>
            /// 别名
            /// </summary>
            public string Alias { get; set; }
            /// <summary>
            /// 字段名
            /// </summary>
            public string Field { get; set; }
            /// <summary>
            /// 关键字
            /// </summary>
            public string Key { get; set; }

        }

        private static IDictionary<string, string> GenricField(string StrQuery, ref List<FieldAtrribute> OriginalField)
        {
            IDictionary<string, string> Result = new Dictionary<string, string>();
            #region 解析Select字段
            //删除回车
            StrQuery = StrQuery.Replace(NewLine, "");
            Regex reg = new Regex(@"^select(.*?)from", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var match = reg.Match(StrQuery).ToString().Trim();
            reg = new Regex(@"select", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");
            reg = new Regex(@"from", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");
            string[] array = match.Split(',');
            foreach (var Item in array)
            {
                string Value = Item.Trim();
                if (Value.ToLower().Contains(" as "))
                {
                    var FliedArr = Regex.Split(Value, " as ", RegexOptions.IgnoreCase);
                    if (!Result.Keys.Any(a => a == (FliedArr[1].Trim().ToLower())))
                    {
                        Result.Add(FliedArr[1].Trim().ToLower(), FliedArr[0].Trim());
                        OriginalField.Add(new FieldAtrribute { Key = FliedArr[1].Trim().ToLower(), Field = FliedArr[0].Trim(), Alias = FliedArr[1].Trim() });
                    }
                }
                else if (Value.Contains(" "))
                {
                    var FliedArr = Regex.Split(Item, " ");
                    if (!Result.Keys.Any(a => a == (FliedArr[1].Trim().ToLower())))
                    {
                        Result.Add(FliedArr[1].Trim().ToLower(), FliedArr[0].Trim());
                        OriginalField.Add(new FieldAtrribute { Key = FliedArr[1].Trim().ToLower(), Field = FliedArr[0].Trim(), Alias = FliedArr[1].Trim() });
                    }
                }
                else
                {
                    if (Value.Contains('.'))
                    {
                        string[] temp = Value.Split('.');
                        if (!Result.Keys.Any(a => a == (temp[1].Trim().ToLower())))
                        {
                            Result.Add(temp[1].Trim().ToLower(), Value.Trim());
                            OriginalField.Add(new FieldAtrribute { Key = temp[1].Trim().ToLower(), Field = Value.Trim(), Alias = temp[1].Trim() });
                        }
                    }
                    else
                    {
                        if (!Result.Keys.Any(a => a == (Value.Trim().ToLower())))
                        {
                            Result.Add(Value.Trim().ToLower(), Value.Trim());
                            OriginalField.Add(new FieldAtrribute { Key = Value.Trim().ToLower(), Field = Value.Trim(), Alias = Value.Trim() });
                        }
                    }
                }
            }
            #endregion
            return Result;
        }
        private static IDictionary<string, string> GenricField(string StrQuery)
        {
            IDictionary<string, string> Result = new Dictionary<string, string>();
            #region 解析Select字段
            //删除回车
            StrQuery = StrQuery.Replace(NewLine, "");
            Regex reg = new Regex(@"^select(.*?)from", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var match = reg.Match(StrQuery).ToString().Trim();
            reg = new Regex(@"select", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");
            reg = new Regex(@"from", RegexOptions.IgnoreCase);
            match = reg.Replace(match, "");
            string[] array = match.Split(',');
            foreach (var Item in array)
            {
                string Value = Item.Trim();
                if (Value.ToLower().Contains(" as "))
                {
                    var FliedArr = Regex.Split(Value, " as ", RegexOptions.IgnoreCase);
                    if (!Result.Keys.Any(a => a == (FliedArr[1].Trim().ToLower())))
                        Result.Add(FliedArr[1].Trim().ToLower(), FliedArr[0].Trim());
                }
                else if (Value.Contains(" "))
                {
                    var FliedArr = Regex.Split(Item, " ");
                    if (!Result.Keys.Any(a => a == (FliedArr[1].Trim().ToLower())))
                        Result.Add(FliedArr[1].Trim().ToLower(), FliedArr[0].Trim());
                }
                else
                {
                    if (Value.Contains('.'))
                    {
                        string[] temp = Value.Split('.');
                        if (!Result.Keys.Any(a => a == (temp[1].Trim().ToLower())))
                            Result.Add(temp[1].Trim().ToLower(), Value.Trim());
                    }
                    else
                    {
                        if (!Result.Keys.Any(a => a == (Value.Trim().ToLower())))
                            Result.Add(Value.Trim().ToLower(), Value.Trim());
                    }
                }
            }
            #endregion
            return Result;
        }

        private static string CreatePageQuery(IDbConnection connection, string SQL, string KeyColName, int PageSize, int PageIndex, bool IsCount, ref int ItemCount, ref int PageCount, DynamicParameters Params = null)
        {
            string strTemp = SQL.Trim();
            int fromIndex = strTemp.ToLower().IndexOf("from", 0);
            if (IsCount)
            {
                strTemp = "select count(*) " + SQL.Trim().Substring(fromIndex);
                ItemCount = connection.Query<int>(strTemp, Params).FirstOrDefault();
                PageSize = PageSize == 0 ? 1 : PageSize;
                PageCount = (int)Math.Ceiling(ItemCount / (double)PageSize);
            }
            strTemp = "select Top {0} * from ( select row_number() over (order by {1} ) as Temp_RowNumber, ";
            strTemp += SQL.Trim().Substring(6) + ") as a where Temp_RowNumber>{2}";
            strTemp = string.Format(strTemp, PageSize, KeyColName, PageSize * (PageIndex - 1));
            strTemp += " Order by Temp_RowNumber";
            return strTemp;
        }

        /// <summary>
        /// 根据返回实体生成Select
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="entity"></param>
        /// <param name="StrQuery"></param>
        /// <param name="Field"></param>
        /// <returns></returns>
        private static string GenricSelect<M>(string StrQuery, List<FieldAtrribute> Field)
        {
            string tempName = string.Empty;  //临时保存属性名称
            string tempSQL = "";
            PropertyInfo[] propertys = typeof(M).GetProperties();
            //循环获取T的属性及其值
            foreach (PropertyInfo pr in propertys)
            {
                tempName = pr.Name.ToLower();//将属性名称赋值给临时变量
                if (Field.Any(e => e.Key.Equals(tempName)))
                {
                    var Item = Field.First(e => e.Key.Equals(tempName));
                    if (Item.Field.Equals(Item.Alias))
                        tempSQL += " " + Item.Field + ",";
                    else
                        tempSQL += string.Format(" {0} AS {1},", Item.Field, Item.Alias);
                }
            }
            if (tempSQL.Length > 0)
            {
                tempSQL = tempSQL.Substring(0, tempSQL.Length - 1);
                tempSQL = "SELECT " + tempSQL + " FROM ";
                Regex re = new Regex("^select(.*?)from ", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                tempSQL = re.Replace(StrQuery, tempSQL, 1);
            }
            else
                tempSQL = StrQuery;
            return tempSQL;
        }
        private static IDbConnection GetConnection(string DataLink)
        {
            var config = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json")
              .Build();
            string connectionString = config.GetConnectionString(DataLink);
            return new SqlConnection(connectionString);
        }

        private static DynamicParameters GetParameters(IEnumerable<ValueParam> Param)
        {

            var param = new DynamicParameters();
            if (Param != null)
            {
                foreach (var model in Param)
                {
                    param.Add(model.Name, model.Value, model.Type);
                }
            }

            return param;
        }

        private static IEnumerable<ValueParam> SetParameters<T>(IEnumerable<ValueParam> Param, T Model)
        {
            IList<ValueParam> Result = new List<ValueParam>();

            PropertyInfo[] pi = Model.GetType().GetProperties();
            foreach (var Item in Param)
            {
                if (Item.Value.ToString().Substring(0, 1).Equals("@"))
                {
                    DbType dbtype = DbType.String;
                    var p = pi.First(e => e.Name.ToLower().Equals(Item.Value.ToString().Substring(1).ToLower()));
                    if (p != null)
                    {

                        if (p.PropertyType == typeof(int))
                        {
                            dbtype = DbType.Int32;
                        }
                        else if (p.PropertyType == typeof(Guid))
                        {
                            dbtype = DbType.Guid;
                        }

                        if (!Result.Any(a => a.Name == Item.Name))
                            Result.Add(new ValueParam
                            {
                                Name = Item.Name,
                                Value = p.GetValue(Model, null),
                                Type = dbtype
                            });
                    }
                    else
                    {
                        if (!Result.Any(a => a.Name == Item.Name))
                            Result.Add(new ValueParam
                            {
                                Name = Item.Name,
                                Value = Item.Value,
                                Type = dbtype
                            });
                    }
                }
                else
                {
                    if (!Result.Any(a => a.Name == Item.Name))
                        Result.Add(new ValueParam
                        {
                            Name = Item.Name,
                            Value = Item.Value,
                            Type = DbType.String
                        });
                }
            }
            return Result;
        }




        #region 解析Lambda表达式
        private static string BinarExpressionProvider(Expression left, Expression right, ExpressionType type, bool IsIncludeAlias, IList<ValueParam> param, IDictionary<string, string> Field)
        {
            string sb = "(";
            //先处理左边
            sb += ExpressionRouter(left, MethodType.None, IsIncludeAlias, param, Field);
            sb += ExpressionTypeCast(type);
            //再处理右边
            string tmpStr = ExpressionRouter(right, MethodType.None, IsIncludeAlias, param, Field);
            if (tmpStr == "null")
            {
                if (sb.EndsWith(" ="))
                    sb = sb.Substring(0, sb.Length - 2) + " is null";
                else if (sb.EndsWith("<>"))
                    sb = sb.Substring(0, sb.Length - 2) + " is not null";
            }
            else
                sb += tmpStr;
            return sb += ")";
        }
        private static string ExpressionRouter(Expression exp, MethodType Type, bool IsIncludeAlias, IList<ValueParam> param, IDictionary<string, string> Field)
        {
            string sb = string.Empty;
            if (exp is BinaryExpression)
            {
                BinaryExpression be = ((BinaryExpression)exp);
                return BinarExpressionProvider(be.Left, be.Right, be.NodeType, IsIncludeAlias, param, Field);
            }
            else if (exp is MemberExpression)
            {
                MemberExpression me = ((MemberExpression)exp);
                if (!me.ToString().Split('.')[0].Equals(me.Expression.ToString()))
                {
                    object CompileValue = Expression.Lambda(exp).Compile().DynamicInvoke();

                    object Result = (CompileValue ?? "");
                    string Value = "";
                    if (Type == MethodType.Like)
                    {
                        Value = "%" + Result.ToString() + "%";
                    }
                    else if (Type == MethodType.In)
                    {
                        var array = Result as object[];
                        Value = "";
                        array.ToList().ForEach(e => { Value += "," + e.ToString(); });
                        if (Value.Length > 0)
                        {
                            Value = Value.Substring(1);
                        }
                    }
                    else
                    {
                        Value = Result.ToString();
                    }
                    if (!param.Any(a => a.Name == "@" + me.Member.Name.ToString()))
                        param.Add(new ValueParam
                        {
                            Name = "@" + me.Member.Name.ToString(),
                            Value = Value,
                            Type = CompileValue is int ? DbType.Int32 : CompileValue is DateTime ? DbType.DateTime : DbType.String
                        });

                    return string.Format("{0}", "@" + me.Member.Name.ToString());
                }
                else if (me.NodeType == ExpressionType.Constant)
                {
                    return Field.Keys.Contains(me.Member.Name.ToLower()) ? IsIncludeAlias ? Field[me.Member.Name.ToLower()] : me.Member.Name : me.Member.Name;
                }
                else
                {
                    return Field.Keys.Contains(me.Member.Name.ToLower()) ? IsIncludeAlias ? Field[me.Member.Name.ToLower()] : me.Member.Name : me.Member.Name;
                }
            }
            else if (exp is NewArrayExpression)
            {
                NewArrayExpression ae = ((NewArrayExpression)exp);
                StringBuilder tmpstr = new StringBuilder();
                foreach (Expression ex in ae.Expressions)
                {
                    tmpstr.Append(ExpressionRouter(ex, MethodType.None, IsIncludeAlias, param, Field));
                    tmpstr.Append(",");
                }
                return tmpstr.ToString(0, tmpstr.Length - 1);
            }
            else if (exp is MethodCallExpression)
            {
                MethodCallExpression mce = (MethodCallExpression)exp;
                if (mce.Method.Name == "Like")
                {
                    string TempLeft = ExpressionRouter(mce.Arguments[0], MethodType.None, IsIncludeAlias, param, Field);
                    return string.Format("({0} like {1})", TempLeft, ExpressionRouter(mce.Arguments[1], MethodType.Like, IsIncludeAlias, param, Field));
                }
                else if (mce.Method.Name == "NotLike")
                {
                    string TempLeft = ExpressionRouter(mce.Arguments[0], MethodType.None, IsIncludeAlias, param, Field);
                    return string.Format("({0} Not like {1})", TempLeft, ExpressionRouter(mce.Arguments[1], MethodType.Like, IsIncludeAlias, param, Field));
                }

                else if (mce.Method.Name == "In")
                {
                    string TempLeft = ExpressionRouter(mce.Arguments[0], MethodType.None, IsIncludeAlias, param, Field);
                    return string.Format("({0} In ({1}))", TempLeft, ExpressionRouter(mce.Arguments[1], MethodType.In, IsIncludeAlias, param, Field));
                }
                else if (mce.Method.Name == "NotIn")
                {
                    string TempLeft = ExpressionRouter(mce.Arguments[0], MethodType.None, IsIncludeAlias, param, Field);
                    return string.Format("({0} Not In  {1})", TempLeft, ExpressionRouter(mce.Arguments[1], MethodType.In, IsIncludeAlias, param, Field));
                }
                else if (mce.Method.Name == "InSql")
                {
                    string TempLeft = ExpressionRouter(mce.Arguments[0], MethodType.None, IsIncludeAlias, param, Field);
                    return string.Format("( {0} in {1} )", TempLeft, mce.Arguments[0], ExpressionRouter(mce.Arguments[1], MethodType.None, IsIncludeAlias, param, Field));
                }
                else if (mce.Method.Name == "NotInSql")
                {
                    string TempLeft = ExpressionRouter(mce.Arguments[0], MethodType.None, IsIncludeAlias, param, Field);
                    return string.Format("({0} not in ({1}))", mce.Arguments[0], ExpressionRouter(mce.Arguments[1], MethodType.None, IsIncludeAlias, param, Field));
                }
                else if (mce.Method.Name == "SubString")
                {
                    string[] Methods = mce.ToString().Split('.');
                    string FliedName = Field.Keys.Contains(Methods[1].ToLower()) ? IsIncludeAlias ? Field[Methods[1].ToLower()] : Methods[1] : Methods[1];
                    string BeginIndex = ExpressionRouter(mce.Arguments[1], MethodType.None, IsIncludeAlias, param, Field);
                    string Len = ExpressionRouter(mce.Arguments[2], MethodType.None, IsIncludeAlias, param, Field);
                    string Value = ExpressionRouter(mce.Arguments[3], MethodType.None, IsIncludeAlias, param, Field);
                    return string.Format("(SUBSTRING({0},{1},{2}))={3}", FliedName, int.Parse(BeginIndex) + 1, Len, Value);
                }

            }
            else if (exp is ConstantExpression)
            {
                ConstantExpression ce = ((ConstantExpression)exp);

                if (ce.Value == null)
                    return "null";
                else if (ce.Value is ValueType)
                {

                    if (ce.Value is Boolean)
                        return Convert.ToInt32(ce.Value).ToString();
                    else
                        //   param.Add("@" + i.ToString(), ce.Value.ToString());
                        return ce.Value.ToString();
                }
                else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
                {
                    string KeyName = Guid.NewGuid().ToString().Substring(0, 8);
                    string Value = ce.Value.ToString();
                    if (Type == MethodType.Like)
                        Value = "%" + Value + "%";
                    else if (Type == MethodType.In)
                    {
                        var array = ce.Value as object[];
                        Value = "";
                        array.ToList().ForEach(e => { Value += "," + e.ToString(); });
                        if (Value.Length > 0)
                        {
                            Value = Value.Substring(1);
                        }
                    }
                    if (!param.Any(a => a.Name == "@" + KeyName))
                        param.Add(new ValueParam
                        {
                            Name = "@" + KeyName,
                            Value = Value,
                            Type = ce.Value.GetType() == typeof(Int32) ? DbType.Int32 : ce.Value.GetType() == typeof(DateTime) ? DbType.DateTime : DbType.String

                        });

                    return string.Format("{0}", "@" + KeyName);

                }

            }
            else if (exp is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp);
                return ExpressionRouter(ue.Operand, MethodType.None, IsIncludeAlias, param, Field);
            }
            return null;
        }
        private static string ExpressionTypeCast(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Equal:
                    return " =";
                case ExpressionType.GreaterThan:
                    return " >";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return " Or ";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                default:
                    return null;
            }
        }
        #endregion
        #endregion

    }
}
