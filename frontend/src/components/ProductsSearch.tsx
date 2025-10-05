import React, { useRef, useEffect } from 'react'
import { 
  Paper, 
  Box,
  Button,
  TextField,
  InputAdornment,
  CircularProgress,
  Chip,
} from '@mui/material'
import { 
  Search as SearchIcon,
  Clear as ClearIcon,
} from '@mui/icons-material'
import { useTranslation } from 'react-i18next'

interface ProductsSearchProps {
  search: string
  onSearchChange: (value: string) => void
  onClearSearch: () => void
  isSearching: boolean
  hasSearch: boolean
  totalCount: number
}

export const ProductsSearch: React.FC<ProductsSearchProps> = ({
  search,
  onSearchChange,
  onClearSearch,
  isSearching,
  hasSearch,
  totalCount,
}) => {
  const { t } = useTranslation('products')
  const { t: tCommon } = useTranslation('common')
  const searchInputRef = useRef<HTMLInputElement>(null)

  // Keyboard shortcut for search focus (Ctrl+K / Cmd+K)
  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if ((event.ctrlKey || event.metaKey) && event.key === 'k') {
        event.preventDefault()
        searchInputRef.current?.focus()
      }
    }

    document.addEventListener('keydown', handleKeyDown)
    return () => document.removeEventListener('keydown', handleKeyDown)
  }, [])

  return (
    <Paper sx={{ p: 3, mb: 3 }}>
      <Box display="flex" gap={2} alignItems="center">
        <TextField
          fullWidth
          inputRef={searchInputRef}
          placeholder={`${t('searchPlaceholder')} (Ctrl+K)`}
          value={search}
          onChange={(e) => onSearchChange(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === 'Escape') {
              onClearSearch()
              searchInputRef.current?.blur()
            }
          }}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon />
              </InputAdornment>
            ),
            endAdornment: hasSearch ? (
              <InputAdornment position="end">
                <Button
                  size="small"
                  onClick={onClearSearch}
                  startIcon={<ClearIcon />}
                  sx={{ minWidth: 'auto' }}
                >
                  {tCommon('clear')}
                </Button>
              </InputAdornment>
            ) : null,
          }}
        />
        
        {isSearching && (
          <CircularProgress size={24} />
        )}
      </Box>

      {hasSearch && (
        <Box mt={2} display="flex" gap={1} alignItems="center">
          <Chip
            label={`${tCommon('search')}: "${search}"`}
            onDelete={onClearSearch}
            variant="outlined"
            color="primary"
          />
          {!isSearching && (
            <Chip
              label={`${totalCount} ${totalCount === 1 ? 'result' : 'results'}`}
              variant="filled"
              color={totalCount > 0 ? 'success' : 'default'}
              size="small"
            />
          )}
        </Box>
      )}
    </Paper>
  )
}