import React from 'react'
import { Table, TableContainer, Paper } from '@mui/material'
import type { Product } from '~/types/product'
import { ProductTableHeader } from './ProductTable/ProductTableHeader'
import { ProductTableBody } from './ProductTable/ProductTableBody'
import { TableErrorState } from './ProductTable/ProductTableStates'



interface ProductTableProps {
  products: Product[]
  loading?: boolean
  error?: string | null
  onProductClick?: (product: Product) => void
  stickyHeader?: boolean
  columns?: string[]
}

export const ProductTable: React.FC<ProductTableProps> = ({
  products,
  loading = false,
  error = null,
  onProductClick,
  stickyHeader = true,
  columns,
}) => {
  if (error) {
    return <TableErrorState error={error} />
  }

  return (
    <TableContainer component={Paper} sx={{ mb: 3 }}>
      <Table stickyHeader={stickyHeader}>
        <ProductTableHeader columns={columns} />
        <ProductTableBody
          products={products}
          loading={loading}
          onProductClick={onProductClick}
        />
      </Table>
    </TableContainer>
  )
}