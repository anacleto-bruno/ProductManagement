import React from 'react'
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Chip,
  Box,
  Typography,
  Skeleton,
  Alert,
} from '@mui/material'
import { useTranslation } from 'react-i18next'
import type { Product } from '~/types/product'
import { formatCurrency } from '~/utils/common'

interface ProductTableProps {
  products: Product[]
  loading?: boolean
  error?: string | null
}

const TableSkeleton: React.FC = () => (
  <>
    {Array.from({ length: 5 }).map((_, index) => (
      <TableRow key={index}>
        {Array.from({ length: 8 }).map((_, cellIndex) => (
          <TableCell key={cellIndex}>
            <Skeleton variant="text" width="100%" />
          </TableCell>
        ))}
      </TableRow>
    ))}
  </>
)

export const ProductTable: React.FC<ProductTableProps> = ({
  products,
  loading = false,
  error = null,
}) => {
  const { t } = useTranslation('products')

  if (error) {
    return (
      <Alert severity="error" sx={{ mb: 2 }}>
        {error}
      </Alert>
    )
  }

  return (
    <TableContainer component={Paper} sx={{ mb: 3 }}>
      <Table stickyHeader>
        <TableHead>
          <TableRow>
            <TableCell sx={{ fontWeight: 600 }}>{t('name')}</TableCell>
            <TableCell sx={{ fontWeight: 600 }}>{t('description')}</TableCell>
            <TableCell sx={{ fontWeight: 600 }}>{t('model')}</TableCell>
            <TableCell sx={{ fontWeight: 600 }}>{t('brand')}</TableCell>
            <TableCell sx={{ fontWeight: 600 }}>{t('sku')}</TableCell>
            <TableCell sx={{ fontWeight: 600 }} align="right">{t('price')}</TableCell>
            <TableCell sx={{ fontWeight: 600 }}>{t('colors')}</TableCell>
            <TableCell sx={{ fontWeight: 600 }}>{t('sizes')}</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {loading ? (
            <TableSkeleton />
          ) : products.length === 0 ? (
            <TableRow>
              <TableCell colSpan={8} align="center" sx={{ py: 4 }}>
                <Typography variant="body1" color="text.secondary">
                  {t('table.noData')}
                </Typography>
              </TableCell>
            </TableRow>
          ) : (
            products.map((product) => (
              <TableRow
                key={product.id}
                hover
                sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
              >
                <TableCell>
                  <Typography variant="body2" fontWeight={500}>
                    {product.name}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Typography 
                    variant="body2" 
                    color="text.secondary"
                    sx={{
                      maxWidth: 200,
                      overflow: 'hidden',
                      textOverflow: 'ellipsis',
                      whiteSpace: 'nowrap',
                    }}
                    title={product.description}
                  >
                    {product.description || '-'}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="body2">
                    {product.model || '-'}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="body2" fontWeight={500}>
                    {product.brand || '-'}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="body2" fontFamily="monospace">
                    {product.sku}
                  </Typography>
                </TableCell>
                <TableCell align="right">
                  <Typography variant="body2" fontWeight={600} color="primary">
                    {formatCurrency(product.price)}
                  </Typography>
                </TableCell>
                <TableCell>
                  <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                    {product.colors && product.colors.length > 0 ? (
                      product.colors.slice(0, 3).map((color, index) => (
                        <Chip
                          key={`${product.id}-color-${index}`}
                          label={color}
                          size="small"
                          variant="outlined"
                        />
                      ))
                    ) : (
                      <Typography variant="body2" color="text.secondary">-</Typography>
                    )}
                    {product.colors && product.colors.length > 3 && (
                      <Chip
                        label={`+${product.colors.length - 3}`}
                        size="small"
                        variant="outlined"
                        color="primary"
                      />
                    )}
                  </Box>
                </TableCell>
                <TableCell>
                  <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                    {product.sizes && product.sizes.length > 0 ? (
                      product.sizes.slice(0, 3).map((size, index) => (
                        <Chip
                          key={`${product.id}-size-${index}`}
                          label={size}
                          size="small"
                          variant="outlined"
                        />
                      ))
                    ) : (
                      <Typography variant="body2" color="text.secondary">-</Typography>
                    )}
                    {product.sizes && product.sizes.length > 3 && (
                      <Chip
                        label={`+${product.sizes.length - 3}`}
                        size="small"
                        variant="outlined"
                        color="primary"
                      />
                    )}
                  </Box>
                </TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </TableContainer>
  )
}