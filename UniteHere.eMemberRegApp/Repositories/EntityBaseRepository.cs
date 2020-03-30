using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UniteHere.eMemberRegApp.Entities;
using UniteHere.eMemberRegApp.Infrastructure;

namespace UniteHere.eMemberRegApp.Repositories {
  public class EntityBaseRepository<T> : IEntityBaseRepository<T> where T : class, IEntityBase, new() {
    private ApplicationDbContext dataContext;

    protected IDbFactory DbFactory { get; private set; }

    public ApplicationDbContext DbContext {
      get { return (dataContext ?? (dataContext = DbFactory.Init())); }
    }

    public EntityBaseRepository(IDbFactory dbFactory) {
      DbFactory = dbFactory;
    }

        public EntityBaseRepository(ApplicationDbContext dbContext)
        {
            dataContext = dbContext;
        }
        public virtual IQueryable<T> GetAll() {
      return DbContext.Set<T>();
    }

    public virtual IQueryable<T> All {
      get { return GetAll(); }
    }

    public virtual IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties) {
      IQueryable<T> query = DbContext.Set<T>();

      foreach (var includeProperty in includeProperties) {
        query = query.Include(includeProperty);
      }
      return query;
    }

    public virtual T GetSingle(string id) {
      return GetAll().FirstOrDefault(x => x.Id == id);
    }

    public virtual IQueryable<T> FindBy(Expression<Func<T, bool>> predicate) {
      return DbContext.Set<T>().Where(predicate);
    }

    public virtual void Add(T entity) {
      entity.Id = Guid.NewGuid().ToString();
      DbEntityEntry dataEntityEntry = DbContext.Entry<T>(entity);
      DbContext.Set<T>().Add(entity);
    }

    public virtual void Edit(T entity) {
      DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
      dbEntityEntry.State = EntityState.Modified;
    }

    public virtual void Delete(T entity) {
      DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
      dbEntityEntry.State = EntityState.Deleted;
    }
  }

}
