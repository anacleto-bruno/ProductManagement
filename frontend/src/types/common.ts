// Common UI types
export interface SelectOption {
  value: string
  label: string
}

// Theme types
export type ThemeMode = 'light' | 'dark'

// Common component props
export interface BaseComponentProps {
  className?: string
  'data-testid'?: string
}

// Table types
export interface TableColumn<T = unknown> {
  id: string
  label: string
  minWidth?: number
  align?: 'left' | 'center' | 'right'
  format?: (value: T) => string | React.ReactNode
  sortable?: boolean
}

// Form types
export interface FormFieldError {
  message: string
}

export interface FormState<T = Record<string, unknown>> {
  values: T
  errors: Record<keyof T, FormFieldError | null>
  isSubmitting: boolean
  isDirty: boolean
}

// Loading states
export type LoadingState = 'idle' | 'loading' | 'success' | 'error'

// Dialog types
export interface ConfirmDialogProps {
  open: boolean
  title: string
  message: string
  onConfirm: () => void
  onCancel: () => void
}

// Notification types
export type NotificationType = 'success' | 'error' | 'warning' | 'info'

export interface Notification {
  id: string
  type: NotificationType
  message: string
  duration?: number
}