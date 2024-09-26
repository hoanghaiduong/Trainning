using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainning.Data;
using Trainning.DTO;
using Trainning.Entities;
using Trainning.Helpers;
using Trainning.Services;

namespace Trainning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly BookService _bookService;
        private readonly SQLHelperNoContext _sql;

        public BookController(BookService bookService, SQLHelperNoContext sql)
        {
            _bookService = bookService;
            _sql = sql;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }
        [HttpGet("books/sql")]
        public async Task<IActionResult> GetAllBooksWithSQL()
        {
          var books=await _sql.ExecuteQueryAsync("SELECT * FROM books");
          return Ok(new {books});
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();

            return Ok(book);
        }
        [HttpPost("add-book")]
        public async Task<IActionResult> AddBookInMemory(Book book)
        {
            try
            {
                await _bookService.AddBookAsync(book);

                return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = ex.Message,
                });
            }
        }
        [HttpPost("insert-sql")]
        public async Task<IActionResult> AddBook([FromBody] BookCreateDto dto)
        {
            try
            {
                var book = new Book()
                {

                    Title = dto.Title,
                    Author = dto.Author,
                    PublishedDate = dto.PublishedDate,
                    ISBN = dto.ISBN
                };

                await _bookService.AddBookAsync(book);

                return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = 400,
                    message = ex.Message,
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook([FromRoute] int id, [FromBody] BookUpdateDto dto)
        {
            try
            {
                var newBook = new Book
                {
                    Id = id,
                    Title = dto.Title,
                    Author = dto.Author,
                    ISBN = dto.ISBN,
                    PublishedDate = dto.PublishedDate
                };
                await _bookService.UpdateBookAsync(id, newBook);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                await _bookService.DeleteBookAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}