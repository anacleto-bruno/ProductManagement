import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { ProductsSearch } from './ProductsSearch'

// Mock i18next
vi.mock('react-i18next', () => ({
  useTranslation: () => ({
    t: (key: string) => {
      const translations: Record<string, string> = {
        'searchPlaceholder': 'Search products',
        'clear': 'Clear',
        'search': 'Search',
      }
      return translations[key] || key
    },
  }),
}))

describe('ProductsSearch', () => {
  const defaultProps = {
    search: '',
    onSearchChange: vi.fn(),
    onClearSearch: vi.fn(),
    isSearching: false,
    hasSearch: false,
    totalCount: 0,
  }

  beforeEach(() => {
    vi.clearAllMocks()
  })

  afterEach(() => {
    vi.clearAllMocks()
  })

  it('should render search input field', () => {
    render(<ProductsSearch {...defaultProps} />)
    
    const searchInput = screen.getByPlaceholderText(/search products/i)
    expect(searchInput).toBeInTheDocument()
  })

  it('should display search icon', () => {
    const { container } = render(<ProductsSearch {...defaultProps} />)
    
    // SearchIcon is rendered as an SVG
    const searchIcon = container.querySelector('[data-testid="SearchIcon"]')
    expect(searchIcon).toBeInTheDocument()
  })

  it('should call onSearchChange when typing in search field', async () => {
    const user = userEvent.setup()
    const onSearchChange = vi.fn()
    
    render(<ProductsSearch {...defaultProps} onSearchChange={onSearchChange} />)
    
    const searchInput = screen.getByPlaceholderText(/search products/i)
    await user.type(searchInput, 'test')
    
    expect(onSearchChange).toHaveBeenCalled()
    // Called for each character typed
    expect(onSearchChange).toHaveBeenCalledWith('t')
    expect(onSearchChange).toHaveBeenCalledWith('e')
    expect(onSearchChange).toHaveBeenCalledWith('s')
    expect(onSearchChange).toHaveBeenCalledWith('t')
  })

  it('should display current search value', () => {
    render(<ProductsSearch {...defaultProps} search="laptop" />)
    
    const searchInput = screen.getByDisplayValue('laptop')
    expect(searchInput).toBeInTheDocument()
  })

  it('should show Clear button when hasSearch is true', () => {
    render(<ProductsSearch {...defaultProps} hasSearch={true} search="test" />)
    
    const clearButton = screen.getAllByText(/clear/i)[0]
    expect(clearButton).toBeInTheDocument()
  })

  it('should not show Clear button when hasSearch is false', () => {
    render(<ProductsSearch {...defaultProps} hasSearch={false} />)
    
    const clearButtons = screen.queryAllByText(/clear/i)
    expect(clearButtons).toHaveLength(0)
  })

  it('should call onClearSearch when Clear button is clicked', async () => {
    const user = userEvent.setup()
    const onClearSearch = vi.fn()
    
    render(
      <ProductsSearch 
        {...defaultProps} 
        hasSearch={true} 
        search="test"
        onClearSearch={onClearSearch} 
      />
    )
    
    const clearButton = screen.getAllByText(/clear/i)[0]
    await user.click(clearButton)
    
    expect(onClearSearch).toHaveBeenCalledTimes(1)
  })

  it('should call onClearSearch when Escape key is pressed', async () => {
    const user = userEvent.setup()
    const onClearSearch = vi.fn()
    
    render(<ProductsSearch {...defaultProps} onClearSearch={onClearSearch} />)
    
    const searchInput = screen.getByPlaceholderText(/search products/i)
    searchInput.focus()
    await user.keyboard('{Escape}')
    
    expect(onClearSearch).toHaveBeenCalledTimes(1)
  })

  it('should show loading indicator when isSearching is true', () => {
    render(<ProductsSearch {...defaultProps} isSearching={true} />)
    
    const loadingIndicator = screen.getByRole('progressbar')
    expect(loadingIndicator).toBeInTheDocument()
  })

  it('should not show loading indicator when isSearching is false', () => {
    render(<ProductsSearch {...defaultProps} isSearching={false} />)
    
    const loadingIndicator = screen.queryByRole('progressbar')
    expect(loadingIndicator).not.toBeInTheDocument()
  })

  it('should display search chip when hasSearch is true', () => {
    render(<ProductsSearch {...defaultProps} hasSearch={true} search="laptop" />)
    
    const searchChip = screen.getByText(/"laptop"/i)
    expect(searchChip).toBeInTheDocument()
  })

  it('should display results count chip when hasSearch and not searching', () => {
    render(
      <ProductsSearch 
        {...defaultProps} 
        hasSearch={true} 
        search="laptop"
        isSearching={false}
        totalCount={5}
      />
    )
    
    const resultsChip = screen.getByText(/5 results/i)
    expect(resultsChip).toBeInTheDocument()
  })

  it('should display singular "result" when totalCount is 1', () => {
    render(
      <ProductsSearch 
        {...defaultProps} 
        hasSearch={true} 
        search="laptop"
        totalCount={1}
      />
    )
    
    const resultsChip = screen.getByText(/1 result$/i)
    expect(resultsChip).toBeInTheDocument()
  })

  it('should display plural "results" when totalCount is not 1', () => {
    render(
      <ProductsSearch 
        {...defaultProps} 
        hasSearch={true} 
        search="laptop"
        totalCount={3}
      />
    )
    
    const resultsChip = screen.getByText(/3 results/i)
    expect(resultsChip).toBeInTheDocument()
  })

  it('should not display results count chip when searching', () => {
    render(
      <ProductsSearch 
        {...defaultProps} 
        hasSearch={true} 
        search="laptop"
        isSearching={true}
        totalCount={5}
      />
    )
    
    const resultsChip = screen.queryByText(/5 results/i)
    expect(resultsChip).not.toBeInTheDocument()
  })

  it('should show Ctrl+K hint in placeholder', () => {
    render(<ProductsSearch {...defaultProps} />)
    
    const searchInput = screen.getByPlaceholderText(/ctrl\+k/i)
    expect(searchInput).toBeInTheDocument()
  })

  it('should focus search input when Ctrl+K is pressed', async () => {
    const user = userEvent.setup()
    
    render(<ProductsSearch {...defaultProps} />)
    
    const searchInput = screen.getByPlaceholderText(/search products/i)
    expect(searchInput).not.toHaveFocus()
    
    // Simulate Ctrl+K
    await user.keyboard('{Control>}k{/Control}')
    
    expect(searchInput).toHaveFocus()
  })

  it('should focus search input when Cmd+K is pressed (Mac)', async () => {
    const user = userEvent.setup()
    
    render(<ProductsSearch {...defaultProps} />)
    
    const searchInput = screen.getByPlaceholderText(/search products/i)
    expect(searchInput).not.toHaveFocus()
    
    // Simulate Cmd+K (Meta key for Mac)
    await user.keyboard('{Meta>}k{/Meta}')
    
    expect(searchInput).toHaveFocus()
  })

  it('should call onClearSearch from search chip delete', async () => {
    const user = userEvent.setup()
    const onClearSearch = vi.fn()
    
    render(
      <ProductsSearch 
        {...defaultProps} 
        hasSearch={true} 
        search="laptop"
        onClearSearch={onClearSearch}
      />
    )
    
    // Find the chip and click its delete button
    const deleteButton = screen.getByTestId('CancelIcon')
    await user.click(deleteButton)
    
    expect(onClearSearch).toHaveBeenCalledTimes(1)
  })
})
