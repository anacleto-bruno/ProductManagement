import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { apiClient, ProductQueries } from '~/api'

// Types for Product operations
interface Product {
  id: string
  name: string
  description?: string
  brand?: string
  sku?: string
  price: number
  category?: string
  createdAt: string
  updatedAt: string
}

interface ProductFilters extends Record<string, unknown> {
  search?: string
  category?: string
  brand?: string
  minPrice?: number
  maxPrice?: number
  page?: number
  pageSize?: number
}

interface ProductsResponse {
  products: Product[]
  totalCount: number
  totalPages: number
  currentPage: number
}

interface CreateProductData {
  name: string
  description?: string
  brand?: string
  sku?: string
  price: number
  category?: string
}

interface UpdateProductData extends Partial<CreateProductData> {
  id: string
}

/**
 * Custom hook for fetching paginated products list with filters
 */
export const useProducts = (filters: ProductFilters = {}) => {
  return useQuery({
    queryKey: ProductQueries.list(filters),
    queryFn: async (): Promise<ProductsResponse> => {
      const response = await apiClient.get('/products', {
        params: filters,
      })
      return response.data
    },
    staleTime: 1000 * 60 * 2, // 2 minutes
    enabled: true, // Always enabled for product list
  })
}

/**
 * Custom hook for fetching a single product by ID
 */
export const useProduct = (id: string) => {
  return useQuery({
    queryKey: ProductQueries.detail(id),
    queryFn: async (): Promise<Product> => {
      const response = await apiClient.get(`/products/${id}`)
      return response.data
    },
    enabled: !!id, // Only fetch if ID is provided
  })
}

/**
 * Custom hook for creating a new product with optimistic updates
 */
export const useCreateProduct = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (data: CreateProductData): Promise<Product> => {
      const response = await apiClient.post('/products', data)
      return response.data
    },
    onSuccess: (newProduct) => {
      // Invalidate and refetch products list
      queryClient.invalidateQueries({ queryKey: ProductQueries.lists() })
      
      // Add the new product to cache
      queryClient.setQueryData(ProductQueries.detail(newProduct.id), newProduct)
    },
  })
}

/**
 * Custom hook for updating a product with optimistic updates
 */
export const useUpdateProduct = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async ({ id, ...data }: UpdateProductData): Promise<Product> => {
      const response = await apiClient.put(`/products/${id}`, data)
      return response.data
    },
    onMutate: async (updatedProduct) => {
      // Cancel outgoing refetches
      await queryClient.cancelQueries({ 
        queryKey: ProductQueries.detail(updatedProduct.id) 
      })

      // Snapshot previous value
      const previousProduct = queryClient.getQueryData(
        ProductQueries.detail(updatedProduct.id)
      )

      // Optimistically update cache
      if (previousProduct) {
        queryClient.setQueryData(
          ProductQueries.detail(updatedProduct.id),
          { ...previousProduct, ...updatedProduct }
        )
      }

      return { previousProduct }
    },
    onError: (_err, updatedProduct, context) => {
      // Rollback on error
      if (context?.previousProduct) {
        queryClient.setQueryData(
          ProductQueries.detail(updatedProduct.id),
          context.previousProduct
        )
      }
    },
    onSettled: (_data, _error, variables) => {
      // Always refetch after error or success
      queryClient.invalidateQueries({ 
        queryKey: ProductQueries.detail(variables.id) 
      })
      queryClient.invalidateQueries({ 
        queryKey: ProductQueries.lists() 
      })
    },
  })
}

/**
 * Custom hook for deleting a product
 */
export const useDeleteProduct = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (id: string): Promise<void> => {
      await apiClient.delete(`/products/${id}`)
    },
    onSuccess: (_, deletedId) => {
      // Remove from cache
      queryClient.removeQueries({ 
        queryKey: ProductQueries.detail(deletedId) 
      })
      
      // Invalidate lists to refetch
      queryClient.invalidateQueries({ 
        queryKey: ProductQueries.lists() 
      })
    },
  })
}

/**
 * Custom hook for searching products
 */
export const useSearchProducts = (query: string, filters: ProductFilters = {}) => {
  return useQuery({
    queryKey: ProductQueries.search(query, filters),
    queryFn: async (): Promise<ProductsResponse> => {
      const response = await apiClient.get('/products/search', {
        params: { query, ...filters },
      })
      return response.data
    },
    enabled: query.length > 2, // Only search if query is longer than 2 characters
    staleTime: 1000 * 60, // 1 minute
  })
}