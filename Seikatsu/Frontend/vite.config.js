import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
    plugins: [
        react(),
       
        tailwindcss()
    ],
    server: {
     
        port: 5173,
        hmr: {
            protocol: 'wss',
            host: 'localhost',
            port: 5173
        },
        proxy: {                             
            "/api": {
                target: "https://localhost:7115",
                changeOrigin: true,
                secure: false,                    
            },
        },
    },
})