import { z } from 'zod'

// Product related schemas and types
export const ProductSchema = z.object({
  id: z.string(),
  name: z.string(),
  description: z.string().optional(),
  model: z.string().optional(),
  brand: z.string().optional(),
  sku: z.string(),
  price: z.number(),
  colors: z.array(z.string()).optional(),
  sizes: z.array(z.string()).optional(),
  createdAt: z.string().optional(),
  updatedAt: z.string().optional(),
})

export const CreateProductSchema = ProductSchema.omit({ id: true, createdAt: true, updatedAt: true })
export const UpdateProductSchema = CreateProductSchema.partial()

export type Product = z.infer<typeof ProductSchema>
export type CreateProduct = z.infer<typeof CreateProductSchema>
export type UpdateProduct = z.infer<typeof UpdateProductSchema>

// Pagination types
export const PaginationSchema = z.object({
  page: z.number().min(1),
  pageSize: z.number().min(1).max(100),
  totalCount: z.number(),
  totalPages: z.number(),
})

export type Pagination = z.infer<typeof PaginationSchema>

// API Response types
export const ProductListResponseSchema = z.object({
  data: z.array(ProductSchema),
  pagination: PaginationSchema,
})

export type ProductListResponse = z.infer<typeof ProductListResponseSchema>

// Search and filter types
export interface ProductFilters {
  search?: string
  brand?: string
  minPrice?: number
  maxPrice?: number
  colors?: string[]
  sizes?: string[]
}

// API Error types
export interface ApiError {
  message: string
  status: number
  errors?: Record<string, string[]>
}