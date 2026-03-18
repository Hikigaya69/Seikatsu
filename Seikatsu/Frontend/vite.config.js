import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import basicSsl from '@vitejs/plugin-basic-ssl'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
    plugins: [
        react(),
        basicSsl(),
        tailwindcss()
    ],
    server: {
        https: true,
        port: 5173,
        hmr: {
            protocol: 'wss',
            host: 'localhost',
            port: 5173
        }
    }
})