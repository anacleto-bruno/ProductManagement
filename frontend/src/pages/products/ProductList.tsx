import { useTranslation } from 'react-i18next'
import { useProducts } from '~/hooks/use-products'
import { useProductStore } from '~/states/product-store'
import { Button } from '~/components/ui/button'
import { Pagination } from '~/components/ui/pagination'
import { ErrorBoundary } from '~/components/ui/error-boundary'
import { ProductTableSkeleton } from '~/components/ui/product-table-skeleton'
import { ResponsiveProductTable } from '~/components/ui/responsive-product-table'
import { AlertCircle } from 'lucide-react'
import type { Product } from '~/api/product-api'

export default function ProductList() {
  const { t } = useTranslation(['product', 'common'])
  
  // Use separate selectors to avoid creating new objects
  const pagination = useProductStore(state => state.pagination)
  const goToPage = useProductStore(state => state.goToPage)
  const setPerPage = useProductStore(state => state.setPerPage)
  
  const { data, isLoading, error } = useProducts({ 
    page: pagination.page, 
    perPage: pagination.perPage 
  })

  if (isLoading) {
    return (
      <div className="container mx-auto py-8">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold">{t('common:navigation.products')}</h1>
          <Button disabled>{t('common:buttons.add')}</Button>
        </div>
        <ProductTableSkeleton rows={pagination.perPage} />
      </div>
    )
  }

  if (error || !data) {
    return (
      <div className="container mx-auto py-8">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold">{t('common:navigation.products')}</h1>
          <Button>{t('common:buttons.add')}</Button>
        </div>
        <div className="flex flex-col items-center justify-center py-12 text-center">
          <AlertCircle className="h-12 w-12 text-destructive mb-4" />
          <h3 className="text-lg font-semibold mb-2">Failed to load products</h3>
          <p className="text-muted-foreground mb-4">
            {error?.message || 'An error occurred while fetching products.'}
          </p>
          <Button onClick={() => window.location.reload()} variant="outline">
            Try Again
          </Button>
        </div>
      </div>
    )
  }

  return (
    <ErrorBoundary>
      <div className="container mx-auto py-8">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold">{t('common:navigation.products')}</h1>
          <Button>{t('common:buttons.add')}</Button>
        </div>
        
        {data.data.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-12 text-center">
            <div className="text-muted-foreground mb-4">
              <h3 className="text-lg font-semibold mb-2">No products found</h3>
              <p>Get started by adding your first product.</p>
            </div>
            <Button>{t('common:buttons.add')} Product</Button>
          </div>
        ) : (
          <>
            <ResponsiveProductTable
              products={data.data}
              onEdit={(product: Product) => {
                // TODO: Implement edit functionality
                console.log('Edit product:', product.id)
              }}
              onDelete={(product: Product) => {
                // TODO: Implement delete functionality
                console.log('Delete product:', product.id)
              }}
            />
            
            {/* Pagination */}
            <Pagination
              currentPage={pagination.page}
              totalPages={data.pagination.totalPages}
              totalItems={data.pagination.totalCount}
              perPage={pagination.perPage}
              onPageChange={goToPage}
              onPerPageChange={setPerPage}
            />
          </>
        )}
      </div>
    </ErrorBoundary>
  )
}