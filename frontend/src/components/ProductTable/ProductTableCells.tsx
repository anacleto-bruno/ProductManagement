import React from 'react'
import { TableCell, Typography, Box, Chip } from '@mui/material'
import { formatCurrency } from '~/utils/common'
import { getLightColor, getTextColor } from '~/utils/colorHelpers'
import type { Color } from '~/types/product'

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

interface ColorChipsCellProps {
  colors: Color[] | undefined
  maxVisible?: number
}

export const ColorChipsCell: React.FC<ColorChipsCellProps> = ({ 
  colors, 
  maxVisible = 3 
}) => (
  <TableCell>
    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
      {colors && colors.length > 0 ? (
        <>
          {colors.slice(0, maxVisible).map((color) => {
            const lightBg = getLightColor(color.hexCode)
            const textColor = getTextColor(color.hexCode)
            
            return (
              <Chip
                key={color.id}
                label={color.name}
                size="small"
                sx={{
                  backgroundColor: lightBg,
                  color: textColor,
                  border: `1px solid ${color.hexCode || '#ccc'}`,
                  fontWeight: 500,
                  '& .MuiChip-label': {
                    px: 1.5,
                  }
                }}
              />
            )
          })}
          {colors.length > maxVisible && (
            <Chip
              label={`+${colors.length - maxVisible}`}
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