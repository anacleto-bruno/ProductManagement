import { z } from 'zod'

// Color and Size schemas
export const ColorSchema = z.object({
  id: z.number(),
  name: z.string(),
  hexCode: z.string().nullable().optional(),
})

export const SizeSchema = z.object({
  id: z.number(),
  name: z.string(),
  code: z.string().nullable().optional(),
  sortOrder: z.number().nullable().optional(),
})

export type Color = z.infer<typeof ColorSchema>
export type Size = z.infer<typeof SizeSchema>

// Product related schemas and types
export const ProductSchema = z.object({
  id: z.number(),
  name: z.string(),
  description: z.string().optional(),
  model: z.string().optional(),
  brand: z.string().optional(),
  sku: z.string(),
  price: z.number(),
  colors: z.array(ColorSchema).optional(),
  sizes: z.array(SizeSchema).optional(),
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
  hasNextPage: z.boolean().optional(),
  hasPreviousPage: z.boolean().optional(),
})

export type Pagination = z.infer<typeof PaginationSchema>

// API Response types - matches backend structure
export const ProductListResponseSchema = z.object({
  data: z.array(ProductSchema),
  page: z.number().min(1),
  pageSize: z.number().min(1).max(100),
  totalCount: z.number(),
  totalPages: z.number(),
  hasNextPage: z.boolean().optional(),
  hasPreviousPage: z.boolean().optional(),
})

export type ProductListResponse = z.infer<typeof ProductListResponseSchema>

// Search and filter types
export interface ProductFilters {
  searchTerm?: string
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