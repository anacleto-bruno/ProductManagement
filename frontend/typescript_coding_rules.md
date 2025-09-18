# Claude Coding Agent Rules for {Project} UI

## **Architecture & Technology Stack**

### **Core Technologies**
- **Frontend**: React 18.2 + TypeScript 5.5 (strict mode)
- **Build System**: Vite with Module Federation
- **State Management**: 
  - Zustand for client-side state
  - React Query (@tanstack/react-query) for server state
- **UI Framework**: shadcn
- **Routing**: React Router v7
- **Internationalization**: react-i18next
- **Validation**: Zod for schema validation

### **Key Architectural Patterns**
- Single Page Application (SPA)
- Domain-driven API organization
- Component-first architecture with co-located tests
- Provider pattern for cross-cutting concerns
- Custom hooks for reusable business logic

---

## **Project Structure & Organization**

### **Directory Structure Rules**
```
src/
├── api/                    # Domain-driven API organization
│   ├── index.ts           # Central API exports
│   ├── Service1Api/      # Service1-specific endpoints
│   ├── Service2Api/       # Service2-specific  endpoints
├── components/            # Reusable UI components
├── hooks/                # Custom React hooks
├── pages/                # Route-level components
├── providers/            # Context providers
├── states/               # State management (Zustand stores)
├── types/                # TypeScript type definitions
└── utils/                # Utility functions
```

### **File Naming Conventions**
- **Components**: PascalCase (`ProductTable.tsx`)
- **Hooks**: camelCase with `use` prefix (`useProductColumns.ts`)
- **Utilities**: camelCase (`common.ts`)
- **Types**: camelCase with `.d.ts` extension (`conexiom-ui.d.ts`)
- **Tests**: Same as source file + `.test.ts/.tsx`

### **Import Organization**
1. External libraries (React, shadcn, etc.)
2. Internal API imports
3. Component imports
4. Hook imports
5. Type imports
6. Utility imports

Use path aliases: `~/` for clean imports from `src/`

---

## **Development Practices**

### **TypeScript Rules**
- **Strict Mode**: Always enabled
- **Type Safety**: Prefer explicit types over `any`
- **Interface vs Type**: Use `interface` for object shapes, `type` for unions/primitives
- **Generic Constraints**: Use proper generic constraints for reusable components
- **Zod Integration**: Use Zod schemas for runtime validation and type inference

```typescript
// Good: Explicit typing with Zod
const UserSchema = z.object({
  id: z.string(),
  name: z.string(),
  email: z.string().email(),
})
type User = z.infer<typeof UserSchema>

// Bad: Using any
const user: any = getData()
```

### **React Component Guidelines**

#### **Component Structure**
1. Imports (grouped and sorted)
2. Type definitions
3. Component logic
4. Export statement

```typescript
// Preferred structure
import React from 'react'
import { Box, Typography } from '@mui/material'
import type { ComponentProps } from '~/types'

interface ProductTableProps {
  data: TableData[]
  onRowClick: (id: string) => void
}

export const ProductTable: React.FC<ProductTableProps> = ({ 
  data, 
  onRowClick 
}) => {
  // Component logic here
  return (
    <Box>
      {/* JSX here */}
    </Box>
  )
}
```

#### **State Management Patterns**
- **Local State**: `useState` for component-specific state
- **Global Client State**: Zustand stores
- **Server State**: React Query hooks
- **Form State**: shadcn form components or react-hook-form

```typescript
// Zustand store pattern
interface AppState {
  user: User | null
  theme: 'light' | 'dark'
  setUser: (user: User | null) => void
  toggleTheme: () => void
}

export const useAppState = create<AppState>((set) => ({
  user: null,
  theme: 'light',
  setUser: (user) => set({ user }),
  toggleTheme: () => set((state) => ({ 
    theme: state.theme === 'light' ? 'dark' : 'light' 
  })),
}))
```

#### **Advanced State Management Patterns**

##### **React Query Mutations for Server State**
- **Optimistic Updates**: Implement optimistic updates for better UX
- **Mutation Error Handling**: Provide rollback mechanisms for failed mutations
- **Cache Invalidation**: Strategic cache invalidation after mutations
- **Loading States**: Granular loading states for different mutation types

