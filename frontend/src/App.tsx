import React from 'react'
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { ThemeProvider } from '~/providers/ThemeProvider'
import { QueryProvider } from '~/providers/QueryProvider'
import { Layout } from '~/components/Layout'
import { ProductsPage } from '~/pages/ProductsPage'
import '~/utils/i18n'

const App: React.FC = () => {
  return (
    <QueryProvider>
      <ThemeProvider>
        <Router>
          <Layout>
            <Routes>
              <Route path="/" element={<ProductsPage />} />
              <Route path="/products" element={<ProductsPage />} />
            </Routes>
          </Layout>
        </Router>
      </ThemeProvider>
    </QueryProvider>
  )
}

export default App
