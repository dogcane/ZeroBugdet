# GitHub Copilot Instructions

This project is a full-stack application with a .NET 9 Web API backend and Angular 19+ PWA frontend following Domain-Driven Design principles.

## Technology Stack

### Backend (.NET 9 API)
- **Framework**: .NET 9 Web API
- **Architecture**: Domain-Driven Design with Clean Architecture
- **ORM**: Entity Framework Core with PostgreSQL
- **GraphQL**: HotChocolate for GraphQL API
- **Mediator**: Wolverine for CQRS pattern
- **Validation**: Resulz for result pattern and error handling
- **Authentication**: ASP.NET Core Identity with JWT
- **Testing**: xUnit with TestContainers for integration tests
- **DDD Library**: ECO.Core for domain building blocks

### Frontend (Angular 19+ PWA)
- **Framework**: Angular 19+ with PWA support
- **Language**: TypeScript with strict mode
- **State Management**: NgRx (for complex state) or RxJS services
- **UI Components**: Angular Material or PrimeNG
- **GraphQL Client**: Apollo Angular for GraphQL operations
- **Architecture**: Feature modules with lazy loading

## Code Style and Patterns

### Backend Patterns
- Use **ECO.Core** base classes for entities, aggregates, and value objects
- Implement **CQRS** using Wolverine commands and queries
- Apply **Result pattern** with Resulz for all operations that can fail
- Use **Repository pattern** with Entity Framework repositories
- Follow **Clean Architecture** with proper dependency injection

### Frontend Patterns
- Use **OnPush change detection** strategy for components
- Implement **Smart/Dumb component** pattern
- Use **Reactive forms** for all user input
- Apply **RxJS operators** for data transformation
- Implement **standalone components** where appropriate (Angular 19+)

## Naming Conventions
- **C# Backend**: PascalCase for public, camelCase for private
- **TypeScript Frontend**: camelCase for variables/methods, PascalCase for classes
- **Database**: snake_case for tables and columns
- **API Endpoints**: kebab-case in URLs
- **Angular Components**: kebab-case for selectors

## Project Structure

### Backend Structure
```
src/
├── zerobudget.core/                                # "backend" root projects folder
│   ├── zerobudget.core.domain/                     # Entities, aggregates, domain services
│   ├── zerobudget.core.infrastructure/             # EF contexts, repositories, external  
│   ├── zerobudget.core.application/                # Application services, commands, queries
│   │   ├── zerobudget.core.application.commands/   # Wolverine command handlers
│   │   └── zerobudget.core.application.queries/    # Wolverine query handlers
│   ├── zerobudget.core.api/                        # controller apis, GraphQL setup
└── zerobudget.core.tests/
│   ├── zerobudget.core.domain.tests/
│   ├── zerobudget.core.infrastructure.tests/
│   └── zerobudget.core.application.tests/
```

### Frontend Structure
```
src/zerobudget.app/
├── core/               # Singleton services, guards, interceptors
├── shared/             # Shared components, pipes, directives
├── features/           # Feature modules (lazy loaded)
├── layout/             # Layout components
└── assets/             # Static assets
```

## Code Generation Guidelines

### When generating C# backend code:
1. **Always use Resulz** for methods that can fail:
   ```csharp
   public async Task<Result<User>> GetUserAsync(int id)
   {
       var user = await _repository.FindByIdAsync(id);
       return user is null 
           ? Result.Failure<User>("User not found")
           : Result.Success(user);
   }
   ```

2. **Use ECO.Core base classes** for domain entities:
   ```csharp
   public class User : AggregateRoot<UserId>
   {
       // Domain logic here
   }
   ```

3. **Implement Wolverine handlers** for commands and queries:
   ```csharp
   public class CreateUserHandler
   {
       public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand command)
       {
           // Handler implementation
       }
   }
   ```

4. **Use Entity Framework** with proper configuration:
   ```csharp
   public class ApplicationDbContext : DbContext
   {
       protected override void OnModelCreating(ModelBuilder builder)
       {
           builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
       }
   }
   ```

5. **Create HotChocolate GraphQL types** that map to domain entities:
   ```csharp
   public class UserType : ObjectType<User>
   {
       protected override void Configure(IObjectTypeDescriptor<User> descriptor)
       {
           descriptor.Field(u => u.Id).Type<NonNullType<IdType>>();
           descriptor.Field(u => u.Email).Type<NonNullType<StringType>>();
           descriptor.Field(u => u.CreatedAt).Type<NonNullType<DateTimeType>>();
       }
   }
   ```

6. **Implement GraphQL resolvers** using Wolverine handlers:
   ```csharp
   [ExtendObjectType<Query>]
   public class UserQueries
   {
       public async Task<Result<User>> GetUserAsync(
           int id,
           [Service] IMediator mediator)
       {
           return await mediator.SendAsync(new GetUserQuery(id));
       }
   }
   ```

7. **Create GraphQL mutations** for commands:
   ```csharp
   [ExtendObjectType<Mutation>]
   public class UserMutations
   {
       public async Task<Result<User>> CreateUserAsync(
           CreateUserInput input,
           [Service] IMediator mediator)
       {
           var command = new CreateUserCommand(input.Email, input.Password);
           return await mediator.SendAsync(command);
       }
   }
   ```

### When generating TypeScript frontend code:
1. **Use OnPush change detection**:
   ```typescript
   @Component({
     changeDetection: ChangeDetectionStrategy.OnPush,
     // component definition
   })
   ```

