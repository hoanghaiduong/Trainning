using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Trainning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        // GET: api/test
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok("Get all items");
        }

        // GET: api/test/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            return Ok($"Get item with ID: {id}");
        }

        // POST: api/test
        [HttpPost]
        public IActionResult Create()
        {
            return Ok("Item created successfully");
        }

        // PUT: api/test/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id)
        {
            return Ok($"Item with ID: {id} updated successfully");
        }

        // DELETE: api/test/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return Ok($"Item with ID: {id} deleted successfully");
        }

        // Example CRUD method with actual data
            
        public class Item
        {
            [Required(ErrorMessage = "ID is required")]
            public int Id { get; set; }

            [Required(ErrorMessage = "Name is required")]
            [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
            public string Name { get; set; }

            [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
            public string Description { get; set; }
        }

        // POST: api/test/withdata
        [HttpPost("withdata")]
        public IActionResult CreateWithData([FromBody] Item newItem)
        {
            // Normally you would save the item to a database
            return Ok(new { message = "Item created", data = newItem });
        }
    }
}
