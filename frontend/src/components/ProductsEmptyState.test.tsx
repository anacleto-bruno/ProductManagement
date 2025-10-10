import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { ProductsEmptyState } from './ProductsEmptyState'

// Mock i18next
vi.mock('react-i18next', () => ({
  useTranslation: () => ({
    t: (key: string) => {
      const translations: Record<string, string> = {
        'seedData': 'Seed Data',
        'addProduct': 'Add Product',
        'clear': 'Clear',
        'search': 'Search',
      }
      return translations[key] || key
    },
  }),
}))

describe('ProductsEmptyState', () => {
  const defaultProps = {
    hasSearch: false,
    searchTerm: '',
    onClearSearch: vi.fn(),
    onAddProduct: vi.fn(),
    onSeedProducts: vi.fn(),
    isSeedingProducts: false,
  }

  describe('when there are no products and no search', () => {
    it('should display "No products found" message', () => {
      render(<ProductsEmptyState {...defaultProps} />)
      
      expect(screen.getByText('No products found')).toBeInTheDocument()
    })

    it('should display helpful subtitle', () => {
      render(<ProductsEmptyState {...defaultProps} />)
      
      expect(
        screen.getByText(/get started by seeding some sample data/i)
      ).toBeInTheDocument()
    })

    it('should render Seed Data button', () => {
      render(<ProductsEmptyState {...defaultProps} />)
      
      const seedButton = screen.getByRole('button', { name: /seed data/i })
      expect(seedButton).toBeInTheDocument()
    })

    it('should render Add Product button', () => {
      render(<ProductsEmptyState {...defaultProps} />)
      
      const addButton = screen.getByRole('button', { name: /add product/i })
      expect(addButton).toBeInTheDocument()
    })

    it('should call onSeedProducts when Seed Data button is clicked', async () => {
      const user = userEvent.setup()
      const onSeedProducts = vi.fn()
      
      render(<ProductsEmptyState {...defaultProps} onSeedProducts={onSeedProducts} />)
      
      const seedButton = screen.getByRole('button', { name: /seed data/i })
      await user.click(seedButton)
      
      expect(onSeedProducts).toHaveBeenCalledTimes(1)
    })

    it('should call onAddProduct when Add Product button is clicked', async () => {
      const user = userEvent.setup()
      const onAddProduct = vi.fn()
      
      render(<ProductsEmptyState {...defaultProps} onAddProduct={onAddProduct} />)
      
      const addButton = screen.getByRole('button', { name: /add product/i })
      await user.click(addButton)
      
      expect(onAddProduct).toHaveBeenCalledTimes(1)
    })

    it('should disable Seed Data button when seeding is in progress', () => {
      render(<ProductsEmptyState {...defaultProps} isSeedingProducts={true} />)
      
      const seedButton = screen.getByRole('button', { name: /seed data/i })
      expect(seedButton).toBeDisabled()
    })
  })

  describe('when there are no search results', () => {
    const searchProps = {
      ...defaultProps,
      hasSearch: true,
      searchTerm: 'laptop',
    }

    it('should display "No results found" message with search term', () => {
      render(<ProductsEmptyState {...searchProps} />)
      
      expect(screen.getByText(/no results found for "laptop"/i)).toBeInTheDocument()
    })

    it('should display helpful subtitle for search', () => {
      render(<ProductsEmptyState {...searchProps} />)
      
      expect(
        screen.getByText(/try adjusting your search criteria/i)
      ).toBeInTheDocument()
    })

    it('should render Clear Search button', () => {
      render(<ProductsEmptyState {...searchProps} />)
      
      const clearButton = screen.getByRole('button', { name: /clear search/i })
      expect(clearButton).toBeInTheDocument()
    })

    it('should render Seed Data button in search state', () => {
      render(<ProductsEmptyState {...searchProps} />)
      
      const seedButton = screen.getByRole('button', { name: /seed data/i })
      expect(seedButton).toBeInTheDocument()
    })

    it('should call onClearSearch when Clear Search button is clicked', async () => {
      const user = userEvent.setup()
      const onClearSearch = vi.fn()
      
      render(<ProductsEmptyState {...searchProps} onClearSearch={onClearSearch} />)
      
      const clearButton = screen.getByRole('button', { name: /clear search/i })
      await user.click(clearButton)
      
      expect(onClearSearch).toHaveBeenCalledTimes(1)
    })

    it('should call onSeedProducts when Seed Data button is clicked in search state', async () => {
      const user = userEvent.setup()
      const onSeedProducts = vi.fn()
      
      render(<ProductsEmptyState {...searchProps} onSeedProducts={onSeedProducts} />)
      
      const seedButton = screen.getByRole('button', { name: /seed data/i })
      await user.click(seedButton)
      
      expect(onSeedProducts).toHaveBeenCalledTimes(1)
    })

    it('should disable Seed Data button when seeding in search state', () => {
      render(<ProductsEmptyState {...searchProps} isSeedingProducts={true} />)
      
      const seedButton = screen.getByRole('button', { name: /seed data/i })
      expect(seedButton).toBeDisabled()
    })

    it('should not show Add Product button in search state', () => {
      render(<ProductsEmptyState {...searchProps} />)
      
      // In search state, only Clear Search and Seed Data buttons should be shown
      const addButton = screen.queryByRole('button', { name: /add product/i })
      expect(addButton).not.toBeInTheDocument()
    })
  })

  describe('conditional rendering based on hasSearch', () => {
    it('should render empty state when hasSearch is false', () => {
      render(<ProductsEmptyState {...defaultProps} hasSearch={false} />)
      
      expect(screen.getByText('No products found')).toBeInTheDocument()
      expect(screen.queryByText(/no results found for/i)).not.toBeInTheDocument()
    })

    it('should render search state when hasSearch is true and searchTerm is provided', () => {
      render(
        <ProductsEmptyState 
          {...defaultProps} 
          hasSearch={true} 
          searchTerm="test" 
        />
      )
      
      expect(screen.getByText(/no results found for "test"/i)).toBeInTheDocument()
      expect(screen.queryByText('No products found')).not.toBeInTheDocument()
    })
  })

  describe('button states', () => {
    it('should enable all buttons by default', () => {
      render(<ProductsEmptyState {...defaultProps} />)
      
      const seedButton = screen.getByRole('button', { name: /seed data/i })
      const addButton = screen.getByRole('button', { name: /add product/i })
      
      expect(seedButton).not.toBeDisabled()
      expect(addButton).not.toBeDisabled()
    })

    it('should only disable Seed Data button when seeding', () => {
      render(<ProductsEmptyState {...defaultProps} isSeedingProducts={true} />)
      
      const seedButton = screen.getByRole('button', { name: /seed data/i })
      const addButton = screen.getByRole('button', { name: /add product/i })
      
      expect(seedButton).toBeDisabled()
      expect(addButton).not.toBeDisabled()
    })
  })
})
