import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import {
  ProductNameCell,
  ProductTextCell,
  ProductPriceCell,
  ProductChipsCell,
  ColorChipsCell,
} from './ProductTableCells'
import type { Color } from '~/types/product'

describe('ProductTable Cells', () => {
  describe('ProductNameCell', () => {
    it('should render product name', () => {
      render(
        <table>
          <tbody>
            <tr>
              <ProductNameCell name="Test Product" />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('Test Product')).toBeInTheDocument()
    })

    it('should render name with medium font weight', () => {
      const { container } = render(
        <table>
          <tbody>
            <tr>
              <ProductNameCell name="Test Product" />
            </tr>
          </tbody>
        </table>
      )
      
      const typography = container.querySelector('.MuiTypography-root')
      expect(typography).toBeInTheDocument()
    })
  })

  describe('ProductTextCell', () => {
    it('should render text value', () => {
      render(
        <table>
          <tbody>
            <tr>
              <ProductTextCell value="Sample Text" />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('Sample Text')).toBeInTheDocument()
    })

    it('should render dash when value is null', () => {
      render(
        <table>
          <tbody>
            <tr>
              <ProductTextCell value={null} />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('-')).toBeInTheDocument()
    })

    it('should render dash when value is undefined', () => {
      render(
        <table>
          <tbody>
            <tr>
              <ProductTextCell value={undefined} />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('-')).toBeInTheDocument()
    })

    it('should render dash when value is empty string', () => {
      render(
        <table>
          <tbody>
            <tr>
              <ProductTextCell value="" />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('-')).toBeInTheDocument()
    })

    it('should apply custom font family', () => {
      const { container } = render(
        <table>
          <tbody>
            <tr>
              <ProductTextCell value="Test" fontFamily="monospace" />
            </tr>
          </tbody>
        </table>
      )
      
      const typography = container.querySelector('.MuiTypography-root')
      expect(typography).toHaveStyle({ fontFamily: 'monospace' })
    })

    it('should apply custom font weight', () => {
      const { container } = render(
        <table>
          <tbody>
            <tr>
              <ProductTextCell value="Test" fontWeight={700} />
            </tr>
          </tbody>
        </table>
      )
      
      const typography = container.querySelector('.MuiTypography-root')
      expect(typography).toHaveStyle({ fontWeight: 700 })
    })
  })

  describe('ProductPriceCell', () => {
    it('should render formatted price', () => {
      render(
        <table>
          <tbody>
            <tr>
              <ProductPriceCell price={99.99} />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('$99.99')).toBeInTheDocument()
    })

    it('should render price with two decimal places', () => {
      render(
        <table>
          <tbody>
            <tr>
              <ProductPriceCell price={100} />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('$100.00')).toBeInTheDocument()
    })

    it('should align cell to the right', () => {
      const { container } = render(
        <table>
          <tbody>
            <tr>
              <ProductPriceCell price={50} />
            </tr>
          </tbody>
        </table>
      )
      
      const cell = container.querySelector('.MuiTableCell-root')
      expect(cell).toHaveClass('MuiTableCell-alignRight')
    })
  })

  describe('ProductChipsCell', () => {
    it('should render all items when count is within max visible', () => {
      const items = ['S', 'M', 'L']
      
      render(
        <table>
          <tbody>
            <tr>
              <ProductChipsCell items={items} />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('S')).toBeInTheDocument()
      expect(screen.getByText('M')).toBeInTheDocument()
      expect(screen.getByText('L')).toBeInTheDocument()
    })

    it('should show +N indicator when items exceed maxVisible', () => {
      const items = ['XS', 'S', 'M', 'L', 'XL']
      
      render(
        <table>
          <tbody>
            <tr>
              <ProductChipsCell items={items} maxVisible={3} />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('XS')).toBeInTheDocument()
      expect(screen.getByText('S')).toBeInTheDocument()
      expect(screen.getByText('M')).toBeInTheDocument()
      expect(screen.getByText('+2')).toBeInTheDocument()
      expect(screen.queryByText('L')).not.toBeInTheDocument()
      expect(screen.queryByText('XL')).not.toBeInTheDocument()
    })

    it('should render dash when items is undefined', () => {
      render(
        <table>
          <tbody>
            <tr>
              <ProductChipsCell items={undefined} />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('-')).toBeInTheDocument()
    })

    it('should render dash when items array is empty', () => {
      render(
        <table>
          <tbody>
            <tr>
              <ProductChipsCell items={[]} />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('-')).toBeInTheDocument()
    })

    it('should use custom maxVisible value', () => {
      const items = ['1', '2', '3', '4', '5']
      
      render(
        <table>
          <tbody>
            <tr>
              <ProductChipsCell items={items} maxVisible={2} />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('1')).toBeInTheDocument()
      expect(screen.getByText('2')).toBeInTheDocument()
      expect(screen.getByText('+3')).toBeInTheDocument()
    })
  })

  describe('ColorChipsCell', () => {
    const mockColors: Color[] = [
      { id: 1, name: 'Red', hexCode: '#FF0000' },
      { id: 2, name: 'Blue', hexCode: '#0000FF' },
      { id: 3, name: 'Green', hexCode: '#008000' },
    ]

    it('should render all colors when count is within max visible', () => {
      render(
        <table>
          <tbody>
            <tr>
              <ColorChipsCell colors={mockColors} />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('Red')).toBeInTheDocument()
      expect(screen.getByText('Blue')).toBeInTheDocument()
      expect(screen.getByText('Green')).toBeInTheDocument()
    })

    it('should show +N indicator when colors exceed maxVisible', () => {
      const manyColors: Color[] = [
        { id: 1, name: 'Red', hexCode: '#FF0000' },
        { id: 2, name: 'Blue', hexCode: '#0000FF' },
        { id: 3, name: 'Green', hexCode: '#008000' },
        { id: 4, name: 'Yellow', hexCode: '#FFFF00' },
        { id: 5, name: 'Purple', hexCode: '#800080' },
      ]
      
      render(
        <table>
          <tbody>
            <tr>
              <ColorChipsCell colors={manyColors} maxVisible={3} />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('Red')).toBeInTheDocument()
      expect(screen.getByText('Blue')).toBeInTheDocument()
      expect(screen.getByText('Green')).toBeInTheDocument()
      expect(screen.getByText('+2')).toBeInTheDocument()
      expect(screen.queryByText('Yellow')).not.toBeInTheDocument()
      expect(screen.queryByText('Purple')).not.toBeInTheDocument()
    })

    it('should render dash when colors is undefined', () => {
      render(
        <table>
          <tbody>
            <tr>
              <ColorChipsCell colors={undefined} />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('-')).toBeInTheDocument()
    })

    it('should render dash when colors array is empty', () => {
      render(
        <table>
          <tbody>
            <tr>
              <ColorChipsCell colors={[]} />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('-')).toBeInTheDocument()
    })

    it('should use custom maxVisible value', () => {
      const manyColors: Color[] = [
        { id: 1, name: 'Color1', hexCode: '#111111' },
        { id: 2, name: 'Color2', hexCode: '#222222' },
        { id: 3, name: 'Color3', hexCode: '#333333' },
        { id: 4, name: 'Color4', hexCode: '#444444' },
      ]
      
      render(
        <table>
          <tbody>
            <tr>
              <ColorChipsCell colors={manyColors} maxVisible={2} />
            </tr>
          </tbody>
        </table>
      )
      
      expect(screen.getByText('Color1')).toBeInTheDocument()
      expect(screen.getByText('Color2')).toBeInTheDocument()
      expect(screen.getByText('+2')).toBeInTheDocument()
      expect(screen.queryByText('Color3')).not.toBeInTheDocument()
    })

    it('should render color chips with color styling', () => {
      const { container } = render(
        <table>
          <tbody>
            <tr>
              <ColorChipsCell colors={mockColors} />
            </tr>
          </tbody>
        </table>
      )
      
      const chips = container.querySelectorAll('.MuiChip-root')
      // Should have 3 color chips (not counting the +N indicator)
      expect(chips.length).toBeGreaterThanOrEqual(3)
    })
  })
})
