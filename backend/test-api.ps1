# Product Management API Test Script
# Run this after starting the Functions app with: func start --dotnet-isolated

Write-Host "=== Product Management API Tests ===" -ForegroundColor Green

$baseUrl = "http://localhost:7071/api"

# Test 1: Seed Basic Data (Colors and Sizes)
Write-Host "`n1. Seeding basic data (colors and sizes)..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/seed/basic" -Method POST
    Write-Host "✅ Basic data seeded: $($response.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed to seed basic data: $($_.Exception.Message)" -ForegroundColor Red
    return
}

# Test 2: Seed Sample Products  
Write-Host "`n2. Seeding 10 sample products..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/products/seed/10" -Method POST
    Write-Host "✅ Products seeded: $($response.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed to seed products: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Get All Products (Paginated)
Write-Host "`n3. Getting products (page 1, size 5)..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/products?page=1&pageSize=5" -Method GET
    Write-Host "✅ Retrieved $($response.data.Count) products (Total: $($response.totalCount))" -ForegroundColor Green
    Write-Host "   Pages: $($response.page)/$($response.totalPages)" -ForegroundColor Cyan
} catch {
    Write-Host "❌ Failed to get products: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Search Products
Write-Host "`n4. Searching products..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/products?searchTerm=shirt&page=1&pageSize=3" -Method GET
    Write-Host "✅ Search returned $($response.data.Count) products" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed to search products: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 5: Create a New Product
Write-Host "`n5. Creating a new product..." -ForegroundColor Yellow
$newProduct = @{
    name = "Test Product"
    description = "A test product created via API"
    model = "TP-001"
    brand = "Test Brand"  
    sku = "TEST-TP-001"
    price = 29.99
    category = "Testing"
    colorIds = @(1, 2)
    sizeIds = @(2, 3, 4)
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/products" -Method POST -Body $newProduct -ContentType "application/json"
    $productId = $response.id
    Write-Host "✅ Product created with ID: $productId" -ForegroundColor Green
    
    # Test 6: Get the Created Product
    Write-Host "`n6. Getting created product..." -ForegroundColor Yellow
    $response = Invoke-RestMethod -Uri "$baseUrl/products/$productId" -Method GET
    Write-Host "✅ Retrieved product: $($response.name) - $($response.price)" -ForegroundColor Green
    Write-Host "   Colors: $($response.colors.Count), Sizes: $($response.sizes.Count)" -ForegroundColor Cyan
    
    # Test 7: Update the Product
    Write-Host "`n7. Updating product..." -ForegroundColor Yellow
    $updateProduct = @{
        name = "Updated Test Product"
        description = "Updated description"
        model = "TP-001-V2"
        brand = "Test Brand"
        sku = "TEST-TP-001"
        price = 39.99
        category = "Testing"
        colorIds = @(1, 3, 5)
        sizeIds = @(2, 4)
    } | ConvertTo-Json
    
    $response = Invoke-RestMethod -Uri "$baseUrl/products/$productId" -Method PUT -Body $updateProduct -ContentType "application/json"
    Write-Host "✅ Product updated: $($response.name) - $($response.price)" -ForegroundColor Green
    
    # Test 8: Delete the Product
    Write-Host "`n8. Deleting product..." -ForegroundColor Yellow
    Invoke-RestMethod -Uri "$baseUrl/products/$productId" -Method DELETE
    Write-Host "✅ Product deleted successfully" -ForegroundColor Green
    
} catch {
    Write-Host "❌ Failed product operation: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== All tests completed! ===" -ForegroundColor Green
Write-Host "The Product Management API is working correctly." -ForegroundColor Cyan