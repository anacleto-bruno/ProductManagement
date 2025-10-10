import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import { TableSkeleton, TableEmptyState, TableErrorState } from './ProductTableStates'

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

describe('ProductTable States', () => {
  describe('TableSkeleton', () => {
    it('should render default number of skeleton rows', () => {
      const { container } = render(
        <table>
          <tbody>
            <TableSkeleton />
          </tbody>
        </table>
      )
      
      const skeletonRows = container.querySelectorAll('.MuiTableRow-root')
      expect(skeletonRows).toHaveLength(5) // Default rows
    })

    it('should render custom number of skeleton rows', () => {
      const { container } = render(
        <table>
          <tbody>
            <TableSkeleton rows={3} />
          </tbody>
        </table>
      )
      
      const skeletonRows = container.querySelectorAll('.MuiTableRow-root')
      expect(skeletonRows).toHaveLength(3)
    })

    it('should render default number of skeleton columns', () => {
      const { container } = render(
        <table>
          <tbody>
            <TableSkeleton />
          </tbody>
        </table>
      )
      
      const firstRow = container.querySelector('.MuiTableRow-root')
      const cells = firstRow?.querySelectorAll('.MuiTableCell-root')
      expect(cells).toHaveLength(7) // Default columns
    })

    it('should render custom number of skeleton columns', () => {
      const { container } = render(
        <table>
          <tbody>
            <TableSkeleton columns={4} />
          </tbody>
        </table>
      )
      
      const firstRow = container.querySelector('.MuiTableRow-root')
      const cells = firstRow?.querySelectorAll('.MuiTableCell-root')
      expect(cells).toHaveLength(4)
    })

    it('should render Skeleton components in each cell', () => {
      const { container } = render(
        <table>
          <tbody>
            <TableSkeleton rows={2} columns={3} />
          </tbody>
        </table>
      )
      
      const skeletons = container.querySelectorAll('.MuiSkeleton-root')
      expect(skeletons).toHaveLength(6) // 2 rows * 3 columns
    })

    it('should render text variant skeletons', () => {
      const { container } = render(
        <table>
          <tbody>
            <TableSkeleton rows={1} columns={1} />
          </tbody>
        </table>
      )
      
      const skeleton = container.querySelector('.MuiSkeleton-root')
      expect(skeleton).toHaveClass('MuiSkeleton-text')
    })
  })

  describe('TableEmptyState', () => {
    it('should render default empty message', () => {
      render(
        <table>
          <tbody>
            <TableEmptyState />
          </tbody>
        </table>
      )
      
      expect(screen.getByText('No products found')).toBeInTheDocument()
    })

    it('should render custom message when provided', () => {
      render(
        <table>
          <tbody>
            <TableEmptyState message="Custom empty message" />
          </tbody>
        </table>
      )
      
      expect(screen.getByText('Custom empty message')).toBeInTheDocument()
      expect(screen.queryByText('No products found')).not.toBeInTheDocument()
    })

    it('should render with default colSpan', () => {
      const { container } = render(
        <table>
          <tbody>
            <TableEmptyState />
          </tbody>
        </table>
      )
      
      const cell = container.querySelector('.MuiTableCell-root')
      expect(cell).toHaveAttribute('colspan', '7')
    })

    it('should render with custom colSpan', () => {
      const { container } = render(
        <table>
          <tbody>
            <TableEmptyState colSpan={5} />
          </tbody>
        </table>
      )
      
      const cell = container.querySelector('.MuiTableCell-root')
      expect(cell).toHaveAttribute('colspan', '5')
    })

    it('should center align the content', () => {
      const { container } = render(
        <table>
          <tbody>
            <TableEmptyState />
          </tbody>
        </table>
      )
      
      const cell = container.querySelector('.MuiTableCell-root')
      expect(cell).toHaveClass('MuiTableCell-alignCenter')
    })

    it('should render a single table row', () => {
      const { container } = render(
        <table>
          <tbody>
            <TableEmptyState />
          </tbody>
        </table>
      )
      
      const rows = container.querySelectorAll('.MuiTableRow-root')
      expect(rows).toHaveLength(1)
    })
  })

  describe('TableErrorState', () => {
    it('should render error message', () => {
      render(<TableErrorState error="Something went wrong" />)
      
      expect(screen.getByText('Something went wrong')).toBeInTheDocument()
    })

    it('should render error Alert with severity error', () => {
      const { container } = render(<TableErrorState error="Error occurred" />)
      
      const alert = container.querySelector('.MuiAlert-root')
      expect(alert).toBeInTheDocument()
      expect(alert).toHaveClass('MuiAlert-standardError')
    })

    it('should display different error messages', () => {
      const { rerender } = render(<TableErrorState error="First error" />)
      expect(screen.getByText('First error')).toBeInTheDocument()
      
      rerender(<TableErrorState error="Second error" />)
      expect(screen.getByText('Second error')).toBeInTheDocument()
      expect(screen.queryByText('First error')).not.toBeInTheDocument()
    })

    it('should render with error icon', () => {
      const { container } = render(<TableErrorState error="Error message" />)
      
      // MUI Alert with severity="error" includes an error icon
      const icon = container.querySelector('.MuiAlert-icon')
      expect(icon).toBeInTheDocument()
    })
  })
})
