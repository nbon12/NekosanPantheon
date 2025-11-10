import React, { useState, useEffect } from 'react';
import { UserList } from '../components/UserList';
import { UserForm } from '../components/UserForm';
import { api, User, CreateUserRequest } from '../services/api';

export default function Home() {
  const [users, setUsers] = useState<User[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchUsers = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const fetchedUsers = await api.getUsers();
      setUsers(fetchedUsers);
    } catch (err) {
      setError('Failed to fetch users. Please try again later.');
      console.error('Error fetching users:', err);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  const handleAddUser = async (userData: CreateUserRequest) => {
    try {
      setIsSubmitting(true);
      setError(null);
      const newUser = await api.createUser(userData);
      setUsers([...users, newUser]);
      // Refresh the list to ensure we have the latest data
      await fetchUsers();
    } catch (err: any) {
      console.error('Error in handleAddUser:', err);
      let errorMessage = 'Failed to create user. Please try again.';
      if (err.response) {
        errorMessage = err.response.data || `Error: ${err.response.status} ${err.response.statusText}`;
      } else if (err.request) {
        errorMessage = 'Cannot connect to server. Please make sure the backend is running.';
      } else {
        errorMessage = err.message || errorMessage;
      }
      setError(errorMessage);
      throw err;
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div style={{ padding: '20px', maxWidth: '800px', margin: '0 auto' }}>
      <h1>User Management</h1>
      
      <h2>Add New User</h2>
      {error && (
        <div style={{ 
          padding: '10px', 
          backgroundColor: '#ffebee', 
          color: '#c62828', 
          borderRadius: '4px',
          marginBottom: '10px'
        }}>
          Error: {error}
        </div>
      )}
      <UserForm onSubmit={handleAddUser} isLoading={isSubmitting} />

      <h2>Users</h2>
      <UserList users={users} isLoading={isLoading} error={null} />
    </div>
  );
}

