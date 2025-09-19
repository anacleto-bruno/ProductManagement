import { useTranslation } from 'react-i18next'
import { Link } from '@tanstack/react-router'
import { useProducts } from '~/hooks/use-products'
import { useProductStore } from '~/states/product-store'
import { 
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow 
} from '~/components/ui/table'
import { Button } from '~/components/ui/button'
import { Skeleton } from '~/components/ui/skeleton'
import { Pencil, Trash2, Eye } from 'lucide-react'

export default function ProductList() {
  const { t } = useTranslation(['product', 'common'])
  const { page, perPage } = useProductStore(state => state.pagination)
  const { data, isLoading, error } = useProducts({ page, perPage })

  if (isLoading) {
    return (
      <div className="container mx-auto py-8">
        <h1 className="text-3xl font-bold mb-6">{t('common:navigation.products')}</h1>
        <div className="space-y-2">
          {Array(5).fill(0).map((_, i) => (
            <Skeleton key={i} className="h-12 w-full" />
          ))}
        </div>
      </div>
    )
  }

  if (error || !data) {
    return (
      <div className="container mx-auto py-8">
        <h1 className="text-3xl font-bold mb-6">{t('common:navigation.products')}</h1>
        <p>{t('common:error')}</p>
      </div>
    )
  }

  return (
    <div className="container mx-auto py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold">{t('common:navigation.products')}</h1>
        <Button>{t('common:buttons.add')}</Button>
      </div>
      
      {data.items.length === 0 ? (
        <p>{t('messages.noProductsFound')}</p>
      ) : (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>{t('fields.name')}</TableHead>
              <TableHead>{t('fields.brand')}</TableHead>
              <TableHead>{t('fields.category')}</TableHead>
              <TableHead>{t('fields.price')}</TableHead>
              <TableHead className="text-right">{t('common:buttons.actions')}</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {data.items.map(product => (
              <TableRow key={product.id}>
                <TableCell>{product.name}</TableCell>
                <TableCell>{product.brand}</TableCell>
                <TableCell>{product.category}</TableCell>
                <TableCell>${product.price.toFixed(2)}</TableCell>
                <TableCell className="text-right space-x-2">
                  <Link to="/products/$productId" params={{ productId: product.id }}>
                    <Button variant="ghost" size="icon">
                      <Eye className="h-4 w-4" />
                    </Button>
                  </Link>
                  <Button variant="ghost" size="icon">
                    <Pencil className="h-4 w-4" />
                  </Button>
                  <Button variant="ghost" size="icon">
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}
      
      {/* Pagination would go here */}
    </div>
  )
}