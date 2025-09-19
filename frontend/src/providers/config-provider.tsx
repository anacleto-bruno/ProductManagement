import { ConfigContext } from './config-context'
import { env } from '~/utils/env'

// Provider component
export function ConfigProvider({ children }: { children: React.ReactNode }) {
  return (
    <ConfigContext.Provider value={env}>
      {children}
    </ConfigContext.Provider>
  )
}