#migrations
dotnet ef migrations add InitialCreate --project LibraryAuthorization.Infrastructure/ --startup-project LibraryAuthorization.Api/

dotnet ef database update --project LibraryAuthorization.Infrastructure --startup-project LibraryAuthorization.Api --verbose

#generate jwt key
openssl rand -base64 32

#start redis
docker run -d --name redis -p 6379:6379 redis:7
