import React from 'react'
import { ChevronLeft, ChevronRight, MoreHorizontal } from 'lucide-react'
import { cn } from '~/utils/cn'
import { Button } from './button'

/**
 * Pagination component props interface
 */
export interface PaginationProps {
  /** Current page number (1-indexed) */
  currentPage: number
  /** Total number of pages */
  totalPages: number
  /** Total number of items */
  totalItems: number
  /** Items per page */
  perPage: number
  /** Callback fired when page changes */
  onPageChange: (page: number) => void
  /** Callback fired when items per page changes */
  onPerPageChange?: (perPage: number) => void
  /** Available options for items per page */
  perPageOptions?: number[]
  /** Show items per page selector */
  showPerPageSelector?: boolean
  /** Show page info text */
  showPageInfo?: boolean
  /** Maximum number of page buttons to show */
  maxPageButtons?: number
}

/**
 * Pagination component for navigating through paginated data
 * 
 * @example
 * ```tsx
 * <Pagination
 *   currentPage={1}
 *   totalPages={10}
 *   totalItems={200}
 *   perPage={20}
 *   onPageChange={(page) => setCurrentPage(page)}
 *   onPerPageChange={(perPage) => setPerPage(perPage)}
 * />
 * ```
 */
export const Pagination: React.FC<PaginationProps> = ({
  currentPage,
  totalPages,
  totalItems,
  perPage,
  onPageChange,
  onPerPageChange,
  perPageOptions = [10, 20, 50, 100],
  showPerPageSelector = true,
  showPageInfo = true,
  maxPageButtons = 7,
}) => {
  /**
   * Generate page numbers to display with ellipsis logic
   */
  const getPageNumbers = () => {
    if (totalPages <= maxPageButtons) {
      return Array.from({ length: totalPages }, (_, i) => i + 1)
    }

    const halfRange = Math.floor(maxPageButtons / 2)
    let startPage = Math.max(1, currentPage - halfRange)
    let endPage = Math.min(totalPages, currentPage + halfRange)

    // Adjust range if we're near the beginning or end
    if (currentPage <= halfRange) {
      endPage = Math.min(totalPages, maxPageButtons)
    } else if (currentPage + halfRange >= totalPages) {
      startPage = Math.max(1, totalPages - maxPageButtons + 1)
    }

    const pages: (number | 'ellipsis')[] = []

    // Add first page and ellipsis if needed
    if (startPage > 1) {
      pages.push(1)
      if (startPage > 2) {
        pages.push('ellipsis')
      }
    }

    // Add middle pages
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i)
    }

    // Add ellipsis and last page if needed
    if (endPage < totalPages) {
      if (endPage < totalPages - 1) {
        pages.push('ellipsis')
      }
      pages.push(totalPages)
    }

    return pages
  }

  const pageNumbers = getPageNumbers()
  const startItem = (currentPage - 1) * perPage + 1
  const endItem = Math.min(currentPage * perPage, totalItems)

  if (totalPages <= 1 && !showPerPageSelector) {
    return null
  }

  return (
    <div className="flex items-center justify-between px-2 py-4">
      {/* Page Info */}
      {showPageInfo && (
        <div className="flex-1 text-sm text-muted-foreground">
          {totalItems > 0 ? (
            <>
              Showing {startItem} to {endItem} of {totalItems} results
            </>
          ) : (
            'No results'
          )}
        </div>
      )}

      {/* Pagination Controls */}
      <div className="flex items-center space-x-6 lg:space-x-8">
        {/* Items per page selector */}
        {showPerPageSelector && onPerPageChange && (
          <div className="flex items-center space-x-2">
            <p className="text-sm font-medium">Rows per page</p>
            <select
              value={perPage}
              onChange={(e) => onPerPageChange(Number(e.target.value))}
              className="h-8 w-[70px] rounded border border-input bg-background px-3 py-1 text-sm ring-offset-background focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2"
            >
              {perPageOptions.map((option) => (
                <option key={option} value={option}>
                  {option}
                </option>
              ))}
            </select>
          </div>
        )}

        {/* Page Navigation */}
        {totalPages > 1 && (
          <>
            {/* Desktop Pagination */}
            <div className="hidden sm:flex items-center space-x-2">
              <Button
                variant="outline"
                size="icon"
                onClick={() => onPageChange(currentPage - 1)}
                disabled={currentPage <= 1}
                className="h-8 w-8"
              >
                <ChevronLeft className="h-4 w-4" />
                <span className="sr-only">Go to previous page</span>
              </Button>

              {pageNumbers.map((page, index) => {
                if (page === 'ellipsis') {
                  return (
                    <div
                      key={`ellipsis-${index}`}
                      className="flex h-8 w-8 items-center justify-center"
                    >
                      <MoreHorizontal className="h-4 w-4" />
                    </div>
                  )
                }

                return (
                  <Button
                    key={page}
                    variant={currentPage === page ? 'default' : 'outline'}
                    size="icon"
                    onClick={() => onPageChange(page)}
                    className={cn(
                      'h-8 w-8',
                      currentPage === page &&
                        'bg-primary text-primary-foreground hover:bg-primary/90'
                    )}
                  >
                    {page}
                  </Button>
                )
              })}

              <Button
                variant="outline"
                size="icon"
                onClick={() => onPageChange(currentPage + 1)}
                disabled={currentPage >= totalPages}
                className="h-8 w-8"
              >
                <ChevronRight className="h-4 w-4" />
                <span className="sr-only">Go to next page</span>
              </Button>
            </div>

            {/* Mobile Pagination */}
            <div className="flex sm:hidden items-center space-x-2">
              <Button
                variant="outline"
                size="sm"
                onClick={() => onPageChange(currentPage - 1)}
                disabled={currentPage <= 1}
              >
                <ChevronLeft className="h-4 w-4 mr-1" />
                Previous
              </Button>
              
              <span className="text-sm text-muted-foreground px-2">
                {currentPage} of {totalPages}
              </span>

              <Button
                variant="outline"
                size="sm"
                onClick={() => onPageChange(currentPage + 1)}
                disabled={currentPage >= totalPages}
              >
                Next
                <ChevronRight className="h-4 w-4 ml-1" />
              </Button>
            </div>
          </>
        )}
      </div>
    </div>
  )
}

export default Pagination