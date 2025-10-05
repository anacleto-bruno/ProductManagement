import React from 'react'
import { Box, Toolbar } from '@mui/material'
import { Header } from './Header'
import { Sidebar } from './Sidebar'
import { NotificationProvider } from './NotificationProvider'

interface LayoutProps {
  children: React.ReactNode
  title?: string
}

export const Layout: React.FC<LayoutProps> = ({ children, title }) => {
  return (
    <Box sx={{ display: 'flex' }}>
      <Header title={title} />
      <Sidebar />
      
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          bgcolor: 'background.default',
          minHeight: '100vh',
          minWidth: 0, // Prevents flex shrinking
          width: '100vw', // Ensures full width
        }}
      >
        <Toolbar /> {/* This creates space for the fixed AppBar */}
        <Box sx={{ 
          px: 3, 
          py: 2, 
          width: '100%',
          minWidth: '100%' // Ensures content container maintains full width
        }}>
          {children}
        </Box>
      </Box>
      
      <NotificationProvider />
    </Box>
  )
}