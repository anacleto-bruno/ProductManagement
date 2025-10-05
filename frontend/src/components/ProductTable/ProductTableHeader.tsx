import React from 'react'
import { TableHead, TableRow, TableCell } from '@mui/material'
import { useTranslation } from 'react-i18next'

interface ProductTableHeaderProps {
  columns?: string[]
}

const DEFAULT_COLUMNS = [
  'name',
  'model',
  'brand',
  'sku',
  'price',
  'colors',
  'sizes'
]

export const ProductTableHeader: React.FC<ProductTableHeaderProps> = ({ 
  columns = DEFAULT_COLUMNS 
}) => {
  const { t } = useTranslation('products')

  return (
    <TableHead>
      <TableRow>
        {columns.map((column) => (
          <TableCell 
            key={column}
            sx={{ fontWeight: 600 }}
            align={column === 'price' ? 'right' : 'left'}
          >
            {t(column)}
          </TableCell>
        ))}
      </TableRow>
    </TableHead>
  )
}