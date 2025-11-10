# Setup Instructions

## Prerequisites

1. **.NET 8 SDK**
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verify: `dotnet --version`

2. **Node.js 18.x**
   - Download from: https://nodejs.org/
   - Verify: `node --version`

3. **Azure Functions Core Tools**
   ```bash
   npm install -g azure-functions-core-tools@4 --unsafe-perm true
   ```
   - Verify: `func --version`

4. **Azure Cosmos DB Emulator** (for local development)
   - Option 1: Download from https://aka.ms/cosmosdb-emulator
   - Option 2: Use Docker:
     ```bash
     docker run -p 8081:8081 -p 10250-10255:10250-10255 mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator
     ```

## Backend Setup

1. Navigate to backend directory:
   ```bash
   cd backend
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Start Cosmos DB Emulator (if not already running)

4. Run tests:
   ```bash
   dotnet test
   ```

5. Start the function locally:
   ```bash
   func start
   ```

   The function will be available at: `http://localhost:7071/api/HelloWorld`

## Frontend Setup

1. Navigate to frontend directory:
   ```bash
   cd frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Create `.env.local` file:
   ```bash
   cp .env.example .env.local
   # Or create manually with: NEXT_PUBLIC_API_URL=http://localhost:7071/api/HelloWorld
   ```

4. Run tests:
   ```bash
   npm test
   ```

5. Start development server:
   ```bash
   npm run dev
   ```

   The frontend will be available at: `http://localhost:3000`

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

### Run All Tests
```bash
# Backend
cd backend && dotnet test && cd ..

# Frontend
cd frontend && npm test && cd ..
```

## Troubleshooting

### Cosmos DB Emulator Connection Issues
- Ensure the emulator is running on port 8081
- Check that `local.settings.json` has the correct endpoint and key
- For Docker: Ensure ports 8081 and 10250-10255 are not in use

### Azure Functions Not Starting
- Verify Azure Functions Core Tools is installed: `func --version`
- Check that port 7071 is available
- Verify `local.settings.json` exists and is properly formatted

### Frontend API Connection Issues
- Verify backend is running on `http://localhost:7071`
- Check `.env.local` has correct `NEXT_PUBLIC_API_URL`
- Check browser console for CORS errors (may need CORS configuration in backend)

## Next Steps

1. Run both backend and frontend
2. Test the application by adding users through the UI
3. Verify data is stored in Cosmos DB
4. Run all tests to ensure everything works
5. Deploy to Azure (backend) and Vercel (frontend)

