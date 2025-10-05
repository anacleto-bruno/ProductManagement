import React from 'react'
import { TableBody } from '@mui/material'
import type { Product } from '~/types/product'
import { ProductTableRow } from './ProductTableRow'
import { TableSkeleton, TableEmptyState } from './ProductTableStates'

interface ProductTableBodyProps {
  products: Product[]
  loading?: boolean
  onProductClick?: (product: Product) => void
  emptyMessage?: string
}

export const ProductTableBody: React.FC<ProductTableBodyProps> = ({
  products,
  loading = false,
  onProductClick,
  emptyMessage,
}) => {
  return (
    <TableBody>
      {loading ? (
        <TableSkeleton />
      ) : products.length === 0 ? (
        <TableEmptyState message={emptyMessage} />
      ) : (
        products.map((product) => (
          <ProductTableRow
            key={product.id}
            product={product}
            onClick={onProductClick}
          />
        ))
      )}
    </TableBody>
  )
}