using CustomerService.DAO;
using CustomerService.Models; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerDbContext _context;

        public CustomerController(CustomerDbContext context)
        {
            _context = context;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var customers = await _context.Customers
                .AsNoTracking()
                .ToListAsync();

            return Ok(customers);
        }

        // GET: api/customers/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (customer == null)
            {
                return NotFound(new
                {
                    Message = $"Customer with ID {id} not found."
                });
            }

            return Ok(customer);
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(
            CreateCustomerDto request)
        {
            // Check duplicate email
            var emailExists = await _context.Customers
                .AnyAsync(x => x.Email == request.Email);

            if (emailExists)
            {
                return Conflict(new
                {
                    Message = "Customer with this email already exists."
                });
            }

            var customer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCustomer),
                new { id = customer.Id },
                customer);
        }

        // PUT: api/customers/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCustomer(
            int id,
            UpdateCustomerDto request)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (customer == null)
            {
                return NotFound(new
                {
                    Message = $"Customer with ID {id} not found."
                });
            }

            // Check duplicate email
            var emailExists = await _context.Customers
                .AnyAsync(x =>
                    x.Email == request.Email &&
                    x.Id != id);

            if (emailExists)
            {
                return Conflict(new
                {
                    Message = "Another customer already uses this email."
                });
            }

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;
            customer.PhoneNumber = request.PhoneNumber;
            customer.IsActive = request.IsActive;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/customers/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (customer == null)
            {
                return NotFound(new
                {
                    Message = $"Customer with ID {id} not found."
                });
            }

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/customers/5/activate
        [HttpPatch("{id:int}/activate")]
        public async Task<IActionResult> ActivateCustomer(int id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            customer.IsActive = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Customer activated successfully."
            });
        }

        // PATCH: api/customers/5/deactivate
        [HttpPatch("{id:int}/deactivate")]
        public async Task<IActionResult> DeactivateCustomer(int id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            customer.IsActive = false;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Customer deactivated successfully."
            });
        }
    }
}
