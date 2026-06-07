using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payments.Api.Interfaces;
using Shared.Requests;

namespace Payments.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentService paymentService) : ControllerBase
{

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePayment(CreatePaymentRequest request)
    {
        var result = await paymentService.ProcessPaymentAsync(request);
        if (!result)
        {
            return BadRequest(new { Message = "Failed to process payment." });
        }

        return Accepted(new { Message = "Payment processed successfully." });
    }
}
