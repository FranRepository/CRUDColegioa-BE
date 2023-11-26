using ColegioCRUDApp.Interfaces;
using Dapper.Contrib.Extensions;
using System.Data.SqlClient;

namespace ColegioCRUDApp.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public long Insert(T entity)
        {
            using (SqlConnection cnx = new SqlConnection(_connectionString))
            {
                return cnx.Insert(entity);
            }
        }

        public bool Update(T entity)
        {
            using (SqlConnection cnx = new SqlConnection(_connectionString))
            {
                return cnx.Update(entity);
            }
        }
        public bool Delete(T entity)
        {
            using (SqlConnection cnx = new SqlConnection(_connectionString))
            {
                return cnx.Delete(entity);
            }
        }

        public T Get(int id)
        {
            using (SqlConnection cnx = new SqlConnection(_connectionString))
            {
                return cnx.Get<T>(id);
            }
        }
        public IEnumerable<T> GetAll()
        {
            using (SqlConnection cnx = new SqlConnection(_connectionString))
            {
                return cnx.GetAll<T>();
            }
        }
    }
}
