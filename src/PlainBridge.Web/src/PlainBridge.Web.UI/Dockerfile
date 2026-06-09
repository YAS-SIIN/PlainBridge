# Stage 1: Build Angular app
FROM node:20-alpine AS build
WORKDIR /app
# Install dependencies
COPY package*.json ./
RUN npm install --legacy-peer-deps
# Copy source and build
COPY . .
RUN npm run build

# Stage 2: Nginx to serve built files
FROM nginx:alpine AS final
# Remove default site config
RUN rm /etc/nginx/conf.d/default.conf
# Copy custom nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf
# Copy built app to nginx html folder (Angular outputs browser/ for client bundle)
COPY --from=build /app/dist/PlainBridge.Web.UI/browser/ /usr/share/nginx/html/
EXPOSE 8080
CMD ["nginx", "-g", "daemon off;"]
