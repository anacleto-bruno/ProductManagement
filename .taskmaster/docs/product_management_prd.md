# Product Requirements Document (PRD) – Expanded into Epics

## Epic 1: Database Foundation
**Goal:** Provide a scalable, maintainable backend with a normalized relational schema.  

### Features
- Design PostgreSQL schema for **Products**, **Colors**, **Sizes**  
- Implement **ActiveRecord migrations** for schema creation and updates  
- Establish Docker setup PostgreSQL server and database

### Acceptance Criteria
- Database containers start with `docker-compose up`  
- Schema supports all required product attributes (name, description, model, brand, SKU, price, color, size)
- Many-to-many relationships for product–color, product–size, brand are implemented  
- Documentation includes database ERD and migration steps  

---

## Epic 2: Backend CRUD API
**Goal:** Provide CRUD product management endpoints.  

### Features
- Establish Docker setup for REST API system
- `POST /products` → Create product  
- `GET /product/:id` → Retrieve product by ID  
- `PUT /products/:id` → Update product  
- `DELETE /products/:id` → Delete product  
- `POST /products/seed/:num` → Seed database with sample products (1 < num < 1000 records for testing)

### Acceptance Criteria
- Each endpoint validates required fields  
- Returns proper HTTP status codes (201, 200, 404, 422, etc.)  
- Includes automated tests for each endpoint  
- API documentation (Swagger) 

---

## Epic 3: Product Retrieval & Pagination
**Goal:** Efficiently retrieve lists of products with pagination.  

### Features
- `GET /products` endpoint with `page` and `pageSize` parameters  
- Return response metadata (total count, current page, total pages)  
- Ensure scalability with database indexing  
- Search across name, description, category, brand, SKU  
- Apply pagination to search results  
- Optimize query with indexes and caching (e.g., Redis)  

### Acceptance Criteria
- Pagination works with large dataset (1000+ products)  
- Default `pageSize` value defined (e.g., 20)  
- Automated tests validate pagination correctness  
- Typing a partial query returns relevant matches  
- Response time <200ms for 1000+ records  
- Caching layer improves repeat query performance  
- Tests cover search scenarios  

---

## Epic 4: Seed Database via API endpoint
**Goal:** Allow users to populate database with mock data 

### Features
- `POST /products/seed/:rows` endpoint  
- Default for <rows> is 100, mim=1, max=10000
- Adds <rows> records of mocked data in the database 

### Acceptance Criteria
- rows when informed needs to be between 1 and 10000
- rows assumes the value 100 when not informed
- Database is populated with numRows of new products with mock date 
- Mock data is meaninfull with the field and data type associade

---

## Epic 5: Frontend Setup & Architecture
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
- **Build System**: Vite
- **State Management**: 
  - Zustand for client-side state
  - React Query (@tanstack/react-query) for server state
- **UI Framework**: Material UI
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

## Epic 6: Product List & Pagination UI
**Goal:** Display paginated list of products in a table.  

### Features
- Fetch products via `GET /products` on initial load  
- Render results in Material UI Table  
- Add pagination controls linked to backend pagination  

### Acceptance Criteria
- Initial load shows table with first page of products (columns: name, description, model, brand, SKU, price, color, size)
- Pagination controls update table correctly 
- Loading and error states are handled gracefully  

---

## Epic 7: Search UI & Integration
**Goal:** Provide a responsive search experience.  

### Features
- Text input field for product search  
- API calls to `GET /products` as user types  
- Update table results dynamically  

### Acceptance Criteria
- Debounced API calls to avoid excessive requests  
- Display search results with pagination controls  
- UI feedback for “no results found”  

---

## Epic 8: CRUD Operations in Frontend
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

## Epic 9: Testing & Quality Assurance
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

## Epic 10: Documentation & Developer Experience
**Goal:** Provide clear, reproducible setup and usage documentation.  

### Features
- `README.md` with setup instructions for backend & frontend  
- Docker usage instructions (`docker-compose up`)  
- API reference (Swagger)  
- Contribution guide for developers  

### Acceptance Criteria
- New developer can run project in <10 minutes  
- Docs cover environment variables, migrations, seeding  
- API usage documented with examples  

---