# Arxitektura Sənədi

## Ümumi baxış

ElectroShop **Clean Architecture** + **Domain-Driven Design (DDD)** + **CQRS** prinsiplərinə əsaslanır.

**Dependency qaydası:** Xarici layer daxili layer-ə asılıdır. Domain heç bir layera asılı deyil.

## Layer-lər

| Layer | Layihə | Məsuliyyət |
|-------|--------|------------|
| WebApi | ElectroShop.WebApi | Controllers, Middleware, Swagger, JWT |
| Application | ElectroShop.Application | CQRS handlers, DTOs, Validators, Services |
| Domain | ElectroShop.Domain | Entities, Value Objects, Domain Events |
| Persistence | ElectroShop.Persistence | EF Core, Repositories, Migrations |

## Design pattern-lər

- CQRS (MediatR), Result Pattern, Aggregate Root, Repository + UoW
- Optimistic Concurrency (RowVersion → xmin), Soft Delete (IsDeleted)
- Domain Events, Pipeline Behaviours (Logging → Validation)

## Middleware pipeline

UseExceptionHandling → Swagger → HTTPS → CORS → Auth → Controllers

## Backend komponentləri

**Controller-lər:** Auth, Products, Categories, Brands, Customers, Orders, Discounts, Dashboard, Images

**Service-lər:** TokenService, PasswordHasher, LocalImageStorage, DiscountCalculationService, LookupCacheInvalidator

**Repository-lər:** Product, Category, Brand, Customer, Order, Discount

**Background job-lar:** Yoxdur

**External:** PostgreSQL (aktiv), Lokal storage (aktiv)