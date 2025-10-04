import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import { DashboardPage } from '~/pages/DashboardPage'

describe('DashboardPage', () => {
  it('should render dashboard title', () => {
    render(<DashboardPage />)
    expect(screen.getByText('Dashboard')).toBeInTheDocument()
  })

  it('should render welcome message', () => {
    render(<DashboardPage />)
    expect(screen.getByText('Welcome to Product Management')).toBeInTheDocument()
  })

  it('should render stat cards', () => {
    render(<DashboardPage />)
    expect(screen.getByText('Total Products')).toBeInTheDocument()
    expect(screen.getByText('Categories')).toBeInTheDocument()
    expect(screen.getByText('Total Value')).toBeInTheDocument()
    expect(screen.getByText('Growth')).toBeInTheDocument()
  })
})