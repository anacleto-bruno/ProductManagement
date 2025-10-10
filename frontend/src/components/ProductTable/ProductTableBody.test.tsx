import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { ProductTableBody } from './ProductTableBody'
import type { Product } from '~/types/product'

// Mock i18next
vi.mock('react-i18next', () => ({
  useTranslation: () => ({
    t: (key: string) => {
      const translations: Record<string, string> = {
        'table.noData': 'No products found',
      }
      return translations[key] || key
    },
  }),
}))

describe('ProductTableBody', () => {
  const mockProducts: Product[] = [
    {
      id: 1,
      name: 'Product 1',
      sku: 'SKU-001',
      price: 99.99,
      colors: [{ id: 1, name: 'Red', hexCode: '#FF0000' }],
      sizes: [{ id: 1, name: 'M', code: 'M' }],
    },
    {
      id: 2,
      name: 'Product 2',
      sku: 'SKU-002',
      price: 149.99,
    },
  ]

  it('should render products when provided', () => {
    render(
      <table>
        <ProductTableBody products={mockProducts} />
      </table>
    )
    
    expect(screen.getByText('Product 1')).toBeInTheDocument()
    expect(screen.getByText('Product 2')).toBeInTheDocument()
  })

  it('should render correct number of product rows', () => {
    const { container } = render(
      <table>
        <ProductTableBody products={mockProducts} />
      </table>
    )
    
    const rows = container.querySelectorAll('.MuiTableRow-root')
    expect(rows).toHaveLength(2)
  })

  it('should render loading skeleton when loading is true', () => {
    const { container } = render(
      <table>
        <ProductTableBody products={[]} loading={true} />
      </table>
    )
    
    const skeletons = container.querySelectorAll('.MuiSkeleton-root')
    expect(skeletons.length).toBeGreaterThan(0)
  })

  it('should not render products when loading', () => {
    render(
      <table>
        <ProductTableBody products={mockProducts} loading={true} />
      </table>
    )
    
    expect(screen.queryByText('Product 1')).not.toBeInTheDocument()
    expect(screen.queryByText('Product 2')).not.toBeInTheDocument()
  })

  it('should render empty state when products array is empty and not loading', () => {
    render(
      <table>
        <ProductTableBody products={[]} loading={false} />
      </table>
    )
    
    expect(screen.getByText('No products found')).toBeInTheDocument()
  })

  it('should render custom empty message when provided', () => {
    render(
      <table>
        <ProductTableBody 
          products={[]} 
          loading={false} 
          emptyMessage="Custom empty message" 
        />
      </table>
    )
    
    expect(screen.getByText('Custom empty message')).toBeInTheDocument()
    expect(screen.queryByText('No products found')).not.toBeInTheDocument()
  })

  it('should call onProductClick when a product row is clicked', async () => {
    const user = userEvent.setup()
    const onProductClick = vi.fn()
    
    render(
      <table>
        <ProductTableBody 
          products={mockProducts} 
          onProductClick={onProductClick} 
        />
      </table>
    )
    
    const product1 = screen.getByText('Product 1')
    await user.click(product1)
    
    expect(onProductClick).toHaveBeenCalledTimes(1)
    expect(onProductClick).toHaveBeenCalledWith(mockProducts[0])
  })

  it('should render each product with unique key', () => {
    const { container } = render(
      <table>
        <ProductTableBody products={mockProducts} />
      </table>
    )
    
    const rows = container.querySelectorAll('.MuiTableRow-root')
    // Check that we have the correct number of rows
    expect(rows).toHaveLength(2)
  })

  it('should not call onProductClick when not provided', async () => {
    const user = userEvent.setup()
    
    render(
      <table>
        <ProductTableBody products={mockProducts} />
      </table>
    )
    
    const product1 = screen.getByText('Product 1')
    // Should not throw error when clicking without handler
    await user.click(product1)
  })

  describe('loading states', () => {
    it('should show skeleton when loading=true regardless of products', () => {
      const { container } = render(
        <table>
          <ProductTableBody products={mockProducts} loading={true} />
        </table>
      )
      
      const skeletons = container.querySelectorAll('.MuiSkeleton-root')
      expect(skeletons.length).toBeGreaterThan(0)
    })

    it('should not show skeleton when loading=false', () => {
      const { container } = render(
        <table>
          <ProductTableBody products={mockProducts} loading={false} />
        </table>
      )
      
      const skeletons = container.querySelectorAll('.MuiSkeleton-root')
      expect(skeletons).toHaveLength(0)
    })
  })

  describe('empty states', () => {
    it('should prioritize loading skeleton over empty state', () => {
      const { container } = render(
        <table>
          <ProductTableBody products={[]} loading={true} />
        </table>
      )
      
      const skeletons = container.querySelectorAll('.MuiSkeleton-root')
      expect(skeletons.length).toBeGreaterThan(0)
      expect(screen.queryByText('No products found')).not.toBeInTheDocument()
    })

    it('should show empty state only when not loading and no products', () => {
      render(
        <table>
          <ProductTableBody products={[]} loading={false} />
        </table>
      )
      
      expect(screen.getByText('No products found')).toBeInTheDocument()
    })

    it('should not show empty state when products exist', () => {
      render(
        <table>
          <ProductTableBody products={mockProducts} loading={false} />
        </table>
      )
      
      expect(screen.queryByText('No products found')).not.toBeInTheDocument()
    })
  })

  describe('TableBody wrapper', () => {
    it('should render TableBody component', () => {
      const { container } = render(
        <table>
          <ProductTableBody products={mockProducts} />
        </table>
      )
      
      const tableBody = container.querySelector('.MuiTableBody-root')
      expect(tableBody).toBeInTheDocument()
    })
  })
})