2. **Implement proper cleanup**:
   ```typescript
   export class MyComponent implements OnInit, OnDestroy {
     private destroy$ = new Subject<void>();
     
     ngOnDestroy(): void {
       this.destroy$.next();
       this.destroy$.complete();
     }
   }
   ```

3. **Use reactive forms**:
   ```typescript
   this.form = this.fb.group({
     email: ['', [Validators.required, Validators.email]],
     password: ['', [Validators.required, Validators.minLength(8)]]
   });
   ```

4. **Handle HTTP responses properly**:
   ```typescript
   this.userService.getUser(id).pipe(
     takeUntil(this.destroy$),
     catchError(error => this.handleError(error))
   ).subscribe(user => {
     // Handle success
   });
   ```

5. **Use Apollo Angular for GraphQL operations**:
   ```typescript
   // Query
   this.apollo.query<GetUserQuery>({
     query: GET_USER_QUERY,
     variables: { id }
   }).pipe(
     map(result => result.data.user),
     takeUntil(this.destroy$)
   ).subscribe(user => {
     // Handle user data
   });
   
   // Mutation
   this.apollo.mutate<CreateUserMutation>({
     mutation: CREATE_USER_MUTATION,
     variables: { input: createUserInput }
   }).subscribe(result => {
     // Handle mutation result
   });
   ```

6. **Define GraphQL operations with code generation**:
   ```typescript
   // Generated types from GraphQL schema
   export interface GetUserQuery {
     user: {
       id: string;
       email: string;
       createdAt: string;
     };
   }
   
   export interface CreateUserMutation {
     createUser: {
       id: string;
       email: string;
     };
   }
   ```

## GraphQL Guidelines

### HotChocolate Backend Implementation
- **Schema First Approach**: Define GraphQL schema using HotChocolate attributes and types
- **Integration with Wolverine**: Use Wolverine handlers inside GraphQL resolvers
- **Result Pattern**: Return Resulz Result<T> from resolvers and handle errors appropriately
- **Authentication**: Implement GraphQL authentication using ASP.NET Core Identity
- **DataLoader Pattern**: Use HotChocolate DataLoaders to prevent N+1 query problems

### GraphQL Schema Design
- Use meaningful names for types, fields, and operations
- Implement proper input validation using HotChocolate validators
- Design mutations to return meaningful response types
- Use GraphQL subscriptions for real-time features when needed
- Implement proper error handling with GraphQL error extensions

### Apollo Angular Frontend
- **Code Generation**: Use GraphQL Code Generator for TypeScript types and operations
- **Caching**: Configure Apollo Client cache policies appropriately
- **Error Handling**: Implement global error handling for GraphQL operations
- **Optimistic Updates**: Use optimistic updates for better UX in mutations
- **Subscriptions**: Implement GraphQL subscriptions for real-time features

### GraphQL Best Practices
- Keep resolvers thin - delegate to application services
- Use DataLoaders to batch database queries
- Implement proper pagination using cursor-based pagination
- Use GraphQL fragments to avoid query duplication
- Implement proper field-level authorization
- Monitor GraphQL query performance and complexity



### Backend Testing
- **Unit Tests**: Test domain logic and application services in isolation
- **Integration Tests**: Use TestContainers for PostgreSQL database tests
- **Architecture Tests**: Verify dependency rules and layer boundaries
- Mock external dependencies using interfaces

### Frontend Testing
- **Component Tests**: Test component behavior and templates
- **Service Tests**: Test business logic and HTTP interactions
- **E2E Tests**: Test critical user workflows
- Use Angular Testing utilities and Jasmine/Karma

## Security Practices

### Backend Security
- Always validate input using data annotations or FluentValidation
- Use parameterized queries (EF Core handles this automatically)
- Implement proper CORS policies
- Use JWT tokens with appropriate expiration
- Apply authorization attributes to controllers/actions

### Frontend Security
- Sanitize user input and output
- Store JWT tokens securely (httpOnly cookies preferred)
- Implement route guards for protected routes
- Use Angular's built-in XSS protection

## Database Conventions
- Use PostgreSQL naming conventions (snake_case)
- Implement proper foreign key relationships
- Add appropriate indexes for performance
- Use migrations for all schema changes
- Consider soft deletes for audit requirements

## API Design

### REST API Conventions
- Follow RESTful conventions for traditional endpoints
- Use appropriate HTTP status codes
- Implement consistent error response format
- Add OpenAPI/Swagger documentation
- Version APIs when breaking changes occur
- Implement pagination for list endpoints

### GraphQL API Design
- Design schema with clear, intuitive field names
- Use proper GraphQL scalar types (ID, String, Int, Boolean, DateTime)
- Implement input types for mutations with validation
- Use unions and interfaces for polymorphic data
- Design subscriptions for real-time features
- Implement proper error handling with extensions
- Use cursor-based pagination for large datasets
- Consider query complexity analysis and depth limiting

## Performance Guidelines
- Use async/await consistently
- Implement proper caching strategies
- Optimize database queries with includes and projections
- Use lazy loading for Angular modules
- Implement OnPush change detection
- Bundle optimization for production builds

## Error Handling
- Use Resulz Result<T> pattern for all operations that can fail
- Implement global exception handling middleware
- Provide meaningful error messages to users
- Log errors appropriately with structured logging
- Handle offline scenarios in PWA

## PWA Implementation
- Configure service worker for caching strategies
- Implement app shell architecture
- Handle online/offline state changes
- Add install prompts and update notifications
- Optimize for mobile and desktop experiences

When generating code, always consider these guidelines and patterns. Prioritize clean, maintainable code that follows the established architecture and conventions.