import i18n from 'i18next'
import { initReactI18next } from 'react-i18next'

// Translation resources
const resources = {
  en: {
    common: {
      loading: 'Loading...',
      error: 'Error',
      success: 'Success',
      cancel: 'Cancel',
      confirm: 'Confirm',
      save: 'Save',
      edit: 'Edit',
      delete: 'Delete',
      search: 'Search',
      clear: 'Clear',
      add: 'Add',
      create: 'Create',
      update: 'Update',
      actions: 'Actions',
      noData: 'No data available',
      page: 'Page',
      of: 'of',
      rows: 'rows',
    },
    products: {
      title: 'Products',
      name: 'Name',
      description: 'Description',
      model: 'Model',
      brand: 'Brand',
      sku: 'SKU',
      price: 'Price',
      colors: 'Colors',
      sizes: 'Sizes',
      addProduct: 'Add Product',
      editProduct: 'Edit Product',
      deleteProduct: 'Delete Product',
      deleteConfirm: 'Are you sure you want to delete this product?',
      searchPlaceholder: 'Search products...',
      seedData: 'Seed Data',
      seedSuccess: 'Successfully seeded {{count}} products',
      createSuccess: 'Product created successfully',
      updateSuccess: 'Product updated successfully',
      deleteSuccess: 'Product deleted successfully',
      table: {
        noData: 'No products found',
      },
    },
    validation: {
      required: 'This field is required',
      minLength: 'Minimum length is {{min}} characters',
      maxLength: 'Maximum length is {{max}} characters',
      invalidEmail: 'Invalid email address',
      invalidNumber: 'Must be a valid number',
      minValue: 'Minimum value is {{min}}',
      maxValue: 'Maximum value is {{max}}',
    },
  },
}

i18n
  .use(initReactI18next)
  .init({
    resources,
    lng: 'en', // default language
    fallbackLng: 'en',
    
    interpolation: {
      escapeValue: false, // not needed for react as it escapes by default
    },

    // namespace configuration
    ns: ['common', 'products', 'validation'],
    defaultNS: 'common',
  })

export default i18n