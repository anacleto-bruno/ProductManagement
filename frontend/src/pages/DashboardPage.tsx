import React from 'react'
import { 
  Typography, 
  Paper, 
  Box, 
  Card,
  CardContent 
} from '@mui/material'
import { 
  Inventory, 
  TrendingUp, 
  AttachMoney,
  Category 
} from '@mui/icons-material'

interface StatCardProps {
  title: string
  value: string | number
  icon: React.ElementType
  color: string
}

const StatCard: React.FC<StatCardProps> = ({ title, value, icon: Icon, color }) => (
  <Card>
    <CardContent>
      <Box display="flex" alignItems="center" justifyContent="space-between">
        <Box>
          <Typography color="textSecondary" gutterBottom variant="body2">
            {title}
          </Typography>
          <Typography variant="h4" component="h2">
            {value}
          </Typography>
        </Box>
        <Icon sx={{ fontSize: 40, color }} />
      </Box>
    </CardContent>
  </Card>
)

export const DashboardPage: React.FC = () => {
  const stats = [
    {
      title: 'Total Products',
      value: 1247,
      icon: Inventory,
      color: '#1976d2',
    },
    {
      title: 'Categories',
      value: 24,
      icon: Category,
      color: '#388e3c',
    },
    {
      title: 'Total Value',
      value: '$45,678',
      icon: AttachMoney,
      color: '#f57c00',
    },
    {
      title: 'Growth',
      value: '+12.5%',
      icon: TrendingUp,
      color: '#7b1fa2',
    },
  ]

  return (
    <Box>
      <Typography variant="h4" component="h1" gutterBottom>
        Dashboard
      </Typography>
      
      <Box 
        sx={{ 
          display: 'grid', 
          gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))',
          gap: 3,
          mb: 3 
        }}
      >
        {stats.map((stat) => (
          <StatCard key={stat.title} {...stat} />
        ))}
      </Box>

      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" gutterBottom>
          Welcome to Product Management
        </Typography>
        <Typography variant="body1" color="textSecondary">
          This is your product management dashboard. You can view statistics, 
          manage products, and configure settings from here.
        </Typography>
      </Paper>
    </Box>
  )
}