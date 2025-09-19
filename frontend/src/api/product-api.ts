import { apiClient } from './api-client'

export interface Product {
  id: string
  name: string
  description: string
  brand: string
  sku: string
  price: number
  category: string
  colors: Color[]
  sizes: Size[]
  createdAt: string
  updatedAt: string
}

export interface Color {
  id: string
  name: string
  hexCode: string
}

export interface Size {
  id: string
  name: string
  value: string
}

export interface PaginatedResponse<T> {
  items: T[]
  totalCount: number
  currentPage: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export interface ProductListRequest {
  page?: number
  perPage?: number
  searchTerm?: string
  category?: string
  brand?: string
  minPrice?: number
  maxPrice?: number
}

export interface CreateProductRequest {
  name: string
  description: string
  brand: string
  sku: string
  price: number
  category: string
  colorIds: string[]
  sizeIds: string[]
}

export interface UpdateProductRequest extends Partial<CreateProductRequest> {
  id: string
}

const productApi = {
  getProducts: async (params: ProductListRequest = {}): Promise<PaginatedResponse<Product>> => {
    const response = await apiClient.get('/products', { params })
    return response.data
  },

  getProductById: async (id: string): Promise<Product> => {
    const response = await apiClient.get(`/products/${id}`)
    return response.data
  },

  searchProducts: async (params: ProductListRequest = {}): Promise<PaginatedResponse<Product>> => {
    const response = await apiClient.get('/products/search', { params })
    return response.data
  },

  createProduct: async (product: CreateProductRequest): Promise<Product> => {
    const response = await apiClient.post('/products', product)
    return response.data
  },

  updateProduct: async (product: UpdateProductRequest): Promise<Product> => {
    const response = await apiClient.put(`/products/${product.id}`, product)
    return response.data
  },

  deleteProduct: async (id: string): Promise<void> => {
    await apiClient.delete(`/products/${id}`)
  },
}

export default productApi