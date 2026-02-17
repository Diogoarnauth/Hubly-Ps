import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react-swc'

export default defineConfig({
  plugins: [react()],
  server: {
    host: true,     
    port: 5173,      
    allowedHosts: ['hubly-web'], 
    watch: {
      usePolling: true, 
      interval: 100,   
    },
  },
})