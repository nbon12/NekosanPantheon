import React from 'react';
import { User } from '../services/api';

interface UserListProps {
  users: User[];
  isLoading?: boolean;
  error?: string | null;
}

export const UserList: React.FC<UserListProps> = ({ users, isLoading, error }) => {
  if (isLoading) {
    return <div data-testid="loading">Loading users...</div>;
  }

  if (error) {
    return <div data-testid="error">Error: {error}</div>;
  }

  if (users.length === 0) {
    return <div data-testid="empty">No users found</div>;
  }

  return (
    <ul data-testid="user-list">
      {users.map((user) => (
        <li key={user.id} data-testid={`user-${user.id}`}>
          <strong>{user.name}</strong> - {user.email}
        </li>
      ))}
    </ul>
  );
};

