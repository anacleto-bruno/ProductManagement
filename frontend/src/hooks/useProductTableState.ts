import { useState, useMemo } from 'react'
import { debounce } from '~/utils/common'
import type { ProductQueryParams } from '~/api/ProductApi'

interface UseProductTableStateOptions {
  initialPage?: number
  initialPageSize?: number
  initialSearch?: string
  debounceMs?: number
}

export const useProductTableState = (options: UseProductTableStateOptions = {}) => {
  const {
    initialPage = 1,
    initialPageSize = 20,
    initialSearch = '',
    debounceMs = 300,
  } = options

  const [page, setPage] = useState(initialPage)
  const [pageSize, setPageSize] = useState(initialPageSize)
  const [search, setSearch] = useState(initialSearch)
  const [debouncedSearch, setDebouncedSearch] = useState(initialSearch)

  // Debounced search handler
  const debouncedSetSearch = useMemo(
    () => debounce((value: string) => {
      setDebouncedSearch(value)
      // Reset to first page when search changes
      setPage(1)
    }, debounceMs),
    [debounceMs]
  )

  // Handle search input change
  const handleSearchChange = (value: string) => {
    setSearch(value)
    debouncedSetSearch(value)
  }

  // Handle page change
  const handlePageChange = (newPage: number) => {
    setPage(newPage)
  }

  // Handle page size change
  const handlePageSizeChange = (newPageSize: number) => {
    setPageSize(newPageSize)
    // Reset to first page when page size changes
    setPage(1)
  }

  // Clear search
  const clearSearch = () => {
    setSearch('')
    setDebouncedSearch('')
    setPage(1)
  }

  // Reset all filters
  const resetFilters = () => {
    setSearch('')
    setDebouncedSearch('')
    setPage(initialPage)
    setPageSize(initialPageSize)
  }

  // Build query parameters for API
  const queryParams: ProductQueryParams = useMemo(() => {
    const params: ProductQueryParams = {
      page,
      pageSize,
    }

    if (debouncedSearch.trim()) {
      params.searchTerm = debouncedSearch.trim()
    }

    return params
  }, [page, pageSize, debouncedSearch])

  return {
    // State values
    page,
    pageSize,
    search,
    debouncedSearch,
    queryParams,
    
    // Handlers
    handleSearchChange,
    handlePageChange,
    handlePageSizeChange,
    clearSearch,
    resetFilters,
    
    // Computed properties
    isSearching: search !== debouncedSearch,
    hasSearch: debouncedSearch.trim().length > 0,
  }
}