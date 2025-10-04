import React from 'react'
import { 
  Typography, 
  Paper, 
  Box,
  Button,
  TextField,
  InputAdornment
} from '@mui/material'
import { 
  Add as AddIcon,
  Search as SearchIcon
} from '@mui/icons-material'

export const ProductsPage: React.FC = () => {
  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4" component="h1">
          Products
        </Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => console.log('Add product')}
        >
          Add Product
        </Button>
      </Box>

      <Paper sx={{ p: 3, mb: 3 }}>
        <TextField
          fullWidth
          placeholder="Search products..."
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon />
              </InputAdornment>
            ),
          }}
          sx={{ mb: 2 }}
        />
        
        <Typography variant="body1" color="textSecondary" align="center">
          Product table will be implemented in Epic 6
        </Typography>
      </Paper>
    </Box>
  )
}