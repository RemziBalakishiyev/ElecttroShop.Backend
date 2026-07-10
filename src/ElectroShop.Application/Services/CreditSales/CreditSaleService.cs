using ElectroShop.Application.Abstractions;

using ElectroShop.Application.Common.Results;

using ElectroShop.Application.DTOs;

using ElectroShop.Application.Features.CreditSales.Common;

using ElectroShop.Domain.Entities;

using ElectroShop.Domain.Enums;



namespace ElectroShop.Application.Services.CreditSales;



public class CreditSaleService : ICreditSaleService

{

    private readonly IWriteRepository<CreditSale> _creditSaleWriteRepository;

    private readonly ICreditSaleQueryRepository _creditSaleQueryRepository;

    private readonly IWriteRepository<Sale> _saleWriteRepository;

    private readonly IProductQueryRepository _productQueryRepository;

    private readonly IWriteRepository<Product> _productWriteRepository;

    private readonly IUnitOfWork _unitOfWork;



    public CreditSaleService(

        IWriteRepository<CreditSale> creditSaleWriteRepository,

        ICreditSaleQueryRepository creditSaleQueryRepository,

        IWriteRepository<Sale> saleWriteRepository,

        IProductQueryRepository productQueryRepository,

        IWriteRepository<Product> productWriteRepository,

        IUnitOfWork unitOfWork)

    {

        _creditSaleWriteRepository = creditSaleWriteRepository;

        _creditSaleQueryRepository = creditSaleQueryRepository;

        _saleWriteRepository = saleWriteRepository;

        _productQueryRepository = productQueryRepository;

        _productWriteRepository = productWriteRepository;

        _unitOfWork = unitOfWork;

    }



    public async Task<Result<CreditSaleDetailDto>> CreateAsync(

        CreateCreditSaleRequest request,

        CancellationToken cancellationToken = default)

