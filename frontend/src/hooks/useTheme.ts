import type { ThemeMode } from '~/types/common'

export interface ThemeContextType {
  mode: ThemeMode
  toggleTheme: () => void
}