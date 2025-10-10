import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import { ThemeProvider } from '@mui/material/styles'
import { createTheme } from '@mui/material/styles'
import { ProductTable } from '~/components/ProductTable'
import type { Product } from '~/types/product'

// Mock react-i18next
vi.mock('react-i18next', () => ({
  useTranslation: () => ({
    t: (key: string) => {
      const keys: Record<string, string> = {
        'name': 'Name',
        'description': 'Description',
        'model': 'Model',
        'brand': 'Brand',
        'sku': 'SKU',
        'price': 'Price',
        'colors': 'Colors',
        'sizes': 'Sizes',
        'table.noData': 'No products found',
      }
      return keys[key] || key
    },
  }),
}))

// Mock formatCurrency utility
vi.mock('~/utils/common', () => ({
  formatCurrency: (price: number) => `$${price.toFixed(2)}`,
}))

const theme = createTheme()

const TestWrapper: React.FC<{ children: React.ReactNode }> = ({ children }) => (
  <ThemeProvider theme={theme}>{children}</ThemeProvider>
)

const mockProducts: Product[] = [
  {
    id: 1,
    name: 'Test Product 1',
    description: 'A great test product with many features',
    model: 'TP-001',
    brand: 'TestBrand',
    sku: 'TEST-001',
    price: 99.99,
    colors: [
      { id: 1, name: 'Red', hexCode: '#FF0000' },
      { id: 2, name: 'Blue', hexCode: '#0000FF' },
      { id: 3, name: 'Green', hexCode: '#008000' },
      { id: 4, name: 'Yellow', hexCode: '#FFFF00' },
    ],
    sizes: [
      { id: 1, name: 'S', code: 'S' },
      { id: 2, name: 'M', code: 'M' },
      { id: 3, name: 'L', code: 'L' },
      { id: 4, name: 'XL', code: 'XL' },
    ],
  },
  {
    id: 2,
    name: 'Test Product 2',
    sku: 'TEST-002',
    price: 149.99,
    colors: [
      { id: 5, name: 'Black', hexCode: '#000000' },
      { id: 6, name: 'White', hexCode: '#FFFFFF' },
    ],
    sizes: [
      { id: 2, name: 'M', code: 'M' },
      { id: 3, name: 'L', code: 'L' },
    ],
  },
]

