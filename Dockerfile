FROM mcr.microsoft.com/appsvc/staticappsclient:stable

WORKDIR /app
COPY . ./
RUN swa build

ENTRYPOINT [ "swa start" ]