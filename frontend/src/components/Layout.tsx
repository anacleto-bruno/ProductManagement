import React from 'react'
import { Box, Container, Toolbar } from '@mui/material'
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
        }}
      >
        <Toolbar /> {/* This creates space for the fixed AppBar */}
        <Container maxWidth="xl" sx={{ mt: 2, mb: 2 }}>
          {children}
        </Container>
      </Box>
      
      <NotificationProvider />
    </Box>
  )
}