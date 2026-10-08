import { defineConfig } from 'vite'

export default defineConfig({
  server: {
    host: '0.0.0.0',
    proxy: {
      '/api': 'http://localhost:5200',
      '/health': 'http://localhost:5200',
      '/openapi': 'http://localhost:5200',
      '/hubs': { target: 'http://localhost:5200', ws: true },
    },
  },
})
