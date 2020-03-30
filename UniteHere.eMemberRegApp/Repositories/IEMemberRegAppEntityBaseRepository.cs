using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UniteHere.eMemberRegApp.Entities;

namespace UniteHere.eMemberRegApp.Repositories {
  public interface IMemberRegAppEntityBaseRepository<T> where T : class, IMemberRegAppEntityBase, new() {
    ApplicationDbContext DbContext { get; }
    IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
    IQueryable<T> All { get; }
    IQueryable<T> GetAll();
    T GetSingle(string id);
    IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
    void Add(T entity);
    void Delete(T entity);
    void Edit(T entity);
  }
}
