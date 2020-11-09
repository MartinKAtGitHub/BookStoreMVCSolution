using BookStoreMVC.DataAccess.Data;
using BookStoreMVC.DataAccess.Repository.IRepository;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookStoreMVC.DataAccess.Repository
{
    public class StoredProcedure_Call : IStoredProcedure_Call
    {
        private static string ConnectionString = "";
        private readonly ApplicationDbContext _dbContext;
        public StoredProcedure_Call(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            ConnectionString = dbContext.Database.GetDbConnection().ConnectionString;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public void Execute(string procedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            sqlConnection.Execute(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);
        }

        public IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            return sqlConnection.Query<T>(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            var result = SqlMapper.QueryMultiple(sqlConnection, procedureName, param, commandType: System.Data.CommandType.StoredProcedure);
            var item1 = result.Read<T1>().ToList();
            var item2 = result.Read<T2>().ToList();
            
            if(item1!=null && item2 != null)
            {
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
            }

            return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(), new List<T2>());

        }

        public T OnceRecord<T>(string procedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            var value = sqlConnection.Query<T>(procedureName, param, commandType: System.Data.CommandType.StoredProcedure);
            return (T)Convert.ChangeType(value.FirstOrDefault(), typeof(T));
        }

        public T single<T>(string procedureName, DynamicParameters param = null)
        {
            using SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            sqlConnection.Open();
            return (T)Convert.ChangeType( sqlConnection.ExecuteScalar<T>(procedureName, param, commandType: System.Data.CommandType.StoredProcedure), typeof(T));
        }
    }
}
