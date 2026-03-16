import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import basicSsl from '@vitejs/plugin-basic-ssl'  // ← add this

export default defineConfig({
    plugins: [
        react(),
        basicSsl()    // ← add this ✅
    ],
    server: {
        https: true,
        port: 5173
    }
})