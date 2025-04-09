import nextJest from 'next/jest';

const createJestConfig = nextJest({
  dir: './', // Diretório raiz do projeto
});

const customJestConfig = {
  setupFilesAfterEnv: ['<rootDir>/jest.setup.ts'],
  testEnvironment: 'jsdom',
  transform: {
    '^.+\\.(ts|tsx)$': 'ts-jest',
  },
  moduleNameMapper: {
    '^@/(.*)$': '<rootDir>/src/$1', // se estiver usando imports absolutos
  },
};

export default createJestConfig(customJestConfig);
