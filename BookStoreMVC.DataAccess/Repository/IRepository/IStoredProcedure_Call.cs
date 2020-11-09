using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookStoreMVC.DataAccess.Repository.IRepository
{
    public interface IStoredProcedure_Call : IDisposable
    {
        /// <summary>
        /// Returns a single value, count or first value
        /// </summary>
        T single<T>(string procedureName, DynamicParameters param = null);
        /// <summary>
        ///  Executes a procedure without returning anything, example Adding or Deleting something
        /// </summary>
        void Execute(string procedureName, DynamicParameters param = null);
        /// <summary>
        /// Returns 1 row/record
        /// </summary>
        T OnceRecord<T>(string procedureName, DynamicParameters param = null);
        /// <summary>
        /// Returns all rows
        /// </summary>
        IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null);
        /// <summary>
        /// Returns 2 tables
        /// </summary>
        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null);
    }
}
