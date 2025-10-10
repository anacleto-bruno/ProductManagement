import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import { ProductTableHeader } from './ProductTableHeader'

// Mock i18next
vi.mock('react-i18next', () => ({
  useTranslation: () => ({
    t: (key: string) => {
      const translations: Record<string, string> = {
        'name': 'Name',
        'description': 'Description',
        'model': 'Model',
        'brand': 'Brand',
        'sku': 'SKU',
        'price': 'Price',
        'colors': 'Colors',
        'sizes': 'Sizes',
      }
      return translations[key] || key
    },
  }),
}))

describe('ProductTableHeader', () => {
  it('should render all default column headers', () => {
    render(
      <table>
        <ProductTableHeader />
      </table>
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

  it('should render custom columns when provided', () => {
    const customColumns = ['name', 'price', 'sku']
    
    render(
      <table>
        <ProductTableHeader columns={customColumns} />
      </table>
    )
    
    expect(screen.getByText('Name')).toBeInTheDocument()
    expect(screen.getByText('Price')).toBeInTheDocument()
    expect(screen.getByText('SKU')).toBeInTheDocument()
    
    // Should not render columns not in the custom list
    expect(screen.queryByText('Model')).not.toBeInTheDocument()
    expect(screen.queryByText('Brand')).not.toBeInTheDocument()
    expect(screen.queryByText('Colors')).not.toBeInTheDocument()
  })

  it('should render correct number of column headers', () => {
    const { container } = render(
      <table>
        <ProductTableHeader />
      </table>
    )
    
    const headers = container.querySelectorAll('th')
    expect(headers).toHaveLength(8) // Default columns count
  })

  it('should render TableHead component', () => {
    const { container } = render(
      <table>
        <ProductTableHeader />
      </table>
    )
    
    const tableHead = container.querySelector('.MuiTableHead-root')
    expect(tableHead).toBeInTheDocument()
  })

  it('should render TableRow component', () => {
    const { container } = render(
      <table>
        <ProductTableHeader />
      </table>
    )
    
    const tableRow = container.querySelector('.MuiTableRow-root')
    expect(tableRow).toBeInTheDocument()
  })

  it('should align price column to the right', () => {
    render(
      <table>
        <ProductTableHeader />
      </table>
    )
    
    const priceHeader = screen.getByText('Price').closest('th')
    expect(priceHeader).toHaveClass('MuiTableCell-alignRight')
  })

  it('should align non-price columns to the left', () => {
    render(
      <table>
        <ProductTableHeader />
      </table>
    )
    
    const nameHeader = screen.getByText('Name').closest('th')
    expect(nameHeader).toHaveClass('MuiTableCell-alignLeft')
    
    const skuHeader = screen.getByText('SKU').closest('th')
    expect(skuHeader).toHaveClass('MuiTableCell-alignLeft')
  })

  it('should render with proper MUI classes', () => {
    const { container } = render(
      <table>
        <ProductTableHeader />
      </table>
    )
    
    const headers = container.querySelectorAll('.MuiTableCell-head')
    expect(headers.length).toBeGreaterThan(0)
    
    // Each header should be a TableCell in the head
    headers.forEach(header => {
      expect(header).toHaveClass('MuiTableCell-head')
    })
  })

  it('should handle empty custom columns array', () => {
    const { container } = render(
      <table>
        <ProductTableHeader columns={[]} />
      </table>
    )
    
    const headers = container.querySelectorAll('th')
    expect(headers).toHaveLength(0)
  })

  it('should render columns in the order specified', () => {
    const customColumns = ['sku', 'name', 'price']
    
    const { container } = render(
      <table>
        <ProductTableHeader columns={customColumns} />
      </table>
    )
    
    const headers = container.querySelectorAll('th')
    expect(headers[0]).toHaveTextContent('SKU')
    expect(headers[1]).toHaveTextContent('Name')
    expect(headers[2]).toHaveTextContent('Price')
  })
})
