import { createContext } from 'react'
import { env, type Env } from '~/utils/env'

// Create configuration context
export const ConfigContext = createContext<Env>(env)