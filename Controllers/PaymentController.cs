
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainning.Common.Enums;
using Trainning.Common.Mappers;
using Trainning.Data;
using Trainning.DTO.Creates;
using Trainning.DTO.Updates;
using Trainning.Entities;

namespace Trainning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IResult> GetPayments()
        {
            try
            {
                var payments = (await _context.Payments.Include(r => r.Booking).ToListAsync()).Select(s => s.ToPaymentDTO());
                return Results.Ok(new
                {
                    message = "Get Payment Successful",
                    data = payments
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        public async Task<IResult> GetPayment([FromQuery] int id)
        {
            try
            {
                var payment = await _context.Payments.Include(r => r.Booking).FirstOrDefaultAsync(x => x.Id == id);
                if (payment == null)
                {
                    return Results.NotFound(new { message = "Payment not found" });
                }

                return Results.Ok(new
                {
                    message = "Get Payment Successful",
                    data = payment.ToPaymentDTO()
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IResult> CreatePayment([FromBody] CreatePaymentDTO dto, [FromQuery] EPaymentMethod paymentMethod = EPaymentMethod.Cash)
        {
            try
            {
                var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id == dto.BookingId);
                if (booking == null)
                {
                    return Results.NotFound(new { message = "Booking not found" });
                }
                var newPayment = new Payment
                {
                    Amount = dto.Amount,
                    PaymentDate = dto.PaymentDate,
                    PaymentMethod = paymentMethod,
                    Booking = booking
                };
                await _context.Payments.AddAsync(newPayment);
                await _context.SaveChangesAsync();
                return Results.Ok(new
                {
                    Message = "Create Payment Successful",
                    data = newPayment.ToPaymentDTO()
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        [HttpPut]
        public async Task<IResult> UpdatePayment([FromQuery] int id, [FromBody] UpdatePaymentDTO dto, [FromQuery] EPaymentMethod paymentMethod = EPaymentMethod.Cash)
        {
            try
            {
                var payment = await _context.Payments.Include(r=>r.Booking).FirstOrDefaultAsync(x => x.Id == id);
                if (payment == null)
                {
                    return Results.NotFound(new { message = "Payment Not Found" });
                }
                else
                {
                    if (dto.BookingId != null)
                    {
                        var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id == dto.BookingId);
                        if (booking == null)
                        {
                            return Results.NotFound(new { message = "Booking not found" });
                        }
                        payment.Booking = booking;
                    }

                }
                payment.Amount = dto.Amount ?? payment.Amount;
                payment.PaymentDate = dto.PaymentDate ?? payment.PaymentDate;
                payment.PaymentMethod = !string.IsNullOrEmpty(paymentMethod.ToString()) ? paymentMethod : payment.PaymentMethod;
                _context.Payments.Update(payment);
                await _context.SaveChangesAsync();
                return Results.Ok(new
                {
                    message = "Updated payment successfully",
                    data = payment.ToPaymentDTO()
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        [HttpDelete]
        public async Task<IResult> DeletePayment([FromQuery] int id)
        {
            try
            {
                var payment = await _context.Payments.FirstOrDefaultAsync(x => x.Id == id);
                if (payment == null)
                {
                    return Results.NotFound(new { message = "Payment Not Found" });
                }
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
                return Results.Ok(new {
                    message="Payment deleted successfully",
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}