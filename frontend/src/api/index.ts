import axios from 'axios'
import type { InternalAxiosRequestConfig, AxiosResponse, AxiosError } from 'axios'

// Base API configuration
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:7071/api'

export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000, // 10 seconds
})

// Request interceptor for adding auth tokens, logging, etc.
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // Add authorization token if available
    const token = localStorage.getItem('auth-token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    
    // Log requests in development
    if (import.meta.env.DEV) {
      console.log(`API Request: ${config.method?.toUpperCase()} ${config.url}`)
    }
    
    return config
  },
  (error: AxiosError) => {
    if (import.meta.env.DEV) {
      console.error('API Request Error:', error)
    }
    return Promise.reject(error)
  }
)

// Response interceptor for error handling and response transformation
apiClient.interceptors.response.use(
  (response: AxiosResponse) => {
    // Log successful responses in development
    if (import.meta.env.DEV) {
      console.log(`API Response: ${response.status} ${response.config.url}`)
    }
    return response
  },
  (error: AxiosError) => {
    // Enhanced error handling
    if (error.response) {
      // Server responded with error status
      const apiError = {
        status: error.response.status,
        message: (error.response.data as { message?: string })?.message || error.response.statusText,
        data: error.response.data,
      }
      
      // Handle specific status codes
      if (error.response.status === 401) {
        // Handle unauthorized - clear token and redirect to login
        localStorage.removeItem('auth-token')
        // Could dispatch logout action here
      }
      
      if (import.meta.env.DEV) {
        console.error('API Response Error:', apiError)
      }
      
      return Promise.reject(apiError)
    } else if (error.request) {
      // Request was made but no response received
      const networkError = {
        status: 0,
        message: 'Network error - please check your connection',
        data: null,
      }
      
      if (import.meta.env.DEV) {
        console.error('API Network Error:', networkError)
      }
      
      return Promise.reject(networkError)
    } else {
      // Something else happened
      if (import.meta.env.DEV) {
        console.error('API Error:', error.message)
      }
      
      return Promise.reject({
        status: 0,
        message: error.message || 'An unexpected error occurred',
        data: null,
      })
    }
  }
)

// Export query factories
export { ProductQueries } from './queries/productQueries'