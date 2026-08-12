import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { api } from '../services/api';
import type { AuthResponseDto, CurrentUserResponseDto, LoginDto, RegisterLecturerDto, RegisterStudentDto } from '../types/api';
import type { ReactNode } from 'react';

type AuthContextValue = {
  user: CurrentUserResponseDto | null;
  token: string | null;
  loading: boolean;
  login: (dto: LoginDto) => Promise<void>;
  registerStudent: (dto: RegisterStudentDto) => Promise<void>;
  registerLecturer: (dto: RegisterLecturerDto) => Promise<void>;
  logout: () => void;
  refreshMe: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | null>(null);
const TOKEN_KEY = 'aa_token';

function saveAuth(result: AuthResponseDto) {
  localStorage.setItem(TOKEN_KEY, result.token);
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => localStorage.getItem(TOKEN_KEY));
  const [user, setUser] = useState<CurrentUserResponseDto | null>(null);
  const [loading, setLoading] = useState(true);

  const refreshMe = useCallback(async () => {
    if (!localStorage.getItem(TOKEN_KEY)) {
      setUser(null);
      return;
    }

    const current = await api.me();
    setUser(current);
  }, []);

  useEffect(() => {
    let mounted = true;

    async function bootstrap() {
      const savedToken = localStorage.getItem(TOKEN_KEY);
      if (!savedToken) {
        if (mounted) {
          setLoading(false);
        }
        return;
      }

      try {
        const current = await api.me();
        if (mounted) {
          setToken(savedToken);
          setUser(current);
        }
      } catch {
        localStorage.removeItem(TOKEN_KEY);
        if (mounted) {
          setToken(null);
          setUser(null);
        }
      } finally {
        if (mounted) {
          setLoading(false);
        }
      }
    }

    bootstrap();
    return () => {
      mounted = false;
    };
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      token,
      loading,
      login: async (dto) => {
        const result = await api.login(dto);
        saveAuth(result);
        setToken(result.token);
        await refreshMe();
      },
      registerStudent: async (dto) => {
        const result = await api.registerStudent(dto);
        saveAuth(result);
        setToken(result.token);
        await refreshMe();
      },
      registerLecturer: async (dto) => {
        const result = await api.registerLecturer(dto);
        saveAuth(result);
        setToken(result.token);
        await refreshMe();
      },
      logout: () => {
        localStorage.removeItem(TOKEN_KEY);
        setToken(null);
        setUser(null);
      },
      refreshMe,
    }),
    [loading, refreshMe, token, user]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return ctx;
}
