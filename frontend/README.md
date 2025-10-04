# Product Management Frontend

A modern React 18.2 + TypeScript 5.5 application built with Vite for product management.

## 🚀 Tech Stack

### Core Technologies
- **Frontend**: React 18.2 + TypeScript 5.5 (strict mode)
- **Build System**: Vite
- **State Management**: 
  - Zustand for client-side state
  - React Query (@tanstack/react-query) for server state
- **UI Framework**: Material UI
- **Routing**: React Router v7
- **Internationalization**: react-i18next
- **Validation**: Zod for schema validation

### Architecture Patterns
- Single Page Application (SPA)
- Domain-driven API organization
- Component-first architecture with co-located tests
- Provider pattern for cross-cutting concerns
- Custom hooks for reusable business logic

## 📁 Project Structure

```
src/
├── api/                    # Domain-driven API organization
│   ├── index.ts           # Central API configuration
│   └── ProductApi/        # Product-specific endpoints
├── components/            # Reusable UI components
├── hooks/                # Custom React hooks
├── pages/                # Route-level components
├── providers/            # Context providers
├── states/               # State management (Zustand stores)
├── types/                # TypeScript type definitions
├── utils/                # Utility functions
└── test/                 # Test setup and utilities
```

## 🛠️ Development Setup

### Prerequisites
- Node.js 18+ and npm
- Backend API running on http://localhost:7071

### Installation

1. **Install dependencies**:
   ```bash
   npm install
   ```

2. **Set up environment variables**:
   ```bash
   cp .env.example .env
   ```
   
   Edit `.env` to configure your API endpoint:
   ```
   VITE_API_BASE_URL=http://localhost:7071/api
   VITE_DEV_MODE=true
   ```

3. **Start the development server**:
   ```bash
   npm run dev
   ```

   The application will be available at http://localhost:3000

## 🧪 Testing

### Run Tests
```bash
# Run all tests
npm test

# Run tests with coverage
npm run test:coverage

# Run tests with UI
npm run test:ui
```

## 🏗️ Build & Deployment

### Development Build
```bash
npm run build
```

### Preview Build
```bash
npm run preview
```

### Type Checking
```bash
npm run type-check
```

### Linting
```bash
npm run lint
```

## 🌐 Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `VITE_API_BASE_URL` | Backend API base URL | `http://localhost:7071/api` |
| `VITE_DEV_MODE` | Enable development features | `true` |

## 📚 Available Scripts

| Script | Description |
|--------|-------------|
| `npm run dev` | Start development server |
| `npm run build` | Build for production |
| `npm run preview` | Preview production build |
| `npm test` | Run tests |
| `npm run test:coverage` | Run tests with coverage |
| `npm run lint` | Run ESLint |
| `npm run type-check` | Run TypeScript type checking |

## 🚀 Next Steps (Future Epics)

- [ ] Product List & Pagination UI (Epic 6)
- [ ] Search UI & Integration (Epic 7) 
- [ ] CRUD Operations in Frontend (Epic 8)
- [ ] Advanced Testing & Quality Assurance (Epic 9)
- [ ] Enhanced Documentation & Developer Experience (Epic 10)
