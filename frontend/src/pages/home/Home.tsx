import { useTranslation } from 'react-i18next'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'

export default function Home() {
  const { t } = useTranslation(['common'])

  return (
    <div className="container mx-auto py-8">
      <h1 className="text-4xl font-bold mb-8">{t('app.title')}</h1>
      
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <Card>
          <CardHeader>
            <CardTitle>{t('navigation.products')}</CardTitle>
            <CardDescription>Manage your product catalog</CardDescription>
          </CardHeader>
          <CardContent>
            <p>View, create, edit, and delete products in your inventory.</p>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader>
            <CardTitle>{t('navigation.search')}</CardTitle>
            <CardDescription>Find products quickly</CardDescription>
          </CardHeader>
          <CardContent>
            <p>Search for products by name, description, brand, and category.</p>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader>
            <CardTitle>{t('navigation.settings')}</CardTitle>
            <CardDescription>Configure application settings</CardDescription>
          </CardHeader>
          <CardContent>
            <p>Customize your application preferences and appearance.</p>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}