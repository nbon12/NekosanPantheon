import axios from 'axios';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:7071/api/HelloWorld';

export interface User {
  id: string;
  name: string;
  email: string;
}

export interface CreateUserRequest {
  name: string;
  email: string;
}

export const api = {
  async getUsers(): Promise<User[]> {
    try {
      console.log('Fetching users from:', API_URL);
      const response = await axios.get<User[]>(API_URL);
      console.log('Users fetched successfully:', response.data);
      return response.data;
    } catch (error: any) {
      console.error('Error fetching users:', error);
      if (error.response) {
        console.error('Response status:', error.response.status);
        console.error('Response data:', error.response.data);
      } else if (error.request) {
        console.error('No response received:', error.request);
      }
      throw error;
    }
  },

  async createUser(user: CreateUserRequest): Promise<User> {
    try {
      console.log('Creating user:', user, 'at:', API_URL);
      const response = await axios.post<User>(API_URL, user, {
        headers: {
          'Content-Type': 'application/json',
        },
      });
      console.log('User created successfully:', response.data);
      return response.data;
    } catch (error: any) {
      console.error('Error creating user:', error);
      if (error.response) {
        console.error('Response status:', error.response.status);
        console.error('Response data:', error.response.data);
        console.error('Response headers:', error.response.headers);
      } else if (error.request) {
        console.error('No response received. Request:', error.request);
        console.error('This usually means CORS issue or server not reachable');
      } else {
        console.error('Error setting up request:', error.message);
      }
      throw error;
    }
  },
};

