import React from 'react'
import { TableCell, Typography, Box, Chip } from '@mui/material'
import { formatCurrency } from '~/utils/common'

interface ProductNameCellProps {
  name: string
}

export const ProductNameCell: React.FC<ProductNameCellProps> = ({ name }) => (
  <TableCell>
    <Typography variant="body2" fontWeight={500}>
      {name}
    </Typography>
  </TableCell>
)

interface ProductTextCellProps {
  value: string | null | undefined
  fontFamily?: string
  fontWeight?: number
}

export const ProductTextCell: React.FC<ProductTextCellProps> = ({ 
  value, 
  fontFamily,
  fontWeight 
}) => (
  <TableCell>
    <Typography 
      variant="body2" 
      fontFamily={fontFamily}
      fontWeight={fontWeight}
    >
      {value || '-'}
    </Typography>
  </TableCell>
)

interface ProductPriceCellProps {
  price: number
}

export const ProductPriceCell: React.FC<ProductPriceCellProps> = ({ price }) => (
  <TableCell align="right">
    <Typography variant="body2" fontWeight={600} color="primary">
      {formatCurrency(price)}
    </Typography>
  </TableCell>
)

interface ProductChipsCellProps {
  items: string[] | undefined
  maxVisible?: number
}

export const ProductChipsCell: React.FC<ProductChipsCellProps> = ({ 
  items, 
  maxVisible = 3 
}) => (
  <TableCell>
    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
      {items && items.length > 0 ? (
        <>
          {items.slice(0, maxVisible).map((item, index) => (
            <Chip
              key={index}
              label={item}
              size="small"
              variant="outlined"
            />
          ))}
          {items.length > maxVisible && (
            <Chip
              label={`+${items.length - maxVisible}`}
              size="small"
              variant="outlined"
              color="primary"
            />
          )}
        </>
      ) : (
        <Typography variant="body2" color="text.secondary">-</Typography>
      )}
    </Box>
  </TableCell>
)