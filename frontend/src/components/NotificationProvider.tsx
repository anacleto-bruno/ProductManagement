import React from 'react'
import { 
  Snackbar, 
  Alert, 
  Box 
} from '@mui/material'
import { useAppStore } from '~/states/appStore'

export const NotificationProvider: React.FC = () => {
  const { notifications, removeNotification } = useAppStore()

  return (
    <Box>
      {notifications.map((notification) => (
        <Snackbar
          key={notification.id}
          open={true}
          autoHideDuration={notification.duration || 5000}
          onClose={() => removeNotification(notification.id)}
          anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
          sx={{ mt: 8 }} // Account for the AppBar
        >
          <Alert
            onClose={() => removeNotification(notification.id)}
            severity={notification.type}
            variant="filled"
            sx={{ width: '100%' }}
          >
            {notification.message}
          </Alert>
        </Snackbar>
      ))}
    </Box>
  )
}