import { useTranslation } from 'react-i18next'
import { useProductSearch } from '~/hooks/use-products'
import { useProductStore } from '~/states/product-store'
import { useState } from 'react'
import { Input } from '~/components/ui/input'
import { Button } from '~/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '~/components/ui/card'
import { Skeleton } from '~/components/ui/skeleton'

export default function SearchPage() {
  const { t } = useTranslation(['product', 'common'])
  const [searchQuery, setSearchQuery] = useState('')
  const [activeSearch, setActiveSearch] = useState('')
  
  const filters = useProductStore(state => state.filters)
  const { data, isLoading, error } = useProductSearch({ 
    searchTerm: activeSearch,
    category: filters.category || undefined,
    brand: filters.brand || undefined,
    minPrice: filters.minPrice || undefined,
    maxPrice: filters.maxPrice || undefined
  })

  const handleSearch = () => {
    setActiveSearch(searchQuery)
  }

  return (
    <div className="container mx-auto py-8">
      <h1 className="text-3xl font-bold mb-6">{t('actions.searchProducts')}</h1>
      
      <div className="flex gap-2 mb-6">
        <Input
          placeholder={t('common:buttons.search')}
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="flex-1"
        />
        <Button onClick={handleSearch}>{t('common:buttons.search')}</Button>
      </div>
      
      {isLoading ? (
        <div className="space-y-4">
          {Array(3).fill(0).map((_, i) => (
            <Skeleton key={i} className="h-32 w-full" />
          ))}
        </div>
      ) : error ? (
        <div>
          <p>{t('common:error')}</p>
        </div>
      ) : data?.items.length === 0 ? (
        <div>
          <p>{t('messages.noProductsFound')}</p>
        </div>
      ) : data ? (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {data.items.map(product => (
            <Card key={product.id}>
              <CardHeader>
                <CardTitle>{product.name}</CardTitle>
              </CardHeader>
              <CardContent>
                <p className="truncate mb-2">{product.description}</p>
                <div className="flex justify-between">
                  <span>{product.brand}</span>
                  <span className="font-bold">${product.price.toFixed(2)}</span>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      ) : null}
    </div>
  )
}