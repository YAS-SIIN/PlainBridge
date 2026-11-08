import fs from 'node:fs';
import path from 'node:path';

const apiUrl = process.env.API_ENDPOINT_HTTPS;
const identityServerUrl = process.env.IDENTITYSERVER_ENDPOINT_HTTPS;

console.log('[Aspire Config] Environment variables:');
console.log('API_ENDPOINT_HTTPS:', apiUrl);
console.log('IDENTITYSERVER_ENDPOINT_HTTPS:', identityServerUrl);

if (!apiUrl || !identityServerUrl) {
    throw new Error('Aspire endpoint environment variables are missing.');
}

const config = {
    apiUrl,
    identityServerUrl
};

const configPath = path.resolve('public/assets/config.json');

fs.mkdirSync(path.dirname(configPath), { recursive: true });

fs.writeFileSync(configPath, JSON.stringify(config, null, 2), 'utf8');

console.log('[Aspire Config] config.json generated successfully.');
console.log(config);