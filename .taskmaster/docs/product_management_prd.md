# Product Requirements Document (PRD) – Expanded into Epics

## Epic 1: Backend API & Database Foundation
**Goal:** Provide a scalable, maintainable backend with a normalized relational schema.  

### Features
- Design PostgreSQL schema for **Products**, **Colors**, **Sizes**  
- Implement **ActiveRecord migrations** for schema creation and updates  
- Establish Docker setup for .Net Core Azure Functions + PostgreSQL  
- Seed database with sample products (1000+ records for testing)

### Architecture Patterns

#### Clean Architecture
Maintain clear separation of concerns across layers:

```
{ProjectName}/
├── Functions/          # Azure Function endpoints (Presentation Layer)
├── Services/           # Business logic (Application Layer)
├── Infrastructure/     # External concerns (Infrastructure Layer)
├── Entities/          # Domain models (Domain Layer)
├── Dtos/              # Data Transfer Objects
├── Models/            # View models and request/response models
├── Helpers/           # Utility classes and extensions
├── Constants/         # Application constants
└── Enumerations/      # Enum definitions
```

### Acceptance Criteria
- Database containers start with `docker-compose up`  
- Schema supports all required product attributes (name, description, brand, SKU, etc.)  
- Many-to-many relationships for product–color and product–size are implemented  
- Documentation includes database ERD and migration steps  

---

## Epic 2: Backend CRUD API
**Goal:** Provide basic product management endpoints.  

### Features
- `POST /products` → Create product  
- `GET /products/:id` → Retrieve product by ID  
- `PUT /products/:id` → Update product  
- `DELETE /products/:id` → Delete product  

### Acceptance Criteria
- Each endpoint validates required fields  
- Returns proper HTTP status codes (201, 200, 404, 422, etc.)  
- Includes automated tests for each endpoint  
- API documentation (Swagger) 

---

## Epic 3: Product Retrieval & Pagination
**Goal:** Efficiently retrieve lists of products with pagination.  

### Features
- `GET /products` endpoint with `page` and `per_page` parameters  
- Return response metadata (total count, current page, total pages)  
- Ensure scalability with database indexing  

### Acceptance Criteria
- Pagination works with large dataset (1000+ products)  
- Default `per_page` value defined (e.g., 20)  
- Automated tests validate pagination correctness  

---

## Epic 4: Dynamic Search & Filtering
**Goal:** Allow users to search products across multiple fields.  

### Features
- `GET /products/search?q=<query>` endpoint  
- Search across name, description, category, brand, SKU  
- Apply pagination to search results  
- Optimize query with indexes and caching (e.g., Redis)  

### Acceptance Criteria
- Typing a partial query returns relevant matches  
- Response time <200ms for 1000+ records  
- Caching layer improves repeat query performance  
- Tests cover search scenarios  

## Epic 5: Seed Database via API endpoint
**Goal:** Allow users to populate database with mock data 

### Features
- `POST /products/seed?numRows=<rows>` endpoint  
- Default for <rows> is 1000, mim=1, max=10000
- Adds <rows> records of mocked data in the database 

### Acceptance Criteria
- numRows when informed needs to be between 1 and 10000
- numRows assumes the value 100 when not informed
- Database is populated with numRows of new products with mock date 
- Mock data is meaninfull with the field and data type associade

---

## Epic 6: Frontend Setup & Architecture
**Goal:** Provide a modern React SPA foundation.  

### Features
- Initialize React project with Vite  
- Setup Material UI theme and components  
- Optmized state management

### Acceptance Criteria
- Material UI styling applied consistently  
- API base URL configurable via environment variable  

### **Architecture & Technology Stack**

#### **Core Technologies**
- **Frontend**: React 18.2 + TypeScript 5.5 (strict mode)
- **Build System**: Vite with Module Federation
- **State Management**: 
  - Zustand for client-side state
  - React Query (@tanstack/react-query) for server state
- **UI Framework**: shadcn
- **Routing**: React Router v7
- **Internationalization**: react-i18next
- **Validation**: Zod for schema validation

#### **Key Architectural Patterns**
- Single Page Application (SPA)
- Domain-driven API organization
- Component-first architecture with co-located tests
- Provider pattern for cross-cutting concerns
- Custom hooks for reusable business logic

---

## Epic 7: Product List & Pagination UI
**Goal:** Display paginated list of products in a table.  

### Features
- Fetch products via `GET /products` on initial load  
- Render results in Material UI Table  
- Add pagination controls linked to backend pagination  

### Acceptance Criteria
- Initial load shows first page of products  
- Pagination controls update table correctly  
- Loading and error states are handled gracefully  

---

## Epic 8: Search UI & Integration
**Goal:** Provide a responsive search experience.  

### Features
- Text input field for product search  
- API calls to `/products/search` as user types  
- Update table results dynamically  

### Acceptance Criteria
- Debounced API calls to avoid excessive requests  
- Display search results with pagination controls  
- UI feedback for “no results found”  

---

## Epic 9: CRUD Operations in Frontend
**Goal:** Enable full product lifecycle management via UI.  

### Features
- Add Product form → calls `POST /products`  
- Edit Product form → calls `PUT /products/:id`  
- Delete Product → confirmation dialog, calls `DELETE /products/:id`  

### Acceptance Criteria
- Forms validate required fields  
- Success/failure notifications shown  
- Data refreshes after operations  

---

## Epic 10: Testing & Quality Assurance
**Goal:** Ensure robust automated testing across backend and frontend.  

### Features
- Backend: unit tests for all testable code
- Frontend: Jest + React Testing Library  
- Integration tests for API interaction  

### Acceptance Criteria
- >90% test coverage in backend critical paths  
- >80% test coverage in frontend components  
- Automated test suite passes with no failures  

---

## Epic 11: Documentation & Developer Experience
**Goal:** Provide clear, reproducible setup and usage documentation.  

### Features
- `README.md` with setup instructions for backend & frontend  
- Docker usage instructions (`docker-compose up`)  
- API reference (Swagger or Markdown docs)  
- Contribution guide for developers  

### Acceptance Criteria
- New developer can run project in <10 minutes  
- Docs cover environment variables, migrations, seeding  
- API usage documented with examples  

---

## Epic 12: Performance & Scalability Enhancements
**Goal:** Anticipate and mitigate real-world scaling concerns.  

### Features
- Add DB indexes on search-relevant fields  
- Implement caching (Redis) for common queries  
- Optimize API responses (e.g., JSON serialization)  

### Acceptance Criteria
- Performance benchmark meets <200ms average search response  
- Indexed fields confirmed in schema  
- Cache hit improves response time in repeated queries  