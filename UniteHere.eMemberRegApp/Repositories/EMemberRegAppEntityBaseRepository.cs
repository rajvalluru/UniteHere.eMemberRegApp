using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using UniteHere.eMemberRegApp.Entities;
using UniteHere.eMemberRegApp.Infrastructure;

namespace UniteHere.eMemberRegApp.Repositories
{
    public class EMemberRegAppEntityBaseRepository<T> : IMemberRegAppEntityBaseRepository<T> where T : class, IMemberRegAppEntityBase, new()
    {
        private ApplicationDbContext dataContext;

        protected IDbFactory DbFactory { get; private set; }

        public ApplicationDbContext DbContext
        {
            get { return (dataContext ?? (dataContext = DbFactory.Init())); }
        }

        public EMemberRegAppEntityBaseRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }
        public EMemberRegAppEntityBaseRepository(ApplicationDbContext dbContext)
        {
            dataContext = dbContext;
        }

        public virtual IQueryable<T> GetAll()
        {
            return DbContext.Set<T>();
        }

        public virtual IQueryable<T> All
        {
            get { return GetAll(); }
        }

        public virtual IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = DbContext.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public virtual T GetSingle(string id)
        {
            return GetAll().FirstOrDefault(x => x.Id == id);
        }

        public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return DbContext.Set<T>().Where(predicate);
        }

        public virtual void Add(T entity)
        {
            if(entity.Id.IsNullOrWhiteSpace())
              entity.Id = Guid.NewGuid().ToString();
            entity.CreatedBy = Thread.CurrentPrincipal.Identity.Name;
            entity.CreatedOn = DateTime.Now;
            DbEntityEntry dataEntityEntry = DbContext.Entry<T>(entity);
            DbContext.Set<T>().Add(entity);
        }

        public virtual void Edit(T entity)
        {
            entity.ModifiedBy = Thread.CurrentPrincipal.Identity.Name;
            entity.ModifiedOn = DateTime.Now;
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }
    }
}
