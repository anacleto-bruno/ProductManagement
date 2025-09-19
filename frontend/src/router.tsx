import { createRootRoute, createRoute, createRouter } from '@tanstack/react-router'
import App from '~/App'
import { lazy } from 'react'

// Import routes with lazy loading for code splitting
const Home = lazy(() => import('~/pages/home/Home'))
const ProductList = lazy(() => import('~/pages/products/ProductList'))
const ProductDetail = lazy(() => import('~/pages/products/ProductDetail'))
const SearchPage = lazy(() => import('~/pages/search/SearchPage'))
const NotFound = lazy(() => import('~/pages/NotFound'))

// Define root route
const rootRoute = createRootRoute({
  component: App,
})

// Define child routes
const indexRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '/',
  component: Home,
})

const productsRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'products',
  component: ProductList,
})

const productRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'products/$productId',
  component: ProductDetail,
})

const searchRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: 'search',
  component: SearchPage,
})

const notFoundRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '*',
  component: NotFound,
})

// Create and export the router
const routeTree = rootRoute.addChildren([
  indexRoute,
  productsRoute,
  productRoute,
  searchRoute,
  notFoundRoute,
])

export const router = createRouter({ routeTree })

// Types
declare module '@tanstack/react-router' {
  interface Register {
    router: typeof router
  }
}