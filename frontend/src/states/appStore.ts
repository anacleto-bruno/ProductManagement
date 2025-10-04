import { create } from 'zustand'
import { devtools } from 'zustand/middleware'
import type { ThemeMode, Notification } from '~/types/common'

interface AppState {
  // Theme management
  theme: ThemeMode
  toggleTheme: () => void
  setTheme: (theme: ThemeMode) => void

  // Notification management
  notifications: Notification[]
  addNotification: (notification: Omit<Notification, 'id'>) => void
  removeNotification: (id: string) => void
  clearNotifications: () => void

  // Global loading states
  isLoading: boolean
  setLoading: (loading: boolean) => void

  // Sidebar/drawer state for mobile
  sidebarOpen: boolean
  setSidebarOpen: (open: boolean) => void
  toggleSidebar: () => void
}

export const useAppStore = create<AppState>()(
  devtools(
    (set, get) => ({
      // Theme state
      theme: 'light',
      toggleTheme: () => 
        set((state) => ({ 
          theme: state.theme === 'light' ? 'dark' : 'light' 
        }), false, 'toggleTheme'),
      setTheme: (theme) => 
        set({ theme }, false, 'setTheme'),

      // Notification state
      notifications: [],
      addNotification: (notification) => {
        const id = Date.now().toString()
        const newNotification = { ...notification, id }
        set(
          (state) => ({ 
            notifications: [...state.notifications, newNotification] 
          }), 
          false, 
          'addNotification'
        )
        
        // Auto-remove notification after specified duration
        if (notification.duration !== 0) {
          setTimeout(() => {
            get().removeNotification(id)
          }, notification.duration || 5000)
        }
      },
      removeNotification: (id) =>
        set(
          (state) => ({
            notifications: state.notifications.filter((n) => n.id !== id)
          }),
          false,
          'removeNotification'
        ),
      clearNotifications: () =>
        set({ notifications: [] }, false, 'clearNotifications'),

      // Global loading state
      isLoading: false,
      setLoading: (loading) => 
        set({ isLoading: loading }, false, 'setLoading'),

      // Sidebar state
      sidebarOpen: false,
      setSidebarOpen: (open) => 
        set({ sidebarOpen: open }, false, 'setSidebarOpen'),
      toggleSidebar: () => 
        set((state) => ({ sidebarOpen: !state.sidebarOpen }), false, 'toggleSidebar'),
    }),
    { name: 'app-store' }
  )
)