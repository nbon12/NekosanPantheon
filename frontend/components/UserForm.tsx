import React, { useState } from 'react';
import { CreateUserRequest } from '../services/api';

interface UserFormProps {
  onSubmit: (user: CreateUserRequest) => Promise<void>;
  isLoading?: boolean;
}

export const UserForm: React.FC<UserFormProps> = ({ onSubmit, isLoading }) => {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!name.trim() || !email.trim()) {
      setError('Name and email are required');
      return;
    }

    try {
      await onSubmit({ name: name.trim(), email: email.trim() });
      setName('');
      setEmail('');
      setError(null);
    } catch (err: any) {
      // Error is already handled by parent component, but show a local message too
      const errorMsg = err?.response?.data || err?.message || 'Failed to create user. Please try again.';
      setError(errorMsg);
    }
  };

  return (
    <form onSubmit={handleSubmit} data-testid="user-form">
      {error && <div data-testid="form-error">{error}</div>}
      
      <div>
        <label htmlFor="name">Name:</label>
        <input
          id="name"
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          disabled={isLoading}
          data-testid="name-input"
        />
      </div>

      <div>
        <label htmlFor="email">Email:</label>
        <input
          id="email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          disabled={isLoading}
          data-testid="email-input"
        />
      </div>

      <button type="submit" disabled={isLoading} data-testid="submit-button">
        {isLoading ? 'Adding...' : 'Add User'}
      </button>
    </form>
  );
};

