# HelloWorld Frontend

Next.js frontend application for the HelloWorld User Management system.

## Setup

1. Install dependencies:
```bash
npm install
```

2. Create `.env.local` file with:
```
NEXT_PUBLIC_API_URL=http://localhost:7071/api/HelloWorld
```

3. Run development server:
```bash
npm run dev
```

4. Run tests:
```bash
npm test
```

## Deployment

Deploy to Vercel:
```bash
vercel --prod
```

Make sure to update the `NEXT_PUBLIC_API_URL` environment variable in Vercel with your production Azure Functions URL.

