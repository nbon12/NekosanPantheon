# HelloWorld Azure Functions Backend

Azure Functions (.NET 8, isolated worker model) backend with Cosmos DB integration using EF Core.

## Prerequisites

- .NET 8 SDK
- Azure Functions Core Tools
- Azure Cosmos DB Emulator (for local development)

## Setup

1. Install Azure Functions Core Tools:
```bash
npm install -g azure-functions-core-tools@4 --unsafe-perm true
```

2. Install Azure Cosmos DB Emulator:
   - Download from: https://aka.ms/cosmosdb-emulator
   - Or use Docker: `docker run -p 8081:8081 -p 10250-10255:10250-10255 mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator`

3. Restore dependencies:
```bash
cd backend
dotnet restore
```

4. Configure local settings:
   - The `local.settings.json` file is already configured for Cosmos DB Emulator
   - Update `COSMOS_ENDPOINT` and `COSMOS_KEY` if using a different Cosmos DB instance

5. Run the function locally:
```bash
func start
```

The function will be available at: `http://localhost:7071/api/HelloWorld`

## Testing

Run tests:
```bash
dotnet test
```

Run tests with coverage:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## API Endpoints

### GET /api/HelloWorld
Returns all users.

**Response:**
```json
[
  {
    "id": "guid",
    "name": "John Doe",
    "email": "john@example.com"
  }
]
```

### POST /api/HelloWorld
Creates a new user.

**Request Body:**
```json
{
  "name": "John Doe",
  "email": "john@example.com"
}
```

**Response:**
```json
{
  "id": "guid",
  "name": "John Doe",
  "email": "john@example.com"
}
```

## Deployment

1. Create an Azure Function App in Azure Portal
2. Configure Cosmos DB connection:
   - Set `COSMOS_ENDPOINT` in Application Settings
   - Set `COSMOS_KEY` in Application Settings
3. Deploy:
```bash
func azure functionapp publish YourFunctionAppName
```

## Project Structure

```
backend/
├── Models/
│   └── User.cs
├── Data/
│   └── AppDbContext.cs
├── Tests/
│   ├── TestHelpers/
│   └── HelloWorldFunctionTests.cs
├── HelloWorldFunction.cs
├── Program.cs
├── host.json
└── local.settings.json
```

