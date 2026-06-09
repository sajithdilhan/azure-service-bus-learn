using Microsoft.AspNetCore.Mvc;
using Payments.Api.Interfaces;
using Shared.Requests;

namespace Payments.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(IPaymentService paymentService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePayment(CreatePaymentRequest request)
    {
        var result = await paymentService.ProcessPaymentAsync(request);
        if (!result.IsSuccess)
        {
            return StatusCode(result.Error?.Status ?? StatusCodes.Status400BadRequest, result.Error);
        }

        return Accepted(result.Value);
    }
}
