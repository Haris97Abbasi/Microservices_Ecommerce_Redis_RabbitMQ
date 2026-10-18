# SmartStore Microservices – Execution Plan

## Context
The brief (`SmartStore_Microservices_Project_Brief.pdf`) asks for a small, API-only .NET 8 microservices demo modeled on the AirlineBookingSystem layout: 4 services (Catalog/MongoDB, Orders/Redis, Payments/PostgreSQL, Notifications/SQL Server), MassTransit + RabbitMQ events, CQRS with MediatR, an AI product assistant inside Catalog, and Docker Compose for local run. The directory is currently empty apart from the PDF. Scope is demo MVP: keep it simple.

## Plan

1. **Solution skeleton**: Create `SmartStore.sln` with `src/BuildingBlocks`, `src/Services/{Catalog,Orders,Payments,Notifications}/` (each with `*.Api`, `*.Application`, `*.Core`, `*.Infrastructure` projects) and a `tests/` folder. Wire project references (Api → Application → Core; Infrastructure → Core).

2. **BuildingBlocks**: Shared event contracts `OrderPlaced` (OrderId, ProductId, Quantity, Amount) and `PaymentProcessed` (OrderId, PaymentId, Success, Amount), plus one small extension method to register MassTransit with RabbitMQ from configuration.

3. **Local infrastructure via Docker Compose**: `docker-compose.yml` with MongoDB, Redis (AOF persistence on), PostgreSQL, SQL Server and RabbitMQ (management UI). Add `.env.example` holding ports, connection strings, credentials and LLM settings.

4. **Catalog – Core & Infrastructure**: `Product` entity, `IProductRepository`, MongoDB implementation, and startup seed data (a handful of sample products).

5. **Catalog – Application & Api**: MediatR commands/queries for product CRUD, `ProductsController`, Swagger, environment-based config.

6. **Catalog – AI assistant**: `ILlmClient` interface (Core) with one OpenAI-compatible HTTP implementation (Infrastructure) configured by `Llm:BaseUrl / ApiKey / Model` – works with OpenAI, Azure OpenAI-compatible endpoints or local Ollama. `POST /catalog/assistant` query handler loads products as read-only context, builds the prompt, and returns the answer.

7. **Orders – Core & Infrastructure**: `Order` entity (Id, ProductId, Quantity, Total, Status: Pending/Paid), `IOrderRepository` with Redis (StackExchange.Redis, JSON) implementation, and a typed `HttpClient` for the Catalog API.

8. **Orders – Application & Api**: `CreateOrderCommand` (validate product via Catalog synchronously → save Pending order → publish `OrderPlaced`), `GetOrderByIdQuery`; `POST /orders`, `GET /orders/{id}`.

9. **Payments service**: `Payment` entity, EF Core + Npgsql repository (`EnsureCreated` on startup – no migrations), `OrderPlacedConsumer` that simulates a successful payment, saves it and publishes `PaymentProcessed`; `GET /payments/{orderId}`.

10. **Orders – PaymentProcessed consumer**: Update the order status in Redis to `Paid`.

11. **Notifications service**: `Notification` entity, EF Core + SQL Server repository (`EnsureCreated`), `PaymentProcessedConsumer` inserts a notification record (no real email/SMS); `GET /notifications` (optionally filter by orderId).

12. **Containerize the APIs**: One Dockerfile per API and add the 4 APIs to `docker-compose.yml` with environment variables for connection strings, RabbitMQ host, Catalog URL and LLM settings.

13. **Basic tests**: A single xUnit test project (Moq) covering key handlers: create product, create order (product found / not found), payment consumer publishes `PaymentProcessed`, order marked Paid.

14. **API examples**: `docs/requests.http` / curl script and a small Postman collection covering the full demo flow.

15. **README**: Architecture overview, project layout, how to run (`docker compose up`), configuring the LLM, and the demo walkthrough.

16. **End-to-end verification**: `docker compose up --build`, then run the demo: create product → place order → check order becomes Paid → check payment record → check notification → ask the AI assistant about products. Run `dotnet test`.

## Verification
- `dotnet build` and `dotnet test` pass.
- All Swagger UIs load; RabbitMQ UI shows the `OrderPlaced` / `PaymentProcessed` exchanges.
- Demo flow in step 16 works end to end via curl/Postman.
