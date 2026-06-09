using Shared.Common;
using Shared.Mappings;
using Shared.Requests;
using Shared.Responses;
using Stocks.Api.Interfaces;
using System.Net;

namespace Stocks.Api.Services;

public sealed class StocksService(IStocksRepository stocksRepository, ILogger<StocksService> logger) : IStocksService
{
    public async Task<Result<IReadOnlyCollection<StockResponse>>> GetStocksAsync()
    {
        var stocks = await stocksRepository.GetStocksAsync();
        if (!stocks.Any())
        {
            logger.LogInformation("No stocks found in the database.");
            return Result<IReadOnlyCollection<StockResponse>>.Failure(new Error((int)HttpStatusCode.NotFound, "No stocks found!"));
        }

        return Result<IReadOnlyCollection<StockResponse>>.Success(stocks.Select(stock => stock.ToResponse()).ToList());
    }

    public async Task<Result<StockResponse>> GetStockByProductIdAsync(string productId)
    {
        var stock = await stocksRepository.GetStockByProductIdAsync(productId);
        if (stock is null)
        {
            logger.LogInformation("Stock not found for product id: {ProductId}", productId);
            return Result<StockResponse>.Failure(new Error((int)HttpStatusCode.NotFound, "Stock not found!"));
        }

        return Result<StockResponse>.Success(stock.ToResponse());
    }

    public async Task<Result<StockResponse>> CreateStockAsync(CreateStockRequest request)
    {
        var existingStock = await stocksRepository.GetStockByProductIdAsync(request.ProductId);
        if (existingStock is not null)
        {
            logger.LogWarning("Stock already exists for product id: {ProductId}", request.ProductId);
            return Result<StockResponse>.Failure(new Error((int)HttpStatusCode.BadRequest, "Stock already exists for this product!"));
        }

        var stock = await stocksRepository.CreateStockAsync(request.ToEntity());
        return Result<StockResponse>.Success(stock.ToResponse());
    }

    public async Task<Result<StockResponse>> UpdateStockQuantityAsync(string productId, UpdateStockRequest request)
    {
        var stock = await stocksRepository.UpdateStockQuantityAsync(productId, request.QuantityAvailable);
        if (stock is null)
        {
            logger.LogInformation("Stock not found for quantity update. Product id: {ProductId}", productId);
            return Result<StockResponse>.Failure(new Error((int)HttpStatusCode.NotFound, "Stock not found!"));
        }

        return Result<StockResponse>.Success(stock.ToResponse());
    }

    public async Task<Result<bool>> HasAvailableStockAsync(string productId, int quantity)
    {
        var stock = await stocksRepository.GetStockByProductIdAsync(productId);
        if (stock is null)
        {
            logger.LogInformation("Stock not found for availability check. Product id: {ProductId}", productId);
            return Result<bool>.Failure(new Error((int)HttpStatusCode.NotFound, "Stock not found!"));
        }

        return Result<bool>.Success(stock.QuantityAvailable >= quantity);
    }

    public async Task<Result<StockReservationResponse>> ReserveStocksAsync(IEnumerable<ReservationItem> reservationItems)
    {
        var result = await stocksRepository.ReserveStocksAsync(reservationItems);
        if (!result)
        {
            logger.LogWarning("One or more stock reservations could not be processed.");
            return Result<StockReservationResponse>.Failure(new Error((int)HttpStatusCode.Conflict, "One or more reservations could not be processed."));
        }

        return Result<StockReservationResponse>.Success(new StockReservationResponse
        {
            Success = true,
            Message = "Stocks reserved successfully."
        });
    }
}
