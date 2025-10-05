import React from 'react'
import { TableRow } from '@mui/material'
import type { Product } from '~/types/product'
import {
  ProductNameCell,
  ProductTextCell,
  ProductPriceCell,
  ProductChipsCell,
} from './ProductTableCells'

interface ProductTableRowProps {
  product: Product
  onClick?: (product: Product) => void
}

export const ProductTableRow: React.FC<ProductTableRowProps> = ({ 
  product, 
  onClick 
}) => {
  const handleClick = () => {
    onClick?.(product)
  }

  return (
    <TableRow
      hover
      onClick={handleClick}
      sx={{ 
        '&:last-child td, &:last-child th': { border: 0 },
        cursor: onClick ? 'pointer' : 'default'
      }}
    >
      <ProductNameCell name={product.name} />
      <ProductTextCell value={product.model} />
      <ProductTextCell value={product.brand} fontWeight={500} />
      <ProductTextCell value={product.sku} fontFamily="monospace" />
      <ProductPriceCell price={product.price} />
      <ProductChipsCell items={product.colors} />
      <ProductChipsCell items={product.sizes} />
    </TableRow>
  )
}