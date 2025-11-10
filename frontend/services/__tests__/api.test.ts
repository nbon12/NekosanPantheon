import axios from 'axios';
import { api, User } from '../api';

jest.mock('axios');
const mockedAxios = axios as jest.Mocked<typeof axios>;

describe('api', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('getUsers', () => {
    it('fetches users successfully', async () => {
      const mockUsers: User[] = [
        { id: '1', name: 'John Doe', email: 'john@example.com' },
        { id: '2', name: 'Jane Smith', email: 'jane@example.com' },
      ];

      mockedAxios.get.mockResolvedValue({ data: mockUsers });

      const result = await api.getUsers();

      expect(result).toEqual(mockUsers);
      expect(mockedAxios.get).toHaveBeenCalledWith(process.env.NEXT_PUBLIC_API_URL || 'http://localhost:7071/api/HelloWorld');
    });

    it('handles errors when fetching users', async () => {
      const error = new Error('Network Error');
      mockedAxios.get.mockRejectedValue(error);

      await expect(api.getUsers()).rejects.toThrow('Network Error');
    });
  });

  describe('createUser', () => {
    it('creates user successfully', async () => {
      const newUser: User = {
        id: '1',
        name: 'John Doe',
        email: 'john@example.com',
      };

      mockedAxios.post.mockResolvedValue({ data: newUser });

      const result = await api.createUser({
        name: 'John Doe',
        email: 'john@example.com',
      });

      expect(result).toEqual(newUser);
      expect(mockedAxios.post).toHaveBeenCalledWith(
        process.env.NEXT_PUBLIC_API_URL || 'http://localhost:7071/api/HelloWorld',
        { name: 'John Doe', email: 'john@example.com' }
      );
    });

    it('handles errors when creating user', async () => {
      const error = new Error('Network Error');
      mockedAxios.post.mockRejectedValue(error);

      await expect(
        api.createUser({
          name: 'John Doe',
          email: 'john@example.com',
        })
      ).rejects.toThrow('Network Error');
    });
  });
});

