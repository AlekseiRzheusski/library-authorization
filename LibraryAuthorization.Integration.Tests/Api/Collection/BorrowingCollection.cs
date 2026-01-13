
using LibraryAuthorization.Integration.Tests.Fixtures;

namespace LibraryAuthorization.Integration.Tests.Api.Collection;

[CollectionDefinition("Borrowing collection")]
public class DatabaseCollection : ICollectionFixture<LibraryAuthorizationApiFactory> {}
