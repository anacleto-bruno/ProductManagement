import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { Header } from './Header'

// Mock the app store
const mockToggleSidebar = vi.fn()
vi.mock('~/states/appStore', () => ({
  useAppStore: () => ({
    toggleSidebar: mockToggleSidebar,
  }),
}))

// Mock the theme context
const mockToggleTheme = vi.fn()
let mockThemeMode = 'light'

vi.mock('~/providers/ThemeContext', () => ({
  useTheme: () => ({
    mode: mockThemeMode,
    toggleTheme: mockToggleTheme,
  }),
}))

describe('Header', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockThemeMode = 'light'
  })

  it('should render with default title', () => {
    render(<Header />)
    
    expect(screen.getByText('Product Management')).toBeInTheDocument()
  })

  it('should render with custom title', () => {
    render(<Header title="My Custom Title" />)
    
    expect(screen.getByText('My Custom Title')).toBeInTheDocument()
    expect(screen.queryByText('Product Management')).not.toBeInTheDocument()
  })

  it('should render menu button', () => {
    render(<Header />)
    
    const menuButton = screen.getByRole('button', { name: /menu/i })
    expect(menuButton).toBeInTheDocument()
  })

  it('should render theme toggle button', () => {
    render(<Header />)
    
    const themeButton = screen.getByRole('button', { name: /toggle theme/i })
    expect(themeButton).toBeInTheDocument()
  })

  it('should call toggleSidebar when menu button is clicked', async () => {
    const user = userEvent.setup()
    
    render(<Header />)
    
    const menuButton = screen.getByRole('button', { name: /menu/i })
    await user.click(menuButton)
    
    expect(mockToggleSidebar).toHaveBeenCalledTimes(1)
  })

  it('should call toggleTheme when theme button is clicked', async () => {
    const user = userEvent.setup()
    
    render(<Header />)
    
    const themeButton = screen.getByRole('button', { name: /toggle theme/i })
    await user.click(themeButton)
    
    expect(mockToggleTheme).toHaveBeenCalledTimes(1)
  })

  describe('theme icon', () => {
    it('should show dark mode icon (Brightness4) in light mode', () => {
      mockThemeMode = 'light'
      const { container } = render(<Header />)
      
      // Brightness4 is shown when mode is light
      const darkModeIcon = container.querySelector('[data-testid="Brightness4Icon"]')
      expect(darkModeIcon).toBeInTheDocument()
    })

    it('should show light mode icon (Brightness7) in dark mode', () => {
      mockThemeMode = 'dark'
      const { container } = render(<Header />)
      
      // Brightness7 is shown when mode is dark
      const lightModeIcon = container.querySelector('[data-testid="Brightness7Icon"]')
      expect(lightModeIcon).toBeInTheDocument()
    })

    it('should not show Brightness7 icon in light mode', () => {
      mockThemeMode = 'light'
      const { container } = render(<Header />)
      
      const lightModeIcon = container.querySelector('[data-testid="Brightness7Icon"]')
      expect(lightModeIcon).not.toBeInTheDocument()
    })

    it('should not show Brightness4 icon in dark mode', () => {
      mockThemeMode = 'dark'
      const { container } = render(<Header />)
      
      const darkModeIcon = container.querySelector('[data-testid="Brightness4Icon"]')
      expect(darkModeIcon).not.toBeInTheDocument()
    })
  })

  describe('AppBar structure', () => {
    it('should render AppBar with fixed position', () => {
      const { container } = render(<Header />)
      
      const appBar = container.querySelector('.MuiAppBar-root')
      expect(appBar).toBeInTheDocument()
      expect(appBar).toHaveClass('MuiAppBar-positionFixed')
    })

    it('should render Toolbar', () => {
      const { container } = render(<Header />)
      
      const toolbar = container.querySelector('.MuiToolbar-root')
      expect(toolbar).toBeInTheDocument()
    })
  })

  describe('accessibility', () => {
    it('should have proper aria-label for menu button', () => {
      render(<Header />)
      
      const menuButton = screen.getByLabelText('menu')
      expect(menuButton).toBeInTheDocument()
    })

    it('should have proper aria-label for theme toggle button', () => {
      render(<Header />)
      
      const themeButton = screen.getByLabelText('toggle theme')
      expect(themeButton).toBeInTheDocument()
    })

    it('should have h6 typography for title', () => {
      render(<Header title="Test Title" />)
      
      const titleElement = screen.getByText('Test Title')
      // Typography component="div" renders a div with variant="h6" styling
      expect(titleElement).toBeInTheDocument()
      expect(titleElement).toHaveClass('MuiTypography-h6')
    })
  })

  describe('multiple clicks', () => {
    it('should handle multiple menu button clicks', async () => {
      const user = userEvent.setup()
      
      render(<Header />)
      
      const menuButton = screen.getByRole('button', { name: /menu/i })
      await user.click(menuButton)
      await user.click(menuButton)
      await user.click(menuButton)
      
      expect(mockToggleSidebar).toHaveBeenCalledTimes(3)
    })

    it('should handle multiple theme toggle clicks', async () => {
      const user = userEvent.setup()
      
      render(<Header />)
      
      const themeButton = screen.getByRole('button', { name: /toggle theme/i })
      await user.click(themeButton)
      await user.click(themeButton)
      
      expect(mockToggleTheme).toHaveBeenCalledTimes(2)
    })
  })
})
