import { QueryClient } from '@tanstack/react-query'

interface ApiError {
  status?: number
  message?: string
}

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minutes
      gcTime: 1000 * 60 * 10, // 10 minutes (formerly cacheTime)
      retry: (failureCount, error: unknown) => {
        const apiError = error as ApiError
        // Don't retry on 4xx errors except 408, 409, 429
        if (apiError?.status && apiError.status >= 400 && apiError.status < 500 && ![408, 409, 429].includes(apiError.status)) {
          return false
        }
        // Retry up to 3 times for other errors
        return failureCount < 3
      },
      refetchOnWindowFocus: false,
    },
    mutations: {
      retry: (failureCount, error: unknown) => {
        const apiError = error as ApiError
        // Don't retry mutations on client errors
        if (apiError?.status && apiError.status >= 400 && apiError.status < 500) {
          return false
        }
        // Retry up to 2 times for server errors
        return failureCount < 2
      },
    },
  },
})