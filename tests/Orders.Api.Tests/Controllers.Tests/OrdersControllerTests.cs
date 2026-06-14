using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Orders.Api.Controllers;
using Orders.Api.Interfaces;
using Shared.Common;
using Shared.Enums;
using Shared.Requests;
using Shared.Responses;

namespace Orders_Api_Tests.Controllers.Tests;

public class OrdersControllerTests
{
    [Fact]
    public async Task GetOrders_WhenServiceSucceeds_ReturnsOkWithOrders()
    {
        var orders = new List<OrderResponse>
        {
            CreateOrderResponse(status: OrderStatus.Pending),
            CreateOrderResponse(status: OrderStatus.Confirmed)
        };

        var orderService = new Mock<IOrderService>(MockBehavior.Strict);
        orderService
            .Setup(service => service.GetOrdersAsync())
            .ReturnsAsync(Result<IReadOnlyCollection<OrderResponse>>.Success(orders));

        var subject = new OrdersController(orderService.Object);

        var result = await subject.GetOrders();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Same(orders, okResult.Value);
        orderService.Verify(service => service.GetOrdersAsync(), Times.Once);
        orderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetOrders_WhenServiceReturnsEmptyCollection_ReturnsOkWithEmptyCollection()
    {
        IReadOnlyCollection<OrderResponse> orders = [];

        var orderService = new Mock<IOrderService>(MockBehavior.Strict);
        orderService
            .Setup(service => service.GetOrdersAsync())
            .ReturnsAsync(Result<IReadOnlyCollection<OrderResponse>>.Success(orders));

        var subject = new OrdersController(orderService.Object);

        var result = await subject.GetOrders();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        var response = Assert.IsAssignableFrom<IReadOnlyCollection<OrderResponse>>(okResult.Value);
        Assert.Empty(response);
        orderService.Verify(service => service.GetOrdersAsync(), Times.Once);
        orderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetOrders_WhenServiceFails_ReturnsNotFound()
    {
        var orderService = new Mock<IOrderService>(MockBehavior.Strict);
        orderService
            .Setup(service => service.GetOrdersAsync())
            .ReturnsAsync(Result<IReadOnlyCollection<OrderResponse>>.Failure(new Error(StatusCodes.Status404NotFound, "not found")));

        var subject = new OrdersController(orderService.Object);

        var result = await subject.GetOrders();

        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        orderService.Verify(service => service.GetOrdersAsync(), Times.Once);
        orderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetOrderById_WhenServiceSucceeds_ReturnsOkWithOrder()
    {
        var orderId = Guid.CreateVersion7();
        var order = CreateOrderResponse(id: orderId, status: OrderStatus.Pending);

        var orderService = new Mock<IOrderService>(MockBehavior.Strict);
        orderService
            .Setup(service => service.GetOrderByIdAsync(orderId))
            .ReturnsAsync(Result<OrderResponse>.Success(order));

        var subject = new OrdersController(orderService.Object);

        var result = await subject.GetOrderById(orderId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Same(order, okResult.Value);
        orderService.Verify(service => service.GetOrderByIdAsync(orderId), Times.Once);
        orderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetOrderById_WhenServiceFails_ReturnsNotFound()
    {
        var orderId = Guid.CreateVersion7();

        var orderService = new Mock<IOrderService>(MockBehavior.Strict);
        orderService
            .Setup(service => service.GetOrderByIdAsync(orderId))
            .ReturnsAsync(Result<OrderResponse>.Failure(new Error(StatusCodes.Status404NotFound, "order not found")));

        var subject = new OrdersController(orderService.Object);

        var result = await subject.GetOrderById(orderId);

        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        orderService.Verify(service => service.GetOrderByIdAsync(orderId), Times.Once);
        orderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateOrder_WhenRequestIsNull_ReturnsBadRequestAndDoesNotCallService()
    {
        var orderService = new Mock<IOrderService>(MockBehavior.Strict);
        var subject = new OrdersController(orderService.Object);

        var result = await subject.CreateOrder(null);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Equal("At least one order line is required.", badRequestResult.Value);
        orderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateOrder_WhenOrderLinesIsNull_ReturnsBadRequestAndDoesNotCallService()
    {
        var request = CreateOrderRequest();
        request.OrderLines = null!;

        var orderService = new Mock<IOrderService>(MockBehavior.Strict);
        var subject = new OrdersController(orderService.Object);

        var result = await subject.CreateOrder(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Equal("At least one order line is required.", badRequestResult.Value);
        orderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateOrder_WhenOrderLinesIsEmpty_ReturnsBadRequestAndDoesNotCallService()
    {
        var request = CreateOrderRequest(orderLines: []);

        var orderService = new Mock<IOrderService>(MockBehavior.Strict);
        var subject = new OrdersController(orderService.Object);

        var result = await subject.CreateOrder(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Equal("At least one order line is required.", badRequestResult.Value);
        orderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateOrder_WhenServiceSucceeds_ReturnsCreatedAtGetOrderById()
    {
        var request = CreateOrderRequest();
        var order = CreateOrderResponse(status: OrderStatus.Pending);

        var orderService = new Mock<IOrderService>(MockBehavior.Strict);
        orderService
            .Setup(service => service.CreateOrderAsync(request))
            .ReturnsAsync(Result<OrderResponse>.Success(order));

        var subject = new OrdersController(orderService.Object);

        var result = await subject.CreateOrder(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        Assert.Equal(nameof(OrdersController.GetOrderById), createdResult.ActionName);
        Assert.Equal(order.Id, createdResult.RouteValues?["id"]);
        Assert.Same(order, createdResult.Value);
        orderService.Verify(service => service.CreateOrderAsync(request), Times.Once);
        orderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateOrder_WhenServiceFails_ReturnsBadRequestWithError()
    {
        var request = CreateOrderRequest();
        var error = new Error(StatusCodes.Status409Conflict, "stock not available");

        var orderService = new Mock<IOrderService>(MockBehavior.Strict);
        orderService
            .Setup(service => service.CreateOrderAsync(request))
            .ReturnsAsync(Result<OrderResponse>.Failure(error));

        var subject = new OrdersController(orderService.Object);

        var result = await subject.CreateOrder(request);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        Assert.Same(error, badRequestResult.Value);
        orderService.Verify(service => service.CreateOrderAsync(request), Times.Once);
        orderService.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(nameof(OrdersController.GetOrders), Constants.AdminPolicy)]
    [InlineData(nameof(OrdersController.GetOrderById), Constants.AdminOrUserPolicy)]
    [InlineData(nameof(OrdersController.CreateOrder), Constants.UserPolicy)]
    public void Action_HasExpectedAuthorizePolicy(string actionName, string expectedPolicy)
    {
        var action = typeof(OrdersController)
            .GetMethods()
            .Single(method => method.Name == actionName);

        var authorizeAttribute = Assert.Single(action.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false));
        var attribute = Assert.IsType<AuthorizeAttribute>(authorizeAttribute);
        Assert.Equal(expectedPolicy, attribute.Policy);
    }

    private static CreateOrderRequest CreateOrderRequest(List<CreateOrderLineRequest>? orderLines = null)
    {
        return new CreateOrderRequest
        {
            CustomerId = "customer-1",
            CustomerName = "Test Customer",
            CustomerPhone = "1234567890",
            CustomerEmail = "customer@example.com",
            ShippingAddress = "123 Test Street",
            OrderLines = orderLines ??
            [
                new CreateOrderLineRequest
                {
                    ProductId = "product-1",
                    ProductName = "Test Product",
                    Quantity = 2,
                    Price = 10.50m
                }
            ]
        };
    }

    private static OrderResponse CreateOrderResponse(Guid? id = null, OrderStatus status = OrderStatus.Pending)
    {
        return new OrderResponse
        {
            Id = id ?? Guid.CreateVersion7(),
            OrderNumber = "ORD-123",
            CustomerId = "customer-1",
            CustomerName = "Test Customer",
            CustomerPhone = "1234567890",
            CustomerEmail = "customer@example.com",
            ShippingAddress = "123 Test Street",
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OrderLines =
            [
                new OrderLineResponse
                {
                    Id = Guid.CreateVersion7(),
                    ProductId = "product-1",
                    ProductName = "Test Product",
                    Quantity = 2,
                    Price = 10.50m,
                    LineTotal = 21.00m
                }
            ],
            TotalAmount = 21.00m
        };
    }
}
