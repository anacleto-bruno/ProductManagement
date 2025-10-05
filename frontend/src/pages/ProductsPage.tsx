import React from 'react'
import { Box } from '@mui/material'
import { ProductsHeader } from '~/components/ProductsHeader'
import { ProductsSearch } from '~/components/ProductsSearch'
import { ProductsContent } from '~/components/ProductsContent'
import { useProductsPage } from '~/hooks/useProductsPage'

export const ProductsPage: React.FC = () => {
  const {
    // Data
    products,
    pagination,
    
    // Loading states
    isLoading,
    isFetching,
    error,
    isSeedingProducts,
    
    // Search state
    search,
    hasSearch,
    isSearching,
    
    // Event handlers
    handleSearchChange,
    handlePageChange,
    handlePageSizeChange,
    clearSearch,
    handleAddProduct,
    handleSeedProducts,
  } = useProductsPage()

  return (
    <Box>
      <ProductsHeader
        totalCount={pagination.totalCount}
        onSeedProducts={handleSeedProducts}
        onAddProduct={handleAddProduct}
        isSeedingProducts={isSeedingProducts}
      />

      <ProductsSearch
        search={search}
        onSearchChange={handleSearchChange}
        onClearSearch={clearSearch}
        isSearching={isSearching}
        hasSearch={hasSearch}
        totalCount={pagination.totalCount}
      />

      <ProductsContent
        products={products}
        pagination={pagination}
        isLoading={isLoading}
        isFetching={isFetching}
        error={error}
        hasSearch={hasSearch}
        searchTerm={search}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
        onClearSearch={clearSearch}
        onAddProduct={handleAddProduct}
        onSeedProducts={handleSeedProducts}
        isSeedingProducts={isSeedingProducts}
      />
    </Box>
  )
}