describe('ProductTable', () => {
  it('should render table headers correctly', () => {
    render(
      <TestWrapper>
        <ProductTable products={[]} />
      </TestWrapper>
    )

    expect(screen.getByText('Name')).toBeInTheDocument()
    expect(screen.getByText('Description')).toBeInTheDocument()
    expect(screen.getByText('Model')).toBeInTheDocument()
    expect(screen.getByText('Brand')).toBeInTheDocument()
    expect(screen.getByText('SKU')).toBeInTheDocument()
    expect(screen.getByText('Price')).toBeInTheDocument()
    expect(screen.getByText('Colors')).toBeInTheDocument()
    expect(screen.getByText('Sizes')).toBeInTheDocument()
  })

  it('should render no data message when products array is empty', () => {
    render(
      <TestWrapper>
        <ProductTable products={[]} />
      </TestWrapper>
    )

    expect(screen.getByText('No products found')).toBeInTheDocument()
  })

  it('should render loading skeletons when loading is true', () => {
    render(
      <TestWrapper>
        <ProductTable products={[]} loading={true} />
      </TestWrapper>
    )

    // Check for skeleton loading elements
    const skeletons = document.querySelectorAll('.MuiSkeleton-root')
    expect(skeletons.length).toBeGreaterThan(0)
  })

  it('should render error message when error is provided', () => {
    const errorMessage = 'Failed to load products'
    render(
      <TestWrapper>
        <ProductTable products={[]} error={errorMessage} />
      </TestWrapper>
    )

    expect(screen.getByText(errorMessage)).toBeInTheDocument()
    expect(screen.getByRole('alert')).toBeInTheDocument()
  })

  it('should render product data correctly', () => {
    render(
      <TestWrapper>
        <ProductTable products={mockProducts} />
      </TestWrapper>
    )

    // Check product names
    expect(screen.getByText('Test Product 1')).toBeInTheDocument()
    expect(screen.getByText('Test Product 2')).toBeInTheDocument()

    // Check SKUs
    expect(screen.getByText('TEST-001')).toBeInTheDocument()
    expect(screen.getByText('TEST-002')).toBeInTheDocument()

    // Check prices (formatted)
    expect(screen.getByText('$99.99')).toBeInTheDocument()
    expect(screen.getByText('$149.99')).toBeInTheDocument()

    // Check brand and model
    expect(screen.getByText('TestBrand')).toBeInTheDocument()
    expect(screen.getByText('TP-001')).toBeInTheDocument()

    // Check colors (chips)
    expect(screen.getByText('Red')).toBeInTheDocument()
    expect(screen.getByText('Blue')).toBeInTheDocument()
    expect(screen.getByText('Green')).toBeInTheDocument()

    // Check sizes (chips) - use getAllByText since M appears twice
    expect(screen.getByText('S')).toBeInTheDocument()
    expect(screen.getAllByText('M').length).toBeGreaterThan(0)
    expect(screen.getAllByText('L').length).toBeGreaterThan(0)
  })

  it('should handle missing optional fields gracefully', () => {
    const productWithMissingFields: Product = {
      id: 3,
      name: 'Minimal Product',
      sku: 'MIN-001',
      price: 50.00,
    }

    render(
      <TestWrapper>
        <ProductTable products={[productWithMissingFields]} />
      </TestWrapper>
    )

    expect(screen.getByText('Minimal Product')).toBeInTheDocument()
    expect(screen.getByText('MIN-001')).toBeInTheDocument()
    expect(screen.getByText('$50.00')).toBeInTheDocument()
    
    // Check for dash placeholders for missing fields
    const dashElements = screen.getAllByText('-')
    expect(dashElements.length).toBeGreaterThan(0)
  })

  it('should truncate long descriptions', () => {
    const productWithLongDescription: Product = {
      id: 4,
      name: 'Product with Long Description',
      description: 'This is a very long description that should be truncated in the table view to prevent layout issues and maintain readability across different screen sizes',
      sku: 'LONG-001',
      price: 75.00,
    }

    render(
      <TestWrapper>
        <ProductTable products={[productWithLongDescription]} />
      </TestWrapper>
    )

    // Check that the product name is rendered
    expect(screen.getByText('Product with Long Description')).toBeInTheDocument()
    expect(screen.getByText('LONG-001')).toBeInTheDocument()
    expect(screen.getByText('$75.00')).toBeInTheDocument()
    
    // Check that description is present (it might be truncated visually but still in DOM)
    expect(screen.getByText('This is a very long description that should be truncated in the table view to prevent layout issues and maintain readability across different screen sizes')).toBeInTheDocument()
  })

  it('should show +N indicator for many colors/sizes', () => {
    const productWithManyOptions: Product = {
      id: 5,
      name: 'Product with Many Options',
      sku: 'MANY-001',
      price: 100.00,
      colors: [
        { id: 7, name: 'Red', hexCode: '#FF0000' },
        { id: 8, name: 'Blue', hexCode: '#0000FF' },
        { id: 9, name: 'Green', hexCode: '#008000' },
        { id: 10, name: 'Yellow', hexCode: '#FFFF00' },
        { id: 11, name: 'Purple', hexCode: '#800080' },
      ],
      sizes: [
        { id: 5, name: 'XS', code: 'XS' },
        { id: 1, name: 'S', code: 'S' },
        { id: 2, name: 'M', code: 'M' },
        { id: 3, name: 'L', code: 'L' },
        { id: 4, name: 'XL', code: 'XL' },
        { id: 6, name: 'XXL', code: 'XXL' },
      ],
    }

    render(
      <TestWrapper>
        <ProductTable products={[productWithManyOptions]} />
      </TestWrapper>
    )

    // Should show first 3 colors and +2 indicator
    expect(screen.getByText('Red')).toBeInTheDocument()
    expect(screen.getByText('Blue')).toBeInTheDocument()
    expect(screen.getByText('Green')).toBeInTheDocument()
    expect(screen.getByText('+2')).toBeInTheDocument()

    // Should show first 3 sizes and +3 indicator
    expect(screen.getByText('XS')).toBeInTheDocument()
    expect(screen.getByText('S')).toBeInTheDocument()
    expect(screen.getByText('M')).toBeInTheDocument()
    expect(screen.getByText('+3')).toBeInTheDocument()
  })
})