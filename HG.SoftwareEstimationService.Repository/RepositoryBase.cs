using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace HG.SoftwareEstimationService.Repository
{
    public abstract class RepositoryBase<TContext, TDbSet> : DbContext
        where TContext : DbContext
        where TDbSet : class
    {
        protected DbContext DataContext;

        protected RepositoryBase()
        {
            DataContext = (TContext)Activator.CreateInstance(typeof(TContext), ConnectionStringManager.GetConnectionString());
        }

        public IQueryable<TDbSet> GetAll()
        {
            return DataContext.Set<TDbSet>();
        }

        public IEnumerable<TDbSet> GetMany(Func<TDbSet, bool> func)
        {
            IQueryable<TDbSet> a = GetAll();
            return a.Where(func);
        }

        public TDbSet GetSingleOrDefault(Func<TDbSet, bool> func)
        {
            return GetAll().SingleOrDefault(func);
        }

        public void Add(TDbSet item)
        {
            DataContext.Set<TDbSet>().Add(item);
        }

        public void AddMany(IEnumerable<TDbSet> set)
        {
            DataContext.Set<TDbSet>().AddRange(set);
        }

        public new void SaveChanges()
        {
            try
            {
                DataContext.SaveChanges();
            }
            catch (Exception)
            {
                DbEntityValidationResult validationError = DataContext.GetValidationErrors().FirstOrDefault();
                if (validationError != null)
                {
                    throw new Exception(validationError.ToString());
                }

                throw;
            }
        }

        public TDbSet GetById(int id)
        {
            return DataContext.Set<TDbSet>().Find(id);
        }

        public TDbSet GetById(long id)
        {
            return DataContext.Set<TDbSet>().Find(id);
        }

        public void Replace(TDbSet oldItem, TDbSet newItem)
        {
            DataContext.Entry(oldItem).CurrentValues.SetValues(newItem);
        }

        public void Replace<T>(Func<TDbSet, T> func, TDbSet newItem)
        {
            TDbSet oldItem = DataContext.Set<TDbSet>().Find(func(newItem));
            if (oldItem == null)
            {
                Add(newItem);
                return;
            }

            DataContext.Entry(oldItem).CurrentValues.SetValues(newItem);
        }

        public void Delete(TDbSet item)
        {
            DataContext.Set<TDbSet>().Remove(item);
        }

        public void DeleteMany(Func<TDbSet, bool> func)
        {
            foreach (TDbSet item in GetAll().Where(func))
            {
                Delete(item);
            }
        }
    }

    public interface IRepositorySqlite<TDbSet> where TDbSet : class
    {
        IEnumerable<TDbSet> GetAll();

        IEnumerable<TDbSet> GetMany(Func<TDbSet, bool> func);

        TDbSet GetSingleOrDefault(Func<TDbSet, bool> func);

        void Add(TDbSet item);

        void AddMany(IEnumerable<TDbSet> set);

        TDbSet GetById(int id);

        TDbSet GetById(long id);

        void Replace(TDbSet oldItem, TDbSet newItem);

        void Replace<T>(Func<TDbSet, T> func, TDbSet newItem);

        void Delete(TDbSet item);

        void DeleteMany(Func<TDbSet, bool> func);

        void SaveChanges();
    }
}