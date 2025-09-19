import { useContext } from 'react'
import { ConfigContext } from '../providers/config-context'

// Custom hook for accessing configuration
export function useConfig() {
  return useContext(ConfigContext)
}