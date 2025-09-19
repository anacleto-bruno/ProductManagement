import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import productApi from '../api/product-api'
import type { 
  CreateProductRequest, 
  ProductListRequest, 
  UpdateProductRequest 
} from '../api/product-api'
import { useProductStore } from '../states/product-store'

export const useProducts = (params: ProductListRequest = {}) => {
  const setStorePagination = useProductStore((state) => state.setPagination)
  
  const { page = 1, perPage = 20 } = params

  const queryResult = useQuery({
    queryKey: ['products', params],
    queryFn: async () => {
      const response = await productApi.getProducts(params)
      
      // Update the store with pagination data
      setStorePagination({
        page,
        perPage,
        totalItems: response.totalCount,
        totalPages: response.totalPages
      })
      
      return response
    }
  })

  return queryResult
}

export const useProduct = (id: string | undefined) => {
  return useQuery({
    queryKey: ['product', id],
    queryFn: () => productApi.getProductById(id!),
    enabled: !!id,
  })
}

export const useProductSearch = (params: ProductListRequest = {}) => {
  return useQuery({
    queryKey: ['products-search', params],
    queryFn: () => productApi.searchProducts(params),
  })
}

export const useCreateProduct = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (newProduct: CreateProductRequest) => {
      return productApi.createProduct(newProduct)
    },
    onSuccess: () => {
      // Invalidate the products list query to refetch the data
      queryClient.invalidateQueries({ queryKey: ['products'] })
    },
  })
}

export const useUpdateProduct = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (product: UpdateProductRequest) => {
      return productApi.updateProduct(product)
    },
    onSuccess: (updatedProduct) => {
      // Update the specific product in the cache
      queryClient.setQueryData(['product', updatedProduct.id], updatedProduct)
      
      // Invalidate the products list query to refetch the data
      queryClient.invalidateQueries({ queryKey: ['products'] })
    },
  })
}

export const useDeleteProduct = () => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (id: string) => {
      return productApi.deleteProduct(id)
    },
    onSuccess: (_data, id) => {
      // Remove the deleted product from the cache
      queryClient.removeQueries({ queryKey: ['product', id] })
      
      // Invalidate the products list query to refetch the data
      queryClient.invalidateQueries({ queryKey: ['products'] })
    },
  })
}