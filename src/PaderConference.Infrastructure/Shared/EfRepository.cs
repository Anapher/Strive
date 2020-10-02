#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.

using PaderConference.Core.Interfaces.Gateways.Repositories;
using PaderConference.Core.Shared;
using PaderConference.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaderConference.Infrastructure.Shared
{
    public abstract class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _appDbContext;

        protected EfRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public virtual ValueTask<T?> GetById(int id)
        {
            return _appDbContext.Set<T>().FindAsync(id);
        }

        public async Task<IList<T>> GetAll()
        {
            return await _appDbContext.Set<T>().ToListAsync();
        }

        public async Task<T> Add(T entity)
        {
            _appDbContext.Set<T>().Add(entity);
            await _appDbContext.SaveChangesAsync();
            return entity;
        }

        public Task Update(T entity)
        {
            _appDbContext.Entry(entity).State = EntityState.Modified;
            return _appDbContext.SaveChangesAsync();
        }

        public Task Delete(T entity)
        {
            _appDbContext.Set<T>().Remove(entity);
            return _appDbContext.SaveChangesAsync();
        }

        public Task<T?> FirstOrDefaultBySpecs(params ISpecification<T>[] specs)
        {
            return QuerySpecs(specs).FirstOrDefaultAsync();
        }

        public async Task<IList<T>> GetAllBySpecs(params ISpecification<T>[] specs)
        {
            return await QuerySpecs(specs).ToListAsync();
        }

        protected IQueryable<T> QuerySpecs(ISpecification<T>[] specs)
        {
            // fetch a Queryable that includes all expression-based includes
            var queryableResultWithIncludes = specs.SelectMany(x => x.Includes)
                .Aggregate(_appDbContext.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));

            // modify the IQueryable to include any string-based include statements
            var secondaryResult = specs.SelectMany(x => x.IncludeStrings)
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            // return the result of the query using the specification's criteria expression
            return specs.Aggregate(secondaryResult, (query, spec) => query.Where(spec.Criteria));
        }
    }
}
