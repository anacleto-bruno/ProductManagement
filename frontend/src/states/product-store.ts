import { create } from 'zustand'

interface PaginationState {
  page: number
  perPage: number
  totalItems: number
  totalPages: number
}

interface ProductFilterState {
  searchTerm: string
  category: string | null
  brand: string | null
  minPrice: number | null
  maxPrice: number | null
}

interface ProductState {
  pagination: PaginationState
  filters: ProductFilterState
  setPagination: (pagination: Partial<PaginationState>) => void
  setFilters: (filters: Partial<ProductFilterState>) => void
  resetFilters: () => void
}

const defaultPagination: PaginationState = {
  page: 1,
  perPage: 20,
  totalItems: 0,
  totalPages: 0,
}

const defaultFilters: ProductFilterState = {
  searchTerm: '',
  category: null,
  brand: null,
  minPrice: null,
  maxPrice: null,
}

export const useProductStore = create<ProductState>()((set) => ({
  pagination: defaultPagination,
  filters: defaultFilters,
  setPagination: (pagination) =>
    set((state) => ({
      pagination: { ...state.pagination, ...pagination },
    })),
  setFilters: (filters) =>
    set((state) => ({
      filters: { ...state.filters, ...filters },
    })),
  resetFilters: () =>
    set({
      filters: defaultFilters,
      pagination: { ...defaultPagination, totalItems: 0, totalPages: 0 },
    }),
}))