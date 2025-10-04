import React from 'react'
import { 
  AppBar, 
  Toolbar, 
  Typography, 
  IconButton, 
  Box 
} from '@mui/material'
import { 
  Menu as MenuIcon, 
  Brightness4, 
  Brightness7 
} from '@mui/icons-material'
import { useAppStore } from '~/states/appStore'
import { useTheme } from '~/providers/ThemeContext'

interface HeaderProps {
  title?: string
}

export const Header: React.FC<HeaderProps> = ({ 
  title = 'Product Management' 
}) => {
  const { toggleSidebar } = useAppStore()
  const { mode, toggleTheme } = useTheme()

  return (
    <AppBar position="fixed">
      <Toolbar>
        <IconButton
          edge="start"
          color="inherit"
          aria-label="menu"
          onClick={toggleSidebar}
          sx={{ mr: 2 }}
        >
          <MenuIcon />
        </IconButton>
        
        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
          {title}
        </Typography>
        
        <Box>
          <IconButton
            color="inherit"
            onClick={toggleTheme}
            aria-label="toggle theme"
          >
            {mode === 'dark' ? <Brightness7 /> : <Brightness4 />}
          </IconButton>
        </Box>
      </Toolbar>
    </AppBar>
  )
}