import React from 'react'
import { TableRow, TableCell, Skeleton, Typography, Alert } from '@mui/material'
import { useTranslation } from 'react-i18next'

interface TableSkeletonProps {
  rows?: number
  columns?: number
}

export const TableSkeleton: React.FC<TableSkeletonProps> = ({ 
  rows = 5, 
  columns = 7 
}) => (
  <>
    {Array.from({ length: rows }).map((_, index) => (
      <TableRow key={index}>
        {Array.from({ length: columns }).map((_, cellIndex) => (
          <TableCell key={cellIndex}>
            <Skeleton variant="text" width="100%" />
          </TableCell>
        ))}
      </TableRow>
    ))}
  </>
)

interface TableEmptyStateProps {
  colSpan?: number
  message?: string
}

export const TableEmptyState: React.FC<TableEmptyStateProps> = ({ 
  colSpan = 7,
  message 
}) => {
  const { t } = useTranslation('products')
  
  return (
    <TableRow>
      <TableCell colSpan={colSpan} align="center" sx={{ py: 4 }}>
        <Typography variant="body1" color="text.secondary">
          {message || t('table.noData')}
        </Typography>
      </TableCell>
    </TableRow>
  )
}

interface TableErrorStateProps {
  error: string
}

export const TableErrorState: React.FC<TableErrorStateProps> = ({ error }) => (
  <Alert severity="error" sx={{ mb: 2 }}>
    {error}
  </Alert>
)