import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import path from "path"   
import { fileURLToPath } from 'url'   


const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)
export default defineConfig({
    plugins: [
        react(),
        tailwindcss()
    ],
    resolve: {                     
        alias: {
            "@": path.resolve(__dirname, "./src"),
        },
    },
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