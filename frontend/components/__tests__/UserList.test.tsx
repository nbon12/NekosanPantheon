import React from 'react';
import { render, screen } from '@testing-library/react';
import { UserList } from '../UserList';
import { User } from '../../services/api';

describe('UserList', () => {
  const mockUsers: User[] = [
    { id: '1', name: 'John Doe', email: 'john@example.com' },
    { id: '2', name: 'Jane Smith', email: 'jane@example.com' },
  ];

  it('renders empty list when no users exist', () => {
    render(<UserList users={[]} />);
    expect(screen.getByTestId('empty')).toBeInTheDocument();
    expect(screen.getByText('No users found')).toBeInTheDocument();
  });

  it('renders loading state', () => {
    render(<UserList users={[]} isLoading={true} />);
    expect(screen.getByTestId('loading')).toBeInTheDocument();
    expect(screen.getByText('Loading users...')).toBeInTheDocument();
  });

  it('renders error message', () => {
    render(<UserList users={[]} error="Failed to load users" />);
    expect(screen.getByTestId('error')).toBeInTheDocument();
    expect(screen.getByText('Error: Failed to load users')).toBeInTheDocument();
  });

  it('renders list of users', () => {
    render(<UserList users={mockUsers} />);
    expect(screen.getByTestId('user-list')).toBeInTheDocument();
    expect(screen.getByTestId('user-1')).toBeInTheDocument();
    expect(screen.getByTestId('user-2')).toBeInTheDocument();
    expect(screen.getByText('John Doe')).toBeInTheDocument();
    expect(screen.getByText('Jane Smith')).toBeInTheDocument();
    expect(screen.getByText('john@example.com')).toBeInTheDocument();
    expect(screen.getByText('jane@example.com')).toBeInTheDocument();
  });
});

