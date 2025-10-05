import React from 'react'
import { 
  Typography, 
  Box,
  Button,
  CircularProgress,
} from '@mui/material'
import { 
  Add as AddIcon,
  CloudDownload as SeedIcon,
} from '@mui/icons-material'
import { useTranslation } from 'react-i18next'

interface ProductsHeaderProps {
  totalCount: number
  onSeedProducts: () => void
  onAddProduct: () => void
  isSeedingProducts: boolean
}

export const ProductsHeader: React.FC<ProductsHeaderProps> = ({
  totalCount,
  onSeedProducts,
  onAddProduct,
  isSeedingProducts,
}) => {
  const { t } = useTranslation('products')
  const { t: tCommon } = useTranslation('common')

  return (
    <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
      <Box>
        <Typography variant="h4" component="h1" gutterBottom>
          {t('title')}
        </Typography>
        {totalCount > 0 && (
          <Typography variant="body2" color="text.secondary">
            {totalCount} {t('title').toLowerCase()} {tCommon('found')}
          </Typography>
        )}
      </Box>
      
      <Box display="flex" gap={2}>
        <Button
          variant="outlined"
          startIcon={<SeedIcon />}
          onClick={onSeedProducts}
          disabled={isSeedingProducts}
        >
          {isSeedingProducts ? (
            <CircularProgress size={16} sx={{ mr: 1 }} />
          ) : null}
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
    </Box>
  )
}