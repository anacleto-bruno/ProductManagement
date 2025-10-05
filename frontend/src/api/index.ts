import axios from 'axios'
import type { AxiosResponse, AxiosError } from 'axios'
import type { ApiError } from '~/types/product'

// Create axios instance with base configuration
export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor for adding auth tokens, logging, etc.
apiClient.interceptors.request.use(
  (config) => {
    // Add timestamp for debugging
    if (import.meta.env.VITE_DEV_MODE === 'true') {
      console.log(`[API Request] ${config.method?.toUpperCase()} ${config.url}`, config.data)
    }
    
    // Add auth token if available
    // const token = localStorage.getItem('authToken')
    // if (token) {
    //   config.headers.Authorization = `Bearer ${token}`
    // }
    
    return config
  },
  (error) => {
    console.error('[API Request Error]', error)
    return Promise.reject(error)
  }
)

// Response interceptor for error handling and response transformation
apiClient.interceptors.response.use(
  (response: AxiosResponse) => {
    if (import.meta.env.VITE_DEV_MODE === 'true') {
      console.log(`[API Response] ${response.config.method?.toUpperCase()} ${response.config.url}`, response.data)
    }
    return response
  },
  (error: AxiosError) => {
    const apiError: ApiError = {
      message: 'An unexpected error occurred',
      status: error.response?.status || 500,
    }

    if (error.response?.data) {
      const errorData = error.response.data as Record<string, unknown>
      apiError.message = (errorData.message as string) || apiError.message
      apiError.errors = errorData.errors as Record<string, string[]>
    } else if (error.request) {
      apiError.message = 'Network error - please check your connection'
    }

    console.error('[API Response Error]', apiError)
    return Promise.reject(apiError)
  }
)

export default apiClient