using Trainning.Entities;
using Trainning.Interfaces;

namespace Trainning.Services
{

    public class BookService
    {
        private readonly IGenericRepository<Book, int> _bookRepository;

        public BookService(IGenericRepository<Book, int> bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task AddBookAsync(Book book)
        {
            var books = await _bookRepository.GetAllAsync();
            var existingBook = books.FirstOrDefault(s => s.Author == book.Author);
            if (existingBook != null)
            {
                throw new InvalidOperationException("Book already exists.");
            }
            await _bookRepository.AddAsync(book);
        }

        public async Task UpdateBookAsync(int id, Book book)
        {
            var existingBook = await _bookRepository.GetByIdAsync(id);
            if (existingBook != null)
            {
                await _bookRepository.UpdateAsync(book);
            }
            else
            {
                throw new KeyNotFoundException("Book not found");
            }
        }

        public async Task DeleteBookAsync(int id)
        {
            var existingBook = await _bookRepository.GetByIdAsync(id);
            if (existingBook != null)
            {
                await _bookRepository.DeleteAsync(existingBook);
            }
            else
            {
                throw new KeyNotFoundException("Book not found");
            }
        }

    }
}