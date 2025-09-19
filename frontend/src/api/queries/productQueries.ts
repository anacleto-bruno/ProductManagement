/**
 * Query factory pattern for Product API endpoints
 * Following the coding rules for consistent query key management
 */
export const ProductQueries = {
  all: ['products'] as const,
  lists: () => [...ProductQueries.all, 'list'] as const,
  list: (filters: Record<string, unknown>) => [...ProductQueries.lists(), { filters }] as const,
  details: () => [...ProductQueries.all, 'detail'] as const,
  detail: (id: string) => [...ProductQueries.details(), id] as const,
  search: (query: string, filters: Record<string, unknown> = {}) => [
    ...ProductQueries.all,
    'search',
    { query, filters }
  ] as const,
} as const