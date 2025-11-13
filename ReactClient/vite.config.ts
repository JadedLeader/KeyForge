import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';
import tailwindcss from '@tailwindcss/vite'

const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ''
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

const certificateName = "reactapp2.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    if (0 !== child_process.spawnSync('dotnet', [
        'dev-certs',
        'https',
        '--export-path',
        certFilePath,
        '--format',
        'Pem',
        '--no-password',
    ], { stdio: 'inherit', }).status) {
        throw new Error("Could not create certificate.");
    }
}




const apiTargets = {
    account: 'https://localhost:7003',
    auth: 'https://localhost:7010',
    vault: 'https://localhost:7149',
    vaultKeys: 'https://localhost:7130',
};


// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin(), tailwindcss()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url)),
        },
    },
    server: {
        port: 5173,
        https: {
            key: fs.readFileSync(keyFilePath),
            cert: fs.readFileSync(certFilePath),
        },
        proxy: {
            '/Account': {
                target: apiTargets.account,
                changeOrigin: true,
                secure: false, // self-signed certs
            },
            '/Auth': {
                target: apiTargets.auth,
                changeOrigin: true,
                secure: false,
            },
            '/VaultKeys': {
                target: apiTargets.vaultKeys,
                changeOrigin: true,
                secure: false,
            },
            '/Vault': {
                target: apiTargets.vault,
                changeOrigin: true,
                secure: false,
            },
            
        },
    },
});
