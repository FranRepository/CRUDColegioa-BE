﻿namespace ColegioCRUDApp.Interfaces
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// This encapsulates the logic required to access data sources.
        /// Thus, a repository as the middleman between the data source and the backend logic.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>

        long Insert(T entity);
        bool Update(T entity);
        bool Delete(T entity);
        T Get(int Id);
        IEnumerable<T> GetAll();

    }
}