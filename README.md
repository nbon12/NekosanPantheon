# HelloWorld Full Stack Application

A full-stack web application with Azure Functions backend (.NET 8) and Next.js frontend, using Cosmos DB for data storage.

## Architecture

- **Backend**: Azure Functions (.NET 8, isolated worker model) with EF Core and Cosmos DB
- **Frontend**: Next.js/React (TypeScript) deployed to Vercel
- **Database**: Azure Cosmos DB
- **Testing**: TDD approach with unit, integration, and API tests

## Project Structure

```
.
├── backend/          # Azure Functions backend
├── frontend/         # Next.js frontend
└── .github/          # CI/CD workflows
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 18.x
- Azure Functions Core Tools
- Azure Cosmos DB Emulator (for local development)

### Backend Setup

1. Navigate to backend directory:
```bash
cd backend
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Start Cosmos DB Emulator (or configure connection to Cosmos DB)

4. Run the function:
```bash
func start
```

See [backend/README.md](backend/README.md) for more details.

### Frontend Setup

1. Navigate to frontend directory:
```bash
cd frontend
```

2. Install dependencies:
```bash
npm install
```

3. Create `.env.local`:
```
NEXT_PUBLIC_API_URL=http://localhost:7071/api/HelloWorld
```

4. Run development server:
```bash
npm run dev
```

See [frontend/README.md](frontend/README.md) for more details.

## Testing

### Backend Tests
```bash
cd backend
dotnet test
```

### Frontend Tests
```bash
cd frontend
npm test
```

## Deployment

### Backend
1. Create Azure Function App in Azure Portal
2. Configure Cosmos DB connection strings
3. Deploy:
```bash
func azure functionapp publish YourFunctionAppName
```

### Frontend
1. Deploy to Vercel:
```bash
cd frontend
vercel --prod
```

2. Update `NEXT_PUBLIC_API_URL` environment variable in Vercel with production backend URL

## CI/CD

GitHub Actions workflows are configured for:
- Backend: Build and test on push/PR
- Frontend: Lint, test, and build on push/PR

## Development Workflow

This project follows TDD (Test-Driven Development):
1. Write tests first
2. Run tests (they should fail)
3. Implement code to pass tests
4. Refactor if needed

## License

MIT

