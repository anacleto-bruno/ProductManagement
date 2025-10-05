import React from 'react'
import { 
  Box,
  CircularProgress,
  Alert,
} from '@mui/material'
import { ProductTable } from './ProductTable'
import { PaginationControls } from './PaginationControls'
import { ProductsEmptyState } from './ProductsEmptyState'
import type { Product, Pagination } from '~/types/product'

interface ProductsContentProps {
  products: Product[]
  pagination: Pagination
  isLoading: boolean
  isFetching: boolean
  error: unknown
  hasSearch: boolean
  searchTerm: string
  onPageChange: (page: number) => void
  onPageSizeChange: (pageSize: number) => void
  onClearSearch: () => void
  onAddProduct: () => void
  onSeedProducts: () => void
  isSeedingProducts: boolean
}

export const ProductsContent: React.FC<ProductsContentProps> = ({
  products,
  pagination,
  isLoading,
  isFetching,
  error,
  hasSearch,
  searchTerm,
  onPageChange,
  onPageSizeChange,
  onClearSearch,
  onAddProduct,
  onSeedProducts,
  isSeedingProducts,
}) => {
  // Error Display
  if (error) {
    return (
      <Alert severity="error" sx={{ mb: 3 }}>
        {error instanceof Error ? error.message : 'An error occurred while fetching products'}
      </Alert>
    )
  }

  // Loading State for Initial Load
  if (isLoading && !isFetching) {
    return (
      <Box display="flex" justifyContent="center" py={4}>
        <CircularProgress />
      </Box>
    )
  }

  // Empty State - No Products or No Search Results
  if (!isLoading && products.length === 0) {
    return (
      <ProductsEmptyState
        hasSearch={hasSearch}
        searchTerm={searchTerm}
        onClearSearch={onClearSearch}
        onAddProduct={onAddProduct}
        onSeedProducts={onSeedProducts}
        isSeedingProducts={isSeedingProducts}
      />
    )
  }

  // Products Table with Pagination
  return (
    <Box sx={{ 
      width: '100%',
      minWidth: '100%', // Prevents shrinking when content is empty
      display: 'flex',
      flexDirection: 'column'
    }}>
      <ProductTable
        products={products}
        loading={isFetching}
        error={error instanceof Error ? error.message : null}
      />

      {/* Pagination Controls */}
      {pagination.totalCount > 0 && (
        <PaginationControls
          pagination={pagination}
          onPageChange={onPageChange}
          onPageSizeChange={onPageSizeChange}
          loading={isFetching}
        />
      )}
    </Box>
  )
}