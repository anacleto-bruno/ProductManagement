import { useTranslation } from 'react-i18next'
import { useParams } from '@tanstack/react-router'
import { useProduct } from '~/hooks/use-products'
import { Card, CardContent, CardHeader, CardTitle } from '~/components/ui/card'
import { Skeleton } from '~/components/ui/skeleton'

export default function ProductDetail() {
  const { productId } = useParams({ from: '/products/$productId' })
  const { t } = useTranslation(['product', 'common'])
  const { data: product, isLoading, error } = useProduct(productId)

  if (isLoading) {
    return (
      <div className="container mx-auto py-8">
        <Skeleton className="h-12 w-3/4 mb-6" />
        <Card>
          <CardHeader>
            <Skeleton className="h-8 w-2/3" />
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <Skeleton className="h-4 w-full" />
              <Skeleton className="h-4 w-3/4" />
              <Skeleton className="h-4 w-1/2" />
            </div>
          </CardContent>
        </Card>
      </div>
    )
  }

  if (error || !product) {
    return (
      <div className="container mx-auto py-8">
        <h1 className="text-3xl font-bold mb-6">{t('common:error')}</h1>
        <p>Failed to load product details.</p>
      </div>
    )
  }

  return (
    <div className="container mx-auto py-8">
      <h1 className="text-3xl font-bold mb-6">{product.name}</h1>
      
      <Card>
        <CardHeader>
          <CardTitle>{t('actions.viewProduct')}</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <h3 className="text-lg font-medium">{t('fields.description')}</h3>
              <p className="mt-1">{product.description}</p>
            </div>
            
            <div className="space-y-4">
              <div>
                <h3 className="text-lg font-medium">{t('fields.brand')}</h3>
                <p className="mt-1">{product.brand}</p>
              </div>
              
              <div>
                <h3 className="text-lg font-medium">{t('fields.price')}</h3>
                <p className="mt-1">${product.price.toFixed(2)}</p>
              </div>
              
              <div>
                <h3 className="text-lg font-medium">{t('fields.sku')}</h3>
                <p className="mt-1">{product.sku}</p>
              </div>
              
              <div>
                <h3 className="text-lg font-medium">{t('fields.category')}</h3>
                <p className="mt-1">{product.category}</p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}