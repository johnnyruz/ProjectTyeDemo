#See https://github.com/dotnet/tye/issues/720 to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM nginx:alpine
WORKDIR /var/www/web
COPY ./wwwroot .
COPY wwwroot/_framework/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
EXPOSE 443

