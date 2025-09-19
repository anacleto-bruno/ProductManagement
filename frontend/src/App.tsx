import { Suspense } from 'react'
import { Outlet, Link } from '@tanstack/react-router'
import { ThemeProvider } from './components/providers/theme-provider'
import { ModeToggle } from './components/ui/mode-toggle'
import { LanguageSelector } from './components/ui/language-selector'
import { Button } from './components/ui/button'
import { useTranslation } from 'react-i18next'

function App() {
  const { t } = useTranslation(['common'])

  return (
    <ThemeProvider defaultTheme="system" storageKey="product-management-theme">
      <div className="min-h-screen">
        {/* Header */}
        <header className="border-b">
          <div className="container mx-auto py-4 flex items-center justify-between">
            <div className="flex items-center gap-2">
              <Link to="/" className="text-xl font-bold">
                {t('app.title')}
              </Link>
            </div>

            <nav className="hidden md:flex items-center gap-6">
              <Link to="/" className="text-sm font-medium">
                {t('navigation.home')}
              </Link>
              <Link to="/products" className="text-sm font-medium">
                {t('navigation.products')}
              </Link>
              <Link to="/search" className="text-sm font-medium">
                {t('navigation.search')}
              </Link>
            </nav>

            <div className="flex items-center gap-2">
              <LanguageSelector />
              <ModeToggle />
            </div>
          </div>
        </header>

        {/* Mobile Navigation */}
        <div className="md:hidden border-b">
          <div className="container mx-auto py-2 flex justify-between">
            <Link to="/" className="text-sm font-medium">
              <Button variant="ghost" size="sm">
                {t('navigation.home')}
              </Button>
            </Link>
            <Link to="/products" className="text-sm font-medium">
              <Button variant="ghost" size="sm">
                {t('navigation.products')}
              </Button>
            </Link>
            <Link to="/search" className="text-sm font-medium">
              <Button variant="ghost" size="sm">
                {t('navigation.search')}
              </Button>
            </Link>
          </div>
        </div>

        {/* Main Content */}
        <main>
          <Suspense fallback={<div className="container mx-auto py-8">{t('app.loading')}</div>}>
            <Outlet />
          </Suspense>
        </main>

        {/* Footer */}
        <footer className="border-t mt-auto">
          <div className="container mx-auto py-4 text-center text-sm text-muted-foreground">
            &copy; {new Date().getFullYear()} Product Management App
          </div>
        </footer>
      </div>
    </ThemeProvider>
  )
}

export default App