```typescript
// ✅ React Query mutation with optimistic updates
export const useUpdateUser = () => {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: updateUser,
    onMutate: async (newUser) => {
      // Cancel outgoing refetches
      await queryClient.cancelQueries({ queryKey: ['users', newUser.id] })
      
      // Snapshot previous value
      const previousUser = queryClient.getQueryData(['users', newUser.id])
      
      // Optimistically update
      queryClient.setQueryData(['users', newUser.id], newUser)
      
      return { previousUser }
    },
    onError: (err, newUser, context) => {
      // Rollback on error
      queryClient.setQueryData(['users', newUser.id], context?.previousUser)
    },
    onSettled: (data, error, variables) => {
      // Always refetch after error or success
      queryClient.invalidateQueries({ queryKey: ['users', variables.id] })
    },
  })
}
```

##### **Complex State Scenarios with Redux Toolkit**
- **When to Use**: Complex state interdependencies, time-travel debugging needs
- **RTK Query**: For advanced caching and server state management
- **Slice Pattern**: Organize state by feature domains
- **DevTools Integration**: Leverage Redux DevTools for debugging

```typescript
// ✅ RTK Slice for complex state management
const userSlice = createSlice({
  name: 'users',
  initialState: {
    entities: {},
    ids: [],
    loading: false,
    error: null,
    filters: {},
    pagination: { page: 1, limit: 20 },
  },
  reducers: {
    setLoading: (state, action) => {
      state.loading = action.payload
    },
    setFilters: (state, action) => {
      state.filters = { ...state.filters, ...action.payload }
      state.pagination.page = 1 // Reset page on filter change
    },
    updateUser: (state, action) => {
      const { id, changes } = action.payload
      if (state.entities[id]) {
        state.entities[id] = { ...state.entities[id], ...changes }
      }
    },
  },
  extraReducers: (builder) => {
    builder
      .addCase(fetchUsers.pending, (state) => {
        state.loading = true
        state.error = null
      })
      .addCase(fetchUsers.fulfilled, (state, action) => {
        state.loading = false
        state.entities = action.payload.entities
        state.ids = action.payload.ids
      })
      .addCase(fetchUsers.rejected, (state, action) => {
        state.loading = false
        state.error = action.error.message
      })
  },
})
```

##### **State Management Decision Matrix**
- **useState**: Component-local, simple state
- **Zustand**: Global client state, moderate complexity
- **React Query**: Server state, caching, synchronization
- **Redux Toolkit**: Complex state logic, debugging requirements
- **Context API**: Theme, auth, rarely changing global state

#### **Custom Hooks Guidelines**
- **Single Responsibility**: One concern per hook
- **Naming**: Start with `use` prefix
- **Return Objects**: For multiple values, return objects not arrays
- **Co-locate Tests**: Include `.test.ts` files alongside hooks

```typescript
// Good custom hook
export const useProductColumns = () => {
  const [columns, setColumns] = useState<Column[]>([])
  const [loading, setLoading] = useState(false)
  
  return {
    columns,
    loading,
    updateColumns: setColumns,
  }
}
```

### **API Integration Patterns**

#### **React Query Usage**
- **Query Keys**: Use factory pattern for consistent query keys
- **Error Handling**: Implement global error boundaries
- **Caching**: Set appropriate stale times for data types
- **Mutations**: Use optimistic updates where appropriate

```typescript
// Query factory pattern
export const ProductQueries = {
  all: ['Product'] as const,
  lists: () => [...ProductQueries.all, 'list'] as const,
  list: (filters: string) => [...ProductQueries.lists(), { filters }] as const,
  details: () => [...ProductQueries.all, 'detail'] as const,
  detail: (id: string) => [...ProductQueries.details(), id] as const,
}
```

#### **Axios Configuration**
- **Base Configuration**: Centralized in `api/index.ts`
- **Interceptors**: Handle auth, logging, and error transformation
- **Type Safety**: Use TypeScript interfaces for request/response shapes

---

## **UI/UX Guidelines**

