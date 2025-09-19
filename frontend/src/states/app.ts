import { create } from 'zustand'
import { devtools } from 'zustand/middleware'

interface User {
  id: string
  name: string
  email: string
}

interface AppState {
  user: User | null
  theme: 'light' | 'dark'
  sidebarCollapsed: boolean
  setUser: (user: User | null) => void
  toggleTheme: () => void
  toggleSidebar: () => void
  setSidebarCollapsed: (collapsed: boolean) => void
}

export const useAppStore = create<AppState>()(
  devtools(
    (set) => ({
      user: null,
      theme: 'light',
      sidebarCollapsed: false,
      setUser: (user) => set({ user }, false, 'setUser'),
      toggleTheme: () => 
        set(
          (state) => ({ 
            theme: state.theme === 'light' ? 'dark' : 'light' 
          }), 
          false, 
          'toggleTheme'
        ),
      toggleSidebar: () =>
        set(
          (state) => ({
            sidebarCollapsed: !state.sidebarCollapsed
          }),
          false,
          'toggleSidebar'
        ),
      setSidebarCollapsed: (collapsed) => 
        set({ sidebarCollapsed: collapsed }, false, 'setSidebarCollapsed'),
    }),
    {
      name: 'app-store',
    }
  )
)