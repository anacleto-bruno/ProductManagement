import React from 'react'
import { 
  Drawer, 
  List, 
  ListItem, 
  ListItemButton, 
  ListItemIcon, 
  ListItemText, 
  Divider, 
  Box 
} from '@mui/material'
import { 
  Inventory
} from '@mui/icons-material'
import { useAppStore } from '~/states/appStore'
import { useNavigate, useLocation } from 'react-router-dom'

const drawerWidth = 240

interface SidebarProps {
  variant?: 'permanent' | 'temporary'
}

export const Sidebar: React.FC<SidebarProps> = ({ 
  variant = 'temporary' 
}) => {
  const { sidebarOpen, setSidebarOpen } = useAppStore()
  const navigate = useNavigate()
  const location = useLocation()

  const menuItems = [
    { text: 'Products', icon: Inventory, path: '/' },
  ]

  const handleItemClick = (path: string) => {
    navigate(path)
    if (variant === 'temporary') {
      setSidebarOpen(false)
    }
  }

  const handleClose = () => {
    setSidebarOpen(false)
  }

  const drawerContent = (
    <Box sx={{ width: drawerWidth }}>
      <Box sx={{ p: 2 }}>
        <img 
          src="/logo.svg" 
          alt="Logo" 
          style={{ width: '100%', maxWidth: 120 }}
        />
      </Box>
      <Divider />
      <List>
        {menuItems.map((item) => (
          <ListItem key={item.text} disablePadding>
            <ListItemButton
              selected={location.pathname === item.path || (item.path === '/' && location.pathname === '/products')}
              onClick={() => handleItemClick(item.path)}
            >
              <ListItemIcon>
                <item.icon />
              </ListItemIcon>
              <ListItemText primary={item.text} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </Box>
  )

  return (
    <Drawer
      variant={variant}
      open={sidebarOpen}
      onClose={handleClose}
      ModalProps={{
        keepMounted: true, // Better open performance on mobile
      }}
      sx={{
        width: drawerWidth,
        flexShrink: 0,
        '& .MuiDrawer-paper': {
          width: drawerWidth,
          boxSizing: 'border-box',
        },
      }}
    >
      {drawerContent}
    </Drawer>
  )
}