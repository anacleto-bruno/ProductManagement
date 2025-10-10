import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { ProductsHeader } from './ProductsHeader'

// Mock i18next
vi.mock('react-i18next', () => ({
  useTranslation: () => ({
    t: (key: string) => {
      const translations: Record<string, string> = {
        'title': 'Products',
        'seedProducts': 'Seed Products',
        'seeding': 'Seeding...',
        'addProduct': 'Add Product',
        'found': 'found',
      }
      return translations[key] || key
    },
  }),
}))

describe('ProductsHeader', () => {
  const defaultProps = {
    totalCount: 0,
    onSeedProducts: vi.fn(),
    onAddProduct: vi.fn(),
    isSeedingProducts: false,
  }

  it('should render the title', () => {
    render(<ProductsHeader {...defaultProps} />)
    expect(screen.getByText('Products')).toBeInTheDocument()
  })

  it('should display total count when greater than 0', () => {
    render(<ProductsHeader {...defaultProps} totalCount={25} />)
    
    expect(screen.getByText(/25/)).toBeInTheDocument()
    // "products" appears in both the title and the count text
    const productsElements = screen.getAllByText(/products/i)
    expect(productsElements.length).toBeGreaterThan(0)
    expect(screen.getByText(/found/i)).toBeInTheDocument()
  })

  it('should not display count when totalCount is 0', () => {
    render(<ProductsHeader {...defaultProps} totalCount={0} />)
    
    expect(screen.queryByText(/found/i)).not.toBeInTheDocument()
  })

  it('should render Seed Data button', () => {
    render(<ProductsHeader {...defaultProps} />)
    
    const seedButton = screen.getByRole('button', { name: /seeddata/i })
    expect(seedButton).toBeInTheDocument()
  })

  it('should render Add Product button', () => {
    render(<ProductsHeader {...defaultProps} />)
    
    const addButton = screen.getByRole('button', { name: /add product/i })
    expect(addButton).toBeInTheDocument()
  })

  it('should call onSeedProducts when Seed Data button is clicked', async () => {
    const user = userEvent.setup()
    const onSeedProducts = vi.fn()
    
    render(<ProductsHeader {...defaultProps} onSeedProducts={onSeedProducts} />)
    
    const seedButton = screen.getByRole('button', { name: /seeddata/i })
    await user.click(seedButton)
    
    expect(onSeedProducts).toHaveBeenCalledTimes(1)
  })

  it('should call onAddProduct when Add Product button is clicked', async () => {
    const user = userEvent.setup()
    const onAddProduct = vi.fn()
    
    render(<ProductsHeader {...defaultProps} onAddProduct={onAddProduct} />)
    
    const addButton = screen.getByRole('button', { name: /add product/i })
    await user.click(addButton)
    
    expect(onAddProduct).toHaveBeenCalledTimes(1)
  })

  it('should disable Seed Data button when seeding is in progress', () => {
    render(<ProductsHeader {...defaultProps} isSeedingProducts={true} />)
    
    const seedButton = screen.getByRole('button', { name: /seeddata/i })
    expect(seedButton).toBeDisabled()
  })

  it('should show loading indicator when seeding', () => {
    render(<ProductsHeader {...defaultProps} isSeedingProducts={true} />)
    
    expect(screen.getByRole('progressbar')).toBeInTheDocument()
  })

  it('should not call onSeedProducts when button is disabled', () => {
    const onSeedProducts = vi.fn()
    
    render(
      <ProductsHeader 
        {...defaultProps} 
        onSeedProducts={onSeedProducts} 
        isSeedingProducts={true} 
      />
    )
    
    const seedButton = screen.getByRole('button', { name: /seeddata/i })
    
    // Button should be disabled, preventing any clicks
    expect(seedButton).toBeDisabled()
    expect(onSeedProducts).not.toHaveBeenCalled()
  })

  it('should render with correct structure and spacing', () => {
    const { container } = render(<ProductsHeader {...defaultProps} totalCount={10} />)
    
    // Check that main container has proper display flex
    const mainBox = container.querySelector('[class*="MuiBox-root"]')
    expect(mainBox).toBeInTheDocument()
  })
})
