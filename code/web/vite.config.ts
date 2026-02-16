import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react-swc'

export default defineConfig({
  plugins: [react()],
  server: {
    host: true,      // Permite ligações externas ao contentor
    port: 5173,      // Garante que usa a porta padrão
    allowedHosts: ['hubly-web'] // Autoriza o Nginx a falar com o Vite
  }
})