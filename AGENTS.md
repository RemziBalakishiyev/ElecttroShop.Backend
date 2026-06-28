# ElectroShop Backend Agent Instructions

This repository is the ElectroShop Backend only.

## Core Rules

- Work only inside this backend repository.
- Do not generate Admin Front or User Front code.
- Follow existing .NET backend architecture.
- Keep controllers thin.
- Put business logic in service/application layer.
- Use DTOs for request and response models.
- Validate inputs.
- Use async/await for database operations.
- Do not change authentication or authorization unless explicitly required.
- Do not change database schema unless explicitly required.
- Do not make unrelated refactors.
- Do not add NuGet packages unless clearly justified.
- Run dotnet build before finishing if possible.
- Report changed files, risks, and manual test steps.

## Safety

- Do not touch secrets.
- Do not expose internal exception details.
- Do not log sensitive data.
- Do not create breaking API changes unless explicitly required.