import { useProducts, useSeedProducts } from './useProducts'
import { useProductTableState } from './useProductTableState'

export const useProductsPage = () => {
  // Table state management
  const tableState = useProductTableState({
    initialPageSize: 20,
  })

  // Fetch products data
  const productsQuery = useProducts(tableState.queryParams)

  // Seed products mutation
  const seedProductsMutation = useSeedProducts()

  // Event handlers
  const handleAddProduct = () => {
    console.log('Add product - Epic 8')
  }

  const handleSeedProducts = () => {
    seedProductsMutation.mutate(100)
  }

  // Derived state
  const products = productsQuery.data?.data || []
  const pagination = productsQuery.data ? {
    page: productsQuery.data.page,
    pageSize: productsQuery.data.pageSize,
    totalCount: productsQuery.data.totalCount,
    totalPages: productsQuery.data.totalPages,
    hasNextPage: productsQuery.data.hasNextPage,
    hasPreviousPage: productsQuery.data.hasPreviousPage,
  } : {
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
    hasNextPage: false,
    hasPreviousPage: false,
  }

  return {
    // Data
    products,
    pagination,
    
    // Loading states
    isLoading: productsQuery.isLoading,
    isFetching: productsQuery.isFetching,
    error: productsQuery.error,
    isSeedingProducts: seedProductsMutation.isPending,
    
    // Table state
    ...tableState,
    
    // Event handlers
    handleAddProduct,
    handleSeedProducts,
  }
}