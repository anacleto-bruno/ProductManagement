# API Documentation

## OpenAPI/Swagger Documentation

The Product Management API provides automatic OpenAPI documentation through Azure Functions built-in support.

### Available Endpoints:

#### **OpenAPI Specifications:**
- **OpenAPI 3.0:** `GET /api/openapi/v3.json` ✨ **(Recommended)**
- **Swagger 2.0:** `GET /api/openapi/v2.json`

#### **Swagger UI:**
Interactive API documentation is available through multiple options:

**🎯 Built-in Swagger UI (Recommended)**
- **URL:** `GET /api/swagger` 
- **Direct Link:** [http://localhost:7071/api/swagger](http://localhost:7071/api/swagger)

**Option 2: Online Swagger Editor**
1. Go to [https://editor.swagger.io/](https://editor.swagger.io/)
2. File → Import URL
3. Enter: `http://localhost:7071/api/openapi/v3.json`

**Option 3: Local Swagger UI (Docker)**
```bash
docker run -p 8080:8080 -e SWAGGER_JSON_URL=http://localhost:7071/api/openapi/v3.json swaggerapi/swagger-ui
```
Then access: [http://localhost:8080](http://localhost:8080)

**Option 4: VS Code Extension**
- Install "Swagger Viewer" extension
- Open the `/api/openapi/v3.json` endpoint in VS Code

### API Features Documented:

✅ **Complete CRUD Operations:** Products, Colors, Sizes  
✅ **Pagination & Search:** Advanced filtering and sorting  
✅ **Data Seeding:** Mock data generation for testing  
✅ **Validation:** Request/response schemas with validation rules  
✅ **Error Handling:** HTTP status codes and error responses  

### API Information:
- **Title:** Product Management API
- **Version:** v1
- **Base URL:** `http://localhost:7071/api`
- **Contact:** Product Management Team
- **License:** MIT

### Example Usage:

```bash
# Get OpenAPI specification
curl -X GET http://localhost:7071/api/openapi/v3.json

# Test API endpoints
curl -X GET http://localhost:7071/api/products
curl -X POST http://localhost:7071/api/products/seed/10
```

---

**Note:** No custom functions are needed for API documentation. Azure Functions automatically generates OpenAPI specifications from the `[OpenApiOperation]` attributes on your endpoint functions.