import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { ProductApi, ProductQueries, type ProductQueryParams } from '~/api/ProductApi'
import { useAppStore } from '~/states/appStore'
import type { Product, CreateProduct, UpdateProduct } from '~/types/product'

/**
 * Hook for fetching paginated products with search and filters
 */
export const useProducts = (params: ProductQueryParams = {}) => {
  return useQuery({
    queryKey: ProductQueries.list(params),
    queryFn: () => ProductApi.getProducts(params),
    staleTime: 5 * 60 * 1000, // 5 minutes
    gcTime: 10 * 60 * 1000, // 10 minutes
    placeholderData: (previousData) => previousData, // Keep previous data while loading new data
    retry: (failureCount, error) => {
      // Don't retry on 4xx errors except 408, 429
      if (error && typeof error === 'object' && 'status' in error) {
        const status = error.status as number
        if (status >= 400 && status < 500 && status !== 408 && status !== 429) {
          return false
        }
      }
      return failureCount < 3
    },
  })
}

/**
 * Hook for fetching a single product by ID
 */
export const useProduct = (id: string) => {
  return useQuery({
    queryKey: ProductQueries.detail(id),
    queryFn: () => ProductApi.getProduct(id),
    enabled: !!id,
    staleTime: 10 * 60 * 1000, // 10 minutes
    gcTime: 15 * 60 * 1000, // 15 minutes
  })
}

/**
 * Hook for creating a new product
 */
export const useCreateProduct = () => {
  const queryClient = useQueryClient()
  const { addNotification } = useAppStore()

  return useMutation({
    mutationFn: (product: CreateProduct) => ProductApi.createProduct(product),
    onSuccess: (newProduct) => {
      // Invalidate and refetch products list
      queryClient.invalidateQueries({ queryKey: ProductQueries.lists() })
      
      // Add the new product to any existing queries
      queryClient.setQueryData(ProductQueries.detail(newProduct.id), newProduct)
      
      addNotification({
        type: 'success',
        message: 'Product created successfully',
      })
    },
    onError: (error) => {
      addNotification({
        type: 'error',
        message: error instanceof Error ? error.message : 'Failed to create product',
      })
    },
  })
}

/**
 * Hook for updating an existing product
 */
export const useUpdateProduct = () => {
  const queryClient = useQueryClient()
  const { addNotification } = useAppStore()

  return useMutation({
    mutationFn: ({ id, product }: { id: string; product: UpdateProduct }) =>
      ProductApi.updateProduct(id, product),
    onMutate: async ({ id, product }) => {
      // Cancel outgoing refetches
      await queryClient.cancelQueries({ queryKey: ProductQueries.detail(id) })

      // Snapshot the previous value
      const previousProduct = queryClient.getQueryData(ProductQueries.detail(id))

      // Optimistically update to the new value
      queryClient.setQueryData(ProductQueries.detail(id), (old: Product | undefined) => {
        if (!old) return old
        return { ...old, ...product }
      })

      return { previousProduct }
    },
    onError: (error, { id }, context) => {
      // Rollback on error
      if (context?.previousProduct) {
        queryClient.setQueryData(ProductQueries.detail(id), context.previousProduct)
      }
      
      addNotification({
        type: 'error',
        message: error instanceof Error ? error.message : 'Failed to update product',
      })
    },
    onSuccess: () => {
      addNotification({
        type: 'success',
        message: 'Product updated successfully',
      })
    },
    onSettled: (_, __, { id }) => {
      // Always refetch after error or success
      queryClient.invalidateQueries({ queryKey: ProductQueries.detail(id) })
      queryClient.invalidateQueries({ queryKey: ProductQueries.lists() })
    },
  })
}

/**
 * Hook for deleting a product
 */
export const useDeleteProduct = () => {
  const queryClient = useQueryClient()
  const { addNotification } = useAppStore()

  return useMutation({
    mutationFn: (id: string) => ProductApi.deleteProduct(id),
    onSuccess: (_, deletedId) => {
      // Remove from cache
      queryClient.removeQueries({ queryKey: ProductQueries.detail(deletedId) })
      
      // Invalidate lists to refetch
      queryClient.invalidateQueries({ queryKey: ProductQueries.lists() })
      
      addNotification({
        type: 'success',
        message: 'Product deleted successfully',
      })
    },
    onError: (error) => {
      addNotification({
        type: 'error',
        message: error instanceof Error ? error.message : 'Failed to delete product',
      })
    },
  })
}

/**
 * Hook for seeding products (testing purposes)
 */
export const useSeedProducts = () => {
  const queryClient = useQueryClient()
  const { addNotification } = useAppStore()

  return useMutation({
    mutationFn: (count: number = 100) => ProductApi.seedProducts(count),
    onSuccess: (_, count) => {
      // Invalidate all product queries to refetch with new data
      queryClient.invalidateQueries({ queryKey: ProductQueries.all })
      
      addNotification({
        type: 'success',
        message: `Successfully seeded ${count} products`,
      })
    },
    onError: (error) => {
      addNotification({
        type: 'error',
        message: error instanceof Error ? error.message : 'Failed to seed products',
      })
    },
  })
}