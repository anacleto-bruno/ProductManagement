import React from 'react'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { ThemeProvider } from '~/providers/ThemeProvider'
import { QueryProvider } from '~/providers/QueryProvider'
import { Layout } from '~/components/Layout'
import { DashboardPage } from '~/pages/DashboardPage'
import { ProductsPage } from '~/pages/ProductsPage'
import '~/utils/i18n'

const App: React.FC = () => {
  return (
    <QueryProvider>
      <ThemeProvider>
        <Router>
          <Layout>
            <Routes>
              <Route path="/" element={<DashboardPage />} />
              <Route path="/products" element={<ProductsPage />} />
              <Route path="/settings" element={<div>Settings Page (Coming Soon)</div>} />
            </Routes>
          </Layout>
        </Router>
      </ThemeProvider>
    </QueryProvider>
  )
}

export default App
