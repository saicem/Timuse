import {defineConfig} from "vite"
import vue from "@vitejs/plugin-vue"

// https://vitejs.dev/config/
export default defineConfig(async () => ({
  plugins: [vue()],
  server: {
    port: 1420,
  },
}))
