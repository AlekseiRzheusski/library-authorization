#migrations
dotnet ef migrations add InitialCreate --project LibraryAuthorization.Infrastructure/ --startup-project LibraryAuthorization.Api/

dotnet ef database update --project LibraryAuthorization.Infrastructure --startup-project LibraryAuthorization.Api --verbose
