# VS Containers

right-click -> _add support for docker_
  creates the docker file
build will generate the image
  downloads the base image + creates new image for the app
  
VS will have a docker mode
  the container will be kept running, even if the app isn't
```
docker stop aff // you don't need the entire name; abbreviations are ok!
```

docker hub supports private registries
Azure Container Registry
  az cli, API support

You can publish to a registry directly from inside VS (no need to visit portal, etc)
- docker hub
- AZ
- Custom
