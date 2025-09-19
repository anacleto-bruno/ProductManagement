import { z } from 'zod'

/**
 * Environment variable schema validation using Zod
 * This ensures that all required environment variables are present and correctly typed
 */
const envSchema = z.object({
  VITE_API_URL: z.string().url().default('http://localhost:7071/api'),
  VITE_APP_NAME: z.string().default('Product Management'),
})

// Validate and parse the environment variables
const processEnv = {
  VITE_API_URL: import.meta.env.VITE_API_URL,
  VITE_APP_NAME: import.meta.env.VITE_APP_NAME,
}

export const env = envSchema.parse(processEnv)

// Define type for environment variables
export type Env = z.infer<typeof envSchema>