### **Material-UI (shadcn) Standards**
- **Theme Integration**: Use shadcn theme system for consistent styling
- **Component Variants**: Leverage shadcn's variant system
- **Custom Components**: Extend shadcn components, don't replace them
- **Responsive Design**: Use shadcn breakpoint system


### **Accessibility (A11y)**
- **ARIA Labels**: Provide meaningful labels for interactive elements
- **Keyboard Navigation**: Ensure all interactive elements are keyboard accessible
- **Color Contrast**: Follow WCAG guidelines
- **Screen Reader Support**: Test with screen readers

### **Internationalization (i18n)**
- **Key Structure**: Use nested keys for organization
- **Fallbacks**: Always provide English fallbacks
- **Interpolation**: Use i18next interpolation for dynamic content
- **Namespaces**: Organize translations by feature/page

```typescript
// i18n usage
const { t } = useTranslation('Product')
return <Typography>{t('table.noData')}</Typography>
```

---

## **Testing Standards**

### **Testing Stack**
- **Framework**: Vitest + Testing Library
- **Coverage**: Maintain >80% code coverage
- **Mocking**: Use MSW for API mocking, vi.mock() for modules

### **Test Organization**
- **Unit Tests**: Individual components and hooks
- **Integration Tests**: Component interactions
- **E2E Tests**: Critical user journeys (separate Katalon setup)

```typescript
// Test structure
describe('ProductTable', () => {
  const mockData = [
    { id: '1', name: 'Test Item' }
  ]

  it('should render table with data', () => {
    render(<ProductTable data={mockData} onRowClick={vi.fn()} />)
    expect(screen.getByText('Test Item')).toBeInTheDocument()
  })

  it('should handle row click', async () => {
    const handleClick = vi.fn()
    render(<ProductTable data={mockData} onRowClick={handleClick} />)
    
    await user.click(screen.getByText('Test Item'))
    expect(handleClick).toHaveBeenCalledWith('1')
  })
})
```

---

## **Code Quality & Linting**

### **ESLint Configuration**
- **Extends**: React, TypeScript, Prettier configurations
- **Plugins**: 
  - `react-hooks` for hooks rules
  - `unused-imports` for clean imports
  - `perfectionist` for import sorting
- **Custom Rules**: Project-specific linting rules

### **Pre-commit Hooks**
- **Husky + lint-staged**: Run linting and formatting on staged files
- **Type Checking**: Ensure TypeScript compilation passes
- **Test Execution**: Run relevant tests for changed files

### **Code Review Guidelines**
- **Performance**: Check for unnecessary re-renders
- **Security**: Validate input sanitization and API security
- **Accessibility**: Verify a11y compliance
- **Bundle Size**: Monitor import impacts on bundle size

---

## **Build & Deployment**

### **Vite Configuration**
- **Path Aliases**: Use `~/*` for src imports
- **Build Optimization**: Tree shaking and code splitting enabled
- **Environment Variables**: Use `VITE_` prefix for client-side variables

### **Performance Guidelines**
- **Lazy Loading**: Use React.lazy for route-level code splitting
- **Bundle Analysis**: Regular bundle size monitoring
- **Caching**: Implement proper browser caching strategies
- **CDN**: Leverage CDN for static assets

---

## **Security & Best Practices**

### **Data Handling**
- **Validation**: Client and server-side validation with Zod
- **Sanitization**: Sanitize user inputs
- **Error Handling**: Never expose sensitive information in errors
- **Logging**: Use structured logging with appropriate levels

---

## **Error Handling & Debugging**

### **Error Boundaries**
- **Component Level**: Wrap major sections in error boundaries
- **Global Handler**: Implement global error reporting
- **User Feedback**: Provide meaningful error messages to users

### **Debugging Tools**
- **React Query Devtools**: For server state debugging
- **Zustand Devtools**: For client state inspection
- **Browser DevTools**: Leverage React Developer Tools

---

## **Monitoring & Analytics**

### **Performance Monitoring**
- **Core Web Vitals**: Monitor and optimize for performance metrics
- **Error Tracking**: Comprehensive error tracking and alerting

