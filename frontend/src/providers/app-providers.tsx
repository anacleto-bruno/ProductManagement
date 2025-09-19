import type { ReactNode } from 'react'
import { QueryProvider } from '~/providers/query-provider'
import { ConfigProvider } from '~/providers/config-provider'

// Import environment variables configuration
import '~/utils/env'

interface AppProvidersProps {
  children: ReactNode
}

/**
 * AppProviders component that wraps the application with all necessary providers
 */
export function AppProviders({ children }: AppProvidersProps) {
  return (
    <ConfigProvider>
      <QueryProvider>
        {children}
      </QueryProvider>
    </ConfigProvider>
  )
}