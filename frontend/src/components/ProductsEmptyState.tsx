import React from 'react'
import { 
  Paper, 
  Typography,
  Box,
  Button,
} from '@mui/material'
import { 
  Add as AddIcon,
  Clear as ClearIcon,
  CloudDownload as SeedIcon,
} from '@mui/icons-material'
import { useTranslation } from 'react-i18next'

interface ProductsEmptyStateProps {
  hasSearch: boolean
  searchTerm?: string
  onClearSearch: () => void
  onAddProduct: () => void
  onSeedProducts: () => void
  isSeedingProducts: boolean
}

export const ProductsEmptyState: React.FC<ProductsEmptyStateProps> = ({
  hasSearch,
  searchTerm,
  onClearSearch,
  onAddProduct,
  onSeedProducts,
  isSeedingProducts,
}) => {
  const { t } = useTranslation('products')
  const { t: tCommon } = useTranslation('common')

  if (hasSearch && searchTerm) {
    // Empty search results state
    return (
      <Paper sx={{ p: 6, textAlign: 'center' }}>
        <Typography variant="h6" gutterBottom>
          No results found for "{searchTerm}"
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          Try adjusting your search criteria or browse all products.
        </Typography>
        <Box display="flex" gap={2} justifyContent="center">
          <Button
            variant="outlined"
            onClick={onClearSearch}
            startIcon={<ClearIcon />}
          >
            {tCommon('clear')} {tCommon('search')}
          </Button>
          <Button
            variant="outlined"
            startIcon={<SeedIcon />}
            onClick={onSeedProducts}
            disabled={isSeedingProducts}
          >
            {t('seedData')}
          </Button>
        </Box>
      </Paper>
    )
  }

  // No products at all state
  return (
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
          onClick={onSeedProducts}
          disabled={isSeedingProducts}
        >
          {t('seedData')}
        </Button>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={onAddProduct}
        >
          {t('addProduct')}
        </Button>
      </Box>
    </Paper>
  )
}