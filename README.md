# ProjectTyeDemo
This project will demonstrate some of the features of the Microsoft Project Tye experiment by deploying a simple photo sharing site.

- .NET Core WebAPI Backend Services
- Python photo processing service
- Blazor Frontend
- Razor Pages Frontend
- MongoDB
- RabbitMQ

### Running Locally

1. Install Docker Desktop
2. Install .NET Core 3.1 (or higher) SDK
3. Install Project Tye (https://github.com/dotnet/tye/blob/master/docs/getting_started.md)
4. Clone this repository
5. CD into the ProjectTyeDemo folder
6. Build the solution

    `dotnet build`

7. Run with Tye

    `tye run`

8. Navgate to http://localhost:8000 to view the Tye Dashboard & launch the services + frontend

### Deploying to Azure Kubernetes Service

1. Create an Azure Container Registry and Azure Kubernetes Service in Azure
2. Authenticate with your Azure CR
```
    docker login [your registry]
```
3. Authenticate with your Kubernetes Cluster
```
    az aks get-credentials --resource-group [rgname] --name [aks-cluster-name]
```
4. Update **tye.yaml** to change the **frontendblazor** to Prod environment variable
```
  env:
#    - ASPNETCORE_ENVIRONMENT=Development
    - ASPNETCORE_ENVIRONMENT=Prod
```
5. Deploy dependencies to your Kubernetes Cluster
```
    kubectl apply -f deploy_dependencies.yml
```
6. Deploy Tye application to the Kubernetes Cluster
```
tye deploy
```
7. When prompted, enter the following
```
    Container Registry: [azure container registry you created in step 1]
    Mongo: mongodb://mongo:27017
    RabbitMq1: rabbitmq://rabbitmq:5672
    RabbitMq2: http://rabbitmq:15672
```
8. Once everything has been deployed, find your Ingress Ip Address and create an entry in your hosts file (C:\Windows\System32\drivers\etc\hosts) to use the hostname of your application. Default is project-tye.kubernetes but this can be changed in the FrontendBlazor.Program.cs file.
```
    kubectl get ingress ***to get IP address of your ingress controller***

    ***In hosts file add the following line***
    [Ingress IP Address] project-tye.kubernetes
```
9. You should now be able to navigate to http://project-tye.kubernetes in your browser to have a fully operational app.

