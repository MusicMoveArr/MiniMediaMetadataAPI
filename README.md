# MiniMedia Metadata API
At last, an self-hosted Metadata API that supports multiple providers (Spotify, Tidal, MusicBrainz)

The Restful API is quite straightforward, still a work in progress but does work

It's using the same database as https://github.com/MusicMoveArr/MiniMediaScanner

So you can already start using the metadata you synchronized in the database

Loving the work I do? buy me a coffee https://buymeacoffee.com/musicmovearr

## Used by projects,
https://github.com/MusicMoveArr/MusicMover

# Supported Providers
1. MusicBrainz
2. Tidal
3. Spotify

# Features
1. Low memory footprint (<250MB)
2. Postgres support
3. Search Artists Metadata by ProviderType (Any, Tidal, MusicBrainz, Spotify)
4. Search Albums Metadata by ProviderType (Any, Tidal, MusicBrainz, Spotify)
5. Search Tracks Metadata by ProviderType (Any, Tidal, MusicBrainz, Spotify)

## Docker compose
```
services:
  minimediametadataapi:
    container_name: MiniMediaMetadataAPI
    deploy:
      resources:
        limits:
          memory: 256M
    hostname: MiniMediaMetadataAPI
    image: musicmovearr/minimediametadataapi:main
    ports:
      - target: 8080
        published: "56232"
        protocol: tcp
    restart: unless-stopped
    volumes:
      - type: bind
        source: /DATA/AppData/minimediametadataapi/config/appsettings.json
        target: /app/appsettings.json
```

## Docker Run
```
docker run -d \
  --name MiniMediaMetadataAPI \
  --hostname MiniMediaMetadataAPI \
  --memory 256m \
  -p 56232:8080 \
  --restart unless-stopped \
  -v /DATA/AppData/minimediametadataapi/config/appsettings.json:/app/appsettings.json \
  musicmovearr/minimediametadataapi:main
```

## Example Configuration
It's important you bind this file to /app/appsettings.json as above in the docker examples

Change the connectionstring to your own postgres database

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "DatabaseConfiguration":{
    "ConnectionString": "Host=192.168.1.1;Username=postgres;Password=postgres;Database=minimedia;Pooling=true;MinPoolSize=5;MaxPoolSize=100;"
  }
}
```