### **User Analytics**
- **Feature Usage**: Track feature adoption and usage patterns
- **Performance Metrics**: Monitor application performance
- **User Experience**: Track user satisfaction metrics

---

## 📚 **Documentation & Knowledge Sharing**

### **Component Library Documentation**

#### **Component Documentation Standards**
- **Storybook Integration**: Document all reusable components
- **Props Documentation**: Use JSDoc for prop descriptions
- **Usage Examples**: Provide multiple usage scenarios
- **Design Tokens**: Document design system tokens and usage

```typescript
// ✅ Well-documented component
interface ButtonProps {
  /** The button variant style */
  variant?: 'primary' | 'secondary' | 'outline'
  /** The size of the button */
  size?: 'small' | 'medium' | 'large'
  /** Whether the button is disabled */
  disabled?: boolean
  /** Click handler function */
  onClick?: (event: MouseEvent<HTMLButtonElement>) => void
  /** Button content */
  children: ReactNode
}

/**
 * Button component following the design system
 * 
 * @example
 * ```tsx
 * <Button variant="primary" size="medium" onClick={handleClick}>
 *   Click me
 * </Button>
 * ```
 */
export const Button: React.FC<ButtonProps> = ({
  variant = 'primary',
  size = 'medium',
  disabled = false,
  onClick,
  children,
}) => {
  // Component implementation
}
```

#### **Storybook Configuration**
- **Story Structure**: Organize stories by component hierarchy
- **Controls**: Interactive controls for all props
- **Docs**: Auto-generated documentation from JSDoc
- **Accessibility**: Include a11y addon for testing
- **Design Tokens**: Document colors, typography, spacing

```typescript
// ✅ Storybook story example
export default {
  title: 'Components/Button',
  component: Button,
  parameters: {
    docs: {
      description: {
        component: 'Primary button component for user actions',
      },
    },
  },
  argTypes: {
    variant: {
      control: { type: 'select' },
      options: ['primary', 'secondary', 'outline'],
    },
    size: {
      control: { type: 'select' },
      options: ['small', 'medium', 'large'],
    },
  },
} as Meta<typeof Button>

export const Primary: StoryObj<typeof Button> = {
  args: {
    variant: 'primary',
    children: 'Button',
  },
}

export const AllSizes: StoryObj<typeof Button> = {
  render: () => (
    <div style={{ display: 'flex', gap: '1rem' }}>
      <Button size="small">Small</Button>
      <Button size="medium">Medium</Button>
      <Button size="large">Large</Button>
    </div>
  ),
}
```

### **Code Documentation Standards**

#### **JSDoc Requirements**
- **Functions**: Document purpose, parameters, return values, examples
- **Classes**: Document purpose, usage patterns, examples
- **Complex Logic**: Explain algorithms and business rules
- **Types**: Document complex type definitions and their usage

```typescript
// ✅ Well-documented function
/**
 * Calculates the total price including taxes and discounts
 * 
 * @param basePrice - The original price before modifications
 * @param taxRate - Tax rate as decimal (e.g., 0.08 for 8%)
 * @param discount - Discount amount or percentage
 * @param discountType - Whether discount is 'amount' or 'percentage'
 * @returns The final calculated price
 * 
 * @example
 * ```typescript
 * const total = calculatePrice(100, 0.08, 10, 'percentage')
 * // Returns 97.2 (100 - 10% = 90, + 8% tax = 97.2)
 * ```
 */
export const calculatePrice = (
  basePrice: number,
  taxRate: number,
  discount: number,
  discountType: 'amount' | 'percentage'
): number => {
  // Implementation with clear variable names and comments
  const discountAmount = discountType === 'percentage' 
    ? basePrice * (discount / 100)
    : discount
    
  const priceAfterDiscount = basePrice - discountAmount
  const finalPrice = priceAfterDiscount * (1 + taxRate)
  
  return Math.round(finalPrice * 100) / 100 // Round to 2 decimal places
}
```

#### **README Documentation**
- **Project Setup**: Clear installation and setup instructions
- **Development Workflow**: How to run, test, and build the project
- **Architecture Overview**: High-level system architecture
- **Contributing Guidelines**: How to contribute to the project
- **Deployment**: How to deploy the application

---
