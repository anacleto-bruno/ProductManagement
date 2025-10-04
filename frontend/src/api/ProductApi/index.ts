import apiClient from '~/api'
import type { 
  Product, 
  CreateProduct, 
  UpdateProduct, 
  ProductListResponse, 
  ProductFilters 
} from '~/types/product'

export interface ProductQueryParams extends ProductFilters {
  page?: number
  pageSize?: number
}

export const ProductApi = {
  // Get paginated list of products
  async getProducts(params: ProductQueryParams = {}): Promise<ProductListResponse> {
    const { data } = await apiClient.get<ProductListResponse>('/products', { params })
    return data
  },

  // Get single product by ID
  async getProduct(id: string): Promise<Product> {
    const { data } = await apiClient.get<Product>(`/products/${id}`)
    return data
  },

  // Create new product
  async createProduct(product: CreateProduct): Promise<Product> {
    const { data } = await apiClient.post<Product>('/products', product)
    return data
  },

  // Update existing product
  async updateProduct(id: string, product: UpdateProduct): Promise<Product> {
    const { data } = await apiClient.put<Product>(`/products/${id}`, product)
    return data
  },

  // Delete product
  async deleteProduct(id: string): Promise<void> {
    await apiClient.delete(`/products/${id}`)
  },

  // Seed database with test data
  async seedProducts(count: number = 100): Promise<void> {
    await apiClient.post(`/products/seed/${count}`)
  },
}

// Query key factory for consistent React Query keys
export const ProductQueries = {
  all: ['products'] as const,
  lists: () => [...ProductQueries.all, 'list'] as const,
  list: (params: ProductQueryParams) => [...ProductQueries.lists(), params] as const,
  details: () => [...ProductQueries.all, 'detail'] as const,
  detail: (id: string) => [...ProductQueries.details(), id] as const,
}