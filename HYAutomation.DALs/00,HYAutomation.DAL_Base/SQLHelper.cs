using Dapper;
using HYAutomation.Core;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HYAutomation.DAL_Base
{
    public class SQLHelper
    {
        private static readonly ConfigManagerUtil configManager = ConfigManagerUtil.GetInstance(System.Reflection.Assembly.GetExecutingAssembly());
        private static readonly string SQLServerconnectStr = configManager.GetValue("SQLServer");
        private static readonly string MySQLconnectStr = configManager.GetValue("MySQL");
        public DataBaseTypes DataBaseType { get; }
        public string CurrentConnStr { get; }
        public enum DataBaseTypes
        {
            SQLServer,
            MySQL
        }
        public static SQLHelper GetInstance(DataBaseTypes dbType)
        {
            return new SQLHelper(dbType);
        }
        public static SQLHelper GetInstance(DataBaseTypes dbType, string connStr)
        {
            return new SQLHelper(dbType, connStr);
        }
        private SQLHelper(DataBaseTypes dbType, string connstr = null)
        {
            CurrentConnStr = connstr;
            DataBaseType = dbType;
        }
        private IDbConnection GetDbConnection()
        {
            IDbConnection dbConnection = null;
            switch (DataBaseType)
            {
                case DataBaseTypes.SQLServer:
                    {
                        string connstr = string.IsNullOrEmpty(CurrentConnStr) ? SQLServerconnectStr : CurrentConnStr;
                        dbConnection = new SqlConnection(connstr);
                        break;
                    }
                case DataBaseTypes.MySQL:
                    {
                        string connstr = string.IsNullOrEmpty(CurrentConnStr) ? MySQLconnectStr : CurrentConnStr;
                        dbConnection = new MySqlConnection(connstr);
                        break;
                    }
                default:
                    break;
            }
            return dbConnection;
        }
        /// <summary>
        /// 查询数据列表(有参)
        /// </summary>
        /// <typeparam name="T">查询类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.Query<T>(sql, param);
            }

        }
        /// <summary>
        /// 异步查询数据列表（有参）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QueryAsync<T>(sql, param);
            }
        }
        /// <summary>
        /// 查询数据列表（无参）
        /// </summary>
        /// <typeparam name="T">查询类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.Query<T>(sql);
            }
        }
        /// <summary>
        /// 异步查询数据列表（无参）
        /// /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> QueryAsync<T>(string sql)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QueryAsync<T>(sql);
            }
        }
        /// <summary>
        /// 查询首条数据(无默认值，无参)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T QueryFirst<T>(string sql)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QueryFirst<T>(sql);
            }
        }
        /// <summary>
        /// 异步查询首条数据(无默认值,无参)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Task<T> QueryFirstAsync<T>(string sql)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QueryFirstAsync<T>(sql);
            }
        }
        /// <summary>
        /// 查询首条数据(无默认值，有参)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T QueryFirst<T>(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QueryFirst<T>(sql, param);
            }
        }
        /// <summary>
        /// 异步查询首条数据(无默认值,有参)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<T> QueryFirstAsync<T>(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QueryFirstAsync<T>(sql, param);
            }
        }
        /// <summary>
        /// 查询首条数据(有默认值,无参)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T QueryFirstOrDefault<T>(string sql)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QueryFirstOrDefault<T>(sql);
            }
        }
        /// <summary>
        /// 异步查询首条数据(有默认值,无参)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Task<T> QueryFirstOrDefaultAsync<T>(string sql)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QueryFirstOrDefaultAsync<T>(sql);
            }

        }
        /// <summary>
        /// 查询首条数据(有默认值,有参)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T QueryFirstOrDefault<T>(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QueryFirstOrDefault<T>(sql, param);
            }
        }
        /// <summary>
        /// 异步查询首条数据(有默认值,有参)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QueryFirstOrDefaultAsync<T>(sql, param);
            }

        }

        /// <summary>
        /// 查询单条数据(无默认值，无参)
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T QuerySingle<T>(string sql)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QuerySingle<T>(sql);
            }
        }
        /// <summary>
        /// 异步查询单条数据(无默认值，无参)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Task<T> QuerySingleAsync<T>(string sql)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QuerySingleAsync<T>(sql);
            }
        }
        /// <summary>
        /// 查询单条数据(无默认值，有参)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T QuerySingle<T>(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QuerySingle<T>(sql, param);
            }
        }
        /// <summary>
        /// 异步查询单条数据(无默认值，有参)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<T> QuerySingleAsync<T>(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QuerySingleAsync<T>(sql, param);
            }
        }
        /// <summary>
        /// 查询单条数据（有默认值，无参）
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public T QuerySingleOrDefault<T>(string sql)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QuerySingleOrDefault<T>(sql);
            }
        }
        /// <summary>
        /// 异步查询单条数据（有默认值，无参）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Task<T> QuerySingleOrDefaultAsync<T>(string sql)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QuerySingleOrDefaultAsync<T>(sql);
            }
        }
        /// <summary>
        /// 查询单条数据（有默认值）
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T QuerySingleOrDefault<T>(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QuerySingleOrDefault<T>(sql, param);
            }
        }
        /// <summary>
        /// 异步查询单条数据（有默认值）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QuerySingleOrDefaultAsync<T>(sql, param);
            }
        }
        /// <summary>
        /// 操作数据库(有参)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int Execute(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.Execute(sql, param);
            }
        }
        /// <summary>
        /// 异步操作数据库(有参)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<int> ExecuteAsync(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.ExecuteAsync(sql, param);
            }

        }
        /// <summary>
        ///操作数据库（无参）
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Execute(string sql)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.Execute(sql);
            }

        }
        /// <summary>
        /// 异步操作数据库（无参）
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Task<int> ExecuteAsync(string sql)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.ExecuteAsync(sql);
            }
        }

        /// <summary>
        /// Reader获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.ExecuteReader(sql, param);
            }
        }
        /// <summary>
        /// Reader异步获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<IDataReader> ExecuteReaderAsync(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.ExecuteReaderAsync(sql, param);
            }

        }

        /// <summary>
        /// Scalar获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.ExecuteScalar(sql, param);
            }
        }
        /// <summary>
        /// Scalar异步获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<object> ExecuteScalarAsync(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.ExecuteScalarAsync(sql, param);
            }
        }

        /// <summary>
        /// Scalar获取T数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.ExecuteScalar<T>(sql, param);
            }
        }
        /// <summary>
        /// Scalar异步获取T数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<T> ExecuteScalarAysnc<T>(string sql, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.ExecuteScalarAsync<T>(sql, param);
            }
        }

        /// <summary>
        /// 带参数的存储过程
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<T> ExecutePro<T>(string proc, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                List<T> list = connection.Query<T>(proc,
                param,
                null,
                true,
                null,
                CommandType.StoredProcedure).ToList();
                return list;
            }

        }
        /// <summary>
        /// 异步带参数的存储过程
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> ExecuteProAsync<T>(string proc, object param)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                return connection.QueryAsync<T>(proc,
  param,
  null,
 5000,
  CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 事务1(无参) - 全SQL
        /// </summary>
        /// <param name="sqlarr">多条SQL</param>
        /// <param name="param">param</param>
        /// <returns></returns>
        public int ExecuteTransaction(string[] sqlarr)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int result = 0;
                        foreach (var sql in sqlarr)
                        {
                            result += connection.Execute(sql, null, transaction);
                        }

                        transaction.Commit();
                        return result;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// 事务2(有参) - 全SQL
        /// </summary>
        /// <param name="Key">多条SQL</param>
        /// <param name="Value">param</param>
        /// <returns></returns>
        public int ExecuteTransaction(List<Tuple<string, object>> dic)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                try
                {
                    int result = 0;
                    foreach (var sql in dic)
                    {
                        result += connection.Execute(sql.Item1, sql.Item2, transaction);
                    }

                    transaction.Commit();
                    return result;
                }
                catch
                {
                    transaction.Rollback();
                    return 0;
                }
            }
        }
    }
}
