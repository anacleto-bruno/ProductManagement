import React from 'react'
import {
  Box,
  Pagination as MuiPagination,
  Typography,
  FormControl,
  Select,
  MenuItem,
} from '@mui/material'
import type { SelectChangeEvent } from '@mui/material'
import { useTranslation } from 'react-i18next'
import type { Pagination } from '~/types/product'

interface PaginationControlsProps {
  pagination: Pagination
  onPageChange: (page: number) => void
  onPageSizeChange: (pageSize: number) => void
  loading?: boolean
}

const PAGE_SIZE_OPTIONS = [10, 20, 50, 100]

export const PaginationControls: React.FC<PaginationControlsProps> = ({
  pagination,
  onPageChange,
  onPageSizeChange,
  loading = false,
}) => {
  const { t } = useTranslation('common')

  const handlePageSizeChange = (event: SelectChangeEvent<number>) => {
    const newPageSize = event.target.value as number
    onPageSizeChange(newPageSize)
  }

  const startItem = (pagination.page - 1) * pagination.pageSize + 1
  const endItem = Math.min(pagination.page * pagination.pageSize, pagination.totalCount)

  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        flexWrap: 'wrap',
        gap: 2,
        mt: 2,
        width: '100%',
        minWidth: '100%', // Prevents shrinking
      }}
    >
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
        <Typography variant="body2" color="text.secondary">
          {t('rows')} {t('page')}:
        </Typography>
        <FormControl size="small" disabled={loading}>
          <Select
            value={pagination.pageSize}
            onChange={handlePageSizeChange}
            variant="outlined"
            sx={{ minWidth: 80 }}
          >
            {PAGE_SIZE_OPTIONS.map((size) => (
              <MenuItem key={size} value={size}>
                {size}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
        
        <Typography variant="body2" color="text.secondary">
          {startItem}-{endItem} {t('of')} {pagination.totalCount}
        </Typography>
      </Box>

      <MuiPagination
        count={pagination.totalPages}
        page={pagination.page}
        onChange={(_, page) => onPageChange(page)}
        disabled={loading}
        variant="outlined"
        shape="rounded"
        showFirstButton
        showLastButton
        sx={{
          '& .MuiPagination-ul': {
            flexWrap: 'nowrap',
          },
        }}
      />
    </Box>
  )
}