import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import Home from '../index';
import { api } from '../../services/api';

jest.mock('../../services/api');
const mockedApi = api as jest.Mocked<typeof api>;

describe('Home', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('renders user management page', () => {
    mockedApi.getUsers.mockResolvedValue([]);
    render(<Home />);
    expect(screen.getByText('User Management')).toBeInTheDocument();
    expect(screen.getByText('Add New User')).toBeInTheDocument();
    expect(screen.getByText('Users')).toBeInTheDocument();
  });

  it('fetches and displays users on load', async () => {
    const mockUsers = [
      { id: '1', name: 'John Doe', email: 'john@example.com' },
      { id: '2', name: 'Jane Smith', email: 'jane@example.com' },
    ];
    mockedApi.getUsers.mockResolvedValue(mockUsers);

    render(<Home />);

    await waitFor(() => {
      expect(mockedApi.getUsers).toHaveBeenCalled();
    });

    expect(screen.getByText('John Doe')).toBeInTheDocument();
    expect(screen.getByText('Jane Smith')).toBeInTheDocument();
  });

  it('displays empty state when no users', async () => {
    mockedApi.getUsers.mockResolvedValue([]);

    render(<Home />);

    await waitFor(() => {
      expect(screen.getByTestId('empty')).toBeInTheDocument();
    });
  });

  it('displays error when fetching users fails', async () => {
    mockedApi.getUsers.mockRejectedValue(new Error('API Error'));

    render(<Home />);

    await waitFor(() => {
      expect(screen.getByTestId('error')).toBeInTheDocument();
    });
  });

  it('adds user and updates list', async () => {
    const user = userEvent.setup();
    const mockUsers = [
      { id: '1', name: 'John Doe', email: 'john@example.com' },
    ];
    const newUser = {
      id: '2',
      name: 'Jane Smith',
      email: 'jane@example.com',
    };

    mockedApi.getUsers.mockResolvedValue(mockUsers);
    mockedApi.createUser.mockResolvedValue(newUser);

    render(<Home />);

    await waitFor(() => {
      expect(screen.getByText('John Doe')).toBeInTheDocument();
    });

    await user.type(screen.getByTestId('name-input'), 'Jane Smith');
    await user.type(screen.getByTestId('email-input'), 'jane@example.com');
    await user.click(screen.getByTestId('submit-button'));

    await waitFor(() => {
      expect(mockedApi.createUser).toHaveBeenCalledWith({
        name: 'Jane Smith',
        email: 'jane@example.com',
      });
      expect(screen.getByText('Jane Smith')).toBeInTheDocument();
    });
  });
});

