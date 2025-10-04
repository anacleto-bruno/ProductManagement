import React from 'react'
import { 
  Typography, 
  Paper, 
  Box,
  Button,
  TextField,
  InputAdornment,
  CircularProgress,
  Alert,
  Chip,
} from '@mui/material'
import { 
  Add as AddIcon,
  Search as SearchIcon,
  Clear as ClearIcon,
  CloudDownload as SeedIcon,
} from '@mui/icons-material'
import { useTranslation } from 'react-i18next'
import { ProductTable } from '~/components/ProductTable'
import { PaginationControls } from '~/components/PaginationControls'
import { useProducts, useSeedProducts } from '~/hooks/useProducts'
import { useProductTableState } from '~/hooks/useProductTableState'

export const ProductsPage: React.FC = () => {
  const { t } = useTranslation('products')
  const { t: tCommon } = useTranslation('common')

  // Table state management
  const {
    search,
    queryParams,
    handleSearchChange,
    handlePageChange,
    handlePageSizeChange,
    clearSearch,
    isSearching,
    hasSearch,
  } = useProductTableState({
    initialPageSize: 20,
  })

  // Fetch products data
  const {
    data: productResponse,
    isLoading,
    error,
    isFetching,
  } = useProducts(queryParams)

  // Seed products mutation
  const seedProductsMutation = useSeedProducts()

  const handleSeedProducts = () => {
    seedProductsMutation.mutate(100)
  }

  const products = productResponse?.data || []
  const pagination = productResponse?.pagination || {
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,
  }

  return (
    <Box>
      {/* Header */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Box>
          <Typography variant="h4" component="h1" gutterBottom>
            {t('title')}
          </Typography>
          {pagination.totalCount > 0 && (
            <Typography variant="body2" color="text.secondary">
              {pagination.totalCount} {t('title').toLowerCase()} {tCommon('found')}
            </Typography>
          )}
        </Box>
        
        <Box display="flex" gap={2}>
          <Button
            variant="outlined"
            startIcon={<SeedIcon />}
            onClick={handleSeedProducts}
            disabled={seedProductsMutation.isPending}
          >
            {seedProductsMutation.isPending ? (
              <CircularProgress size={16} sx={{ mr: 1 }} />
            ) : null}
            {t('seedData')}
          </Button>
          
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => console.log('Add product - Epic 8')}
          >
            {t('addProduct')}
          </Button>
        </Box>
      </Box>

      {/* Search Controls */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Box display="flex" gap={2} alignItems="center">
          <TextField
            fullWidth
            placeholder={t('searchPlaceholder')}
            value={search}
            onChange={(e) => handleSearchChange(e.target.value)}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon />
                </InputAdornment>
              ),
              endAdornment: hasSearch ? (
                <InputAdornment position="end">
                  <Button
                    size="small"
                    onClick={clearSearch}
                    startIcon={<ClearIcon />}
                    sx={{ minWidth: 'auto' }}
                  >
                    {tCommon('clear')}
                  </Button>
                </InputAdornment>
              ) : null,
            }}
          />
          
          {isSearching && (
            <CircularProgress size={24} />
          )}
        </Box>

        {hasSearch && (
          <Box mt={2}>
            <Chip
              label={`${tCommon('search')}: "${search}"`}
              onDelete={clearSearch}
              variant="outlined"
              color="primary"
            />
          </Box>
        )}
      </Paper>

      {/* Error Display */}
      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error instanceof Error ? error.message : 'An error occurred while fetching products'}
        </Alert>
      )}

      {/* Loading State for Initial Load */}
      {isLoading && !isFetching && (
        <Box display="flex" justifyContent="center" py={4}>
          <CircularProgress />
        </Box>
      )}

      {/* Products Table */}
      {!isLoading && (
        <>
          <ProductTable
            products={products}
            loading={isFetching}
            error={error instanceof Error ? error.message : null}
          />

          {/* Pagination Controls */}
          {pagination.totalCount > 0 && (
            <PaginationControls
              pagination={pagination}
              onPageChange={handlePageChange}
              onPageSizeChange={handlePageSizeChange}
              loading={isFetching}
            />
          )}
        </>
      )}

      {/* Empty State */}
      {!isLoading && !error && products.length === 0 && !hasSearch && (
        <Paper sx={{ p: 6, textAlign: 'center' }}>
          <Typography variant="h6" gutterBottom>
            No products found
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
            Get started by seeding some sample data or adding your first product.
          </Typography>
          <Box display="flex" gap={2} justifyContent="center">
            <Button
              variant="outlined"
              startIcon={<SeedIcon />}
              onClick={handleSeedProducts}
              disabled={seedProductsMutation.isPending}
            >
              {t('seedData')}
            </Button>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => console.log('Add product - Epic 8')}
            >
              {t('addProduct')}
            </Button>
          </Box>
        </Paper>
      )}
    </Box>
  )
}