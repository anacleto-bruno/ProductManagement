import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { RouterProvider } from '@tanstack/react-router'
import { QueryProvider } from './providers/query-provider'
import { env } from './utils/env'
import './i18n' // Import i18n configuration
import './globals.css'

import { router } from './router'

// Validate environment variables at startup
console.info(`App: ${env.VITE_APP_NAME}`)
console.info(`API URL: ${env.VITE_API_URL}`)

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryProvider>
      <RouterProvider router={router} />
    </QueryProvider>
  </StrictMode>,
)