    {

        CreditSale creditSale;

        Product? product = null;



        try

        {

            if (request.ProductSourceType == CreditSaleProductSource.SystemProduct)

            {

                if (!request.ProductId.HasValue)

                {

                    return Result.Failure<CreditSaleDetailDto>(

                        Error.Validation("CreditSale.ProductIdRequired", "Sistem məhsulu üçün ProductId tələb olunur"));

                }



                product = await _productQueryRepository.GetProductWithDetailsAsync(

                    request.ProductId.Value,

                    cancellationToken);



                if (product is null)

                    return Result.Failure<CreditSaleDetailDto>(DomainErrors.Product.NotFound(request.ProductId.Value));



                if (product.Stock < request.Quantity)

                    return Result.Failure<CreditSaleDetailDto>(DomainErrors.Product.OutOfStock);



                var costPrice = request.CostPrice > 0 ? request.CostPrice : product.Price.Amount;

                var salePrice = request.SalePrice > 0 ? request.SalePrice : product.Price.Amount;



                creditSale = CreditSale.CreateFromSystemProduct(

                    request.CustomerName,

                    request.CustomerPhone,

                    product.Id,

                    product.Name,

                    product.Sku.Value,

                    product.CategoryId,

                    product.Category?.Name,

                    costPrice,

                    salePrice,

                    request.Quantity,

                    request.CreditDate,

                    request.DueDate,

                    request.Note);

            }

            else

            {

                creditSale = CreditSale.CreateManual(

                    request.CustomerName,

                    request.CustomerPhone,

                    request.ProductName!,

                    request.Sku,

                    request.CostPrice,

                    request.SalePrice,

                    request.Quantity,

                    request.CreditDate,

                    request.DueDate,

                    request.Note);

            }



            var expenseDrafts = CreditSaleMapper.ToExpenseDrafts(request.Expenses);

            if (expenseDrafts.Count > 0)

                creditSale.SetExpenses(expenseDrafts);

        }

        catch (ArgumentException ex)

        {

            return Result.Failure<CreditSaleDetailDto>(Error.Validation("CreditSale.InvalidData", ex.Message));

        }



        await _unitOfWork.BeginTransactionAsync(cancellationToken);



        try

        {

            await _creditSaleWriteRepository.AddAsync(creditSale, cancellationToken);



            if (product is not null)

            {

                product.DecreaseStock(request.Quantity);

                if (product.Stock == 0)

                    product.Deactivate();



                _productWriteRepository.Update(product);

            }



            await _unitOfWork.PrepareCreditSaleForSaveAsync(creditSale.Id, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

        }

        catch

        {

            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            throw;

        }



        var detail = await _creditSaleQueryRepository.GetCreditSaleByIdAsync(creditSale.Id, cancellationToken);

        return Result.Success(CreditSaleMapper.ToDetailDto(detail!));

    }



    public async Task<Result<CreditSaleDetailDto>> UpdateAsync(

        Guid id,

        UpdateCreditSaleRequest request,

        CancellationToken cancellationToken = default)

    {

        var creditSale = await _creditSaleQueryRepository.GetCreditSaleWithExpensesForUpdateAsync(id, cancellationToken);

        if (creditSale is null)

            return Result.Failure<CreditSaleDetailDto>(DomainErrors.CreditSale.NotFound(id));



        if (creditSale.Status != CreditSaleStatus.Pending)

            return Result.Failure<CreditSaleDetailDto>(DomainErrors.CreditSale.CannotEdit);



        var previousQuantity = creditSale.Quantity;

        Product? product = null;



        try

        {

            if (creditSale.ProductSource == CreditSaleProductSource.SystemProduct)

            {

                if (!creditSale.ProductId.HasValue)

                {

                    return Result.Failure<CreditSaleDetailDto>(

                        Error.Failure("CreditSale.InvalidState", "Sistem məhsulu nisyəsində ProductId tapılmadı"));

                }



                product = await _productQueryRepository.GetByIdAsync(creditSale.ProductId.Value, cancellationToken);

                if (product is null)

                    return Result.Failure<CreditSaleDetailDto>(DomainErrors.Product.NotFound(creditSale.ProductId.Value));



                var quantityDelta = request.Quantity - previousQuantity;

                if (quantityDelta > 0 && product.Stock < quantityDelta)

                    return Result.Failure<CreditSaleDetailDto>(DomainErrors.Product.OutOfStock);

            }



            creditSale.UpdatePending(

                request.CustomerName,

                request.CustomerPhone,

                request.CostPrice,

                request.SalePrice,

                request.Quantity,

                request.CreditDate,

                request.DueDate,

                request.Note);



            if (request.Expenses is not null)

            {

                var expenseDrafts = CreditSaleMapper.ToExpenseDrafts(request.Expenses);

                creditSale.ReplaceExpenses(expenseDrafts);

            }

        }

        catch (ArgumentException ex)

        {

            return Result.Failure<CreditSaleDetailDto>(Error.Validation("CreditSale.InvalidData", ex.Message));

        }

        catch (InvalidOperationException ex)

        {

            return Result.Failure<CreditSaleDetailDto>(Error.Validation("CreditSale.InvalidOperation", ex.Message));

        }



        await _unitOfWork.BeginTransactionAsync(cancellationToken);



        try

        {

            if (product is not null)

            {

                var quantityDelta = request.Quantity - previousQuantity;

                if (quantityDelta > 0)

                {

                    product.DecreaseStock(quantityDelta);

                    if (product.Stock == 0)

                        product.Deactivate();

                }

                else if (quantityDelta < 0)

                {

                    product.IncreaseStock(-quantityDelta);

                    if (product.Stock > 0 && !product.IsActive)

                        product.Activate();

                }



                _productWriteRepository.Update(product);

            }



            await _unitOfWork.PrepareCreditSaleForSaveAsync(creditSale.Id, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

        }

        catch

        {

            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            throw;

        }



        var detail = await _creditSaleQueryRepository.GetCreditSaleByIdAsync(id, cancellationToken);

        return Result.Success(CreditSaleMapper.ToDetailDto(detail!));

    }



    public async Task<Result> CancelAsync(Guid id, CancellationToken cancellationToken = default)

    {

        var creditSale = await _creditSaleQueryRepository.GetCreditSaleForUpdateAsync(id, cancellationToken);

        if (creditSale is null)

            return Result.Failure(DomainErrors.CreditSale.NotFound(id));



        if (creditSale.Status == CreditSaleStatus.Sold)

            return Result.Failure(DomainErrors.CreditSale.CannotCancel);



        if (creditSale.Status == CreditSaleStatus.Cancelled)

            return Result.Success();



        try

        {

            creditSale.Cancel();

        }

        catch (InvalidOperationException ex)

        {

            return Result.Failure(Error.Validation("CreditSale.InvalidOperation", ex.Message));

        }



        await _unitOfWork.BeginTransactionAsync(cancellationToken);



        try

        {

            if (creditSale.ProductSource == CreditSaleProductSource.SystemProduct && creditSale.ProductId.HasValue)

            {

                var product = await _productQueryRepository.GetByIdAsync(creditSale.ProductId.Value, cancellationToken);

                if (product is not null)

                {

                    product.IncreaseStock(creditSale.Quantity);

                    if (product.Stock > 0 && !product.IsActive)

                        product.Activate();



                    _productWriteRepository.Update(product);

                }

            }



            await _unitOfWork.CommitTransactionAsync(cancellationToken);

        }

        catch

        {

            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            throw;

        }



        return Result.Success();

    }



    public async Task<Result<CreditSaleDetailDto>> MarkAsSoldAsync(

        Guid id,

        DateTime? soldDate,

        CancellationToken cancellationToken = default)

    {

        var creditSale = await _creditSaleQueryRepository.GetCreditSaleWithExpensesForUpdateAsync(id, cancellationToken);

        if (creditSale is null)

            return Result.Failure<CreditSaleDetailDto>(DomainErrors.CreditSale.NotFound(id));



        if (creditSale.Status == CreditSaleStatus.Sold)

            return Result.Failure<CreditSaleDetailDto>(DomainErrors.CreditSale.AlreadySold);



        if (creditSale.Status == CreditSaleStatus.Cancelled)

            return Result.Failure<CreditSaleDetailDto>(DomainErrors.CreditSale.InvalidStatus);



        var soldAt = soldDate ?? DateTime.UtcNow;

        var note = BuildConvertedSaleNote(creditSale.Note);



        Sale sale;

        try

        {

            sale = creditSale.ProductSource == CreditSaleProductSource.SystemProduct

                ? Sale.CreateFromCreditSaleExistingProduct(

                    creditSale.Id,

                    creditSale.ProductId!.Value,

                    creditSale.ProductName,

                    creditSale.ProductCode,

                    creditSale.CategoryId,

                    creditSale.CategoryName,

                    creditSale.CostPrice,

                    creditSale.SalePrice,

                    creditSale.Quantity,

                    soldAt,

                    note)

                : Sale.CreateFromCreditSaleManualEntry(

                    creditSale.Id,

                    creditSale.ProductName,

                    creditSale.ProductCode,

                    null,

                    null,

                    creditSale.CostPrice,

                    creditSale.SalePrice,

                    creditSale.Quantity,

                    soldAt,

                    note);



            var expenseDrafts = creditSale.Expenses

                .Where(e => !e.IsDeleted)

                .Select(e => new SaleExpenseDraft(e.ExpenseType, e.Amount, e.Description))

                .ToList();



            if (expenseDrafts.Count > 0)

                sale.SetExpenses(expenseDrafts);

        }

        catch (ArgumentException ex)

        {

            return Result.Failure<CreditSaleDetailDto>(Error.Validation("CreditSale.InvalidData", ex.Message));

        }

        catch (InvalidOperationException ex)

        {

            return Result.Failure<CreditSaleDetailDto>(Error.Validation("CreditSale.InvalidOperation", ex.Message));

        }



        await _unitOfWork.BeginTransactionAsync(cancellationToken);



        try

        {

            await _saleWriteRepository.AddAsync(sale, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);



            creditSale.MarkAsSold(sale.Id, DateTime.UtcNow);

            await _unitOfWork.PrepareSaleForSaveAsync(sale.Id, cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

        }

        catch

        {

            await _unitOfWork.RollbackTransactionAsync(cancellationToken);

            throw;

        }



        var detail = await _creditSaleQueryRepository.GetCreditSaleByIdAsync(id, cancellationToken);

        return Result.Success(CreditSaleMapper.ToDetailDto(detail!));

    }



    private static string BuildConvertedSaleNote(string? originalNote)

    {

        const string prefix = "Nisyədən satışa çevrildi";

        return string.IsNullOrWhiteSpace(originalNote)

            ? prefix

            : $"{prefix}. {originalNote.Trim()}";

    }

}


