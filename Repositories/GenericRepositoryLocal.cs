using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Trainning.Interfaces;

namespace Trainning.Repositories
{
    public class GenericRepositoryLocal<T, TKey> : IGenericRepository<T, TKey> where T : class
    {
        private static List<T> _items = [];
        private static Func<T, TKey> _keySelector;
        public GenericRepositoryLocal()
        {
            _keySelector = GenericRepositoryLocal<T, TKey>.GetKeySelector();
        }
        private static Func<T, TKey> GetKeySelector()
        {
            var keyProperty = typeof(T).GetProperties()
                .FirstOrDefault(prop => prop.GetCustomAttribute<KeyAttribute>() != null) ?? throw new InvalidOperationException($"No property with [Key] attribute found on {typeof(T).Name}");
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, keyProperty);
            var convert = Expression.Convert(property, typeof(TKey)); // Ensure the type is TKey
            return Expression.Lambda<Func<T, TKey>>(convert, parameter).Compile();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.FromResult(_items);
        }
        public async Task<T> GetByIdAsync(TKey id)
        {
            var entity = _items.FirstOrDefault(item => _keySelector(item).Equals(id));
            return await Task.FromResult(entity);
        }
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            var query = _items.AsQueryable().Where(predicate);
            return await Task.FromResult(query.ToList());
        }
        public async Task AddAsync(T entity)
        {
            _items.Add(entity);
            await Task.CompletedTask;
        }
        public async Task UpdateAsync(T entity)
        {
            var id = _keySelector(entity);
            var existingEntity = await GetByIdAsync(id);

            if (existingEntity != null)
            {
                _items.Remove(existingEntity);
                _items.Add(entity);
            }

            await Task.CompletedTask;
        }
        public async Task DeleteAsync(T entity)
        {
            _items.Remove(entity);
            await Task.CompletedTask;
        }
    }